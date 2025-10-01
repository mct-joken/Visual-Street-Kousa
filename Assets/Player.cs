using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 6f;
    public float mouseSensitivity = 0.1f;
    public float jumpPower = 4f;
    public GameObject camera;
    public GameObject cursor;

    [Header("���X�|�[���ݒ�")]
    public Vector3 respawnPoint = new Vector3(0, 300, 0);
    public float fallThreshold = -200f;

    [Header("�ő嗎�����x")]
    public float fallSpeedLimit = -50f;

    [Header("�t���C�g�ݒ�")]
    public float flightSpeed = 8f;
    public float doubleTapTime = 0.3f; // �_�u���^�b�v���莞��

    private Vector3 viewVec = new Vector3(1, 0, 0);
    private Vector3 moveVec;
    private Vector3 camPos;
    private Rigidbody Rigidbody;
    private BoxCollider groundCollider;
    private bool isGround = false;
    private bool cursorLocked = true;

    // �t���C�g�֘A
    private bool isFlying = false;
    private float lastSpaceTime = -1f;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        groundCollider = GetComponent<BoxCollider>();
        LookAtSet();
        LockCursor(true);
    }

    void Update()
    {
        // --- Esc�ŃJ�[�\����Ԃ�؂�ւ� ---
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLocked = !cursorLocked;
            LockCursor(cursorLocked);
        }

        if (cursorLocked)
        {
            Rigidbody.isKinematic = false; // �ĊJ

            // --- �J�������� ---
            float mouse_x = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouse_y = Input.GetAxis("Mouse Y") * mouseSensitivity;
            Vector3 nomVec = new Vector3(viewVec.z, viewVec.y, -viewVec.x);
            viewVec += nomVec.normalized * mouse_x;
            nomVec = Vector3.up;
            viewVec += nomVec.normalized * mouse_y;
            viewVec = viewVec.normalized;

            // --- �t���C�g�ؑցi�_�u���X�y�[�X���o�j ---
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Time.time - lastSpaceTime <= doubleTapTime)
                {
                    isFlying = !isFlying;
                    Rigidbody.useGravity = !isFlying;
                    if (isFlying)
                        Rigidbody.velocity = Vector3.zero; // ���V�J�n���Ɋ���������
                }
                lastSpaceTime = Time.time;
            }

            if (isFlying)
                HandleFlightMovement();
            else
                HandleNormalMovement();

            LookAtSet();

            // --- �������x���� ---
            if (!isFlying && Rigidbody.velocity.y < fallSpeedLimit)
            {
                Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, fallSpeedLimit, Rigidbody.velocity.z);
            }

            // --- �������� ---
            if (transform.position.y < fallThreshold)
            {
                Respawn();
            }
        }
        else
        {
            // UI���쒆�͊��S��~
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.isKinematic = true;
        }
    }

    void HandleNormalMovement()
    {
        // --- �ړ����� ---
        bool isMove = false;
        if (Input.GetKey(KeyCode.W))
        {
            isMove = true;
            moveVec = new Vector3(viewVec.x, 0, viewVec.z).normalized * speed;
            Rigidbody.velocity = new Vector3(moveVec.x, Rigidbody.velocity.y, moveVec.z);
        }
        if (Input.GetKey(KeyCode.S))
        {
            isMove = true;
            moveVec = new Vector3(-viewVec.x, 0, -viewVec.z).normalized * speed;
            Rigidbody.velocity = new Vector3(moveVec.x, Rigidbody.velocity.y, moveVec.z);
        }
        if (Input.GetKey(KeyCode.A))
        {
            isMove = true;
            moveVec = new Vector3(-viewVec.z, 0, viewVec.x).normalized * speed;
            Rigidbody.velocity = new Vector3(moveVec.x, Rigidbody.velocity.y, moveVec.z);
        }
        if (Input.GetKey(KeyCode.D))
        {
            isMove = true;
            moveVec = new Vector3(viewVec.z, 0, -viewVec.x).normalized * speed;
            Rigidbody.velocity = new Vector3(moveVec.x, Rigidbody.velocity.y, moveVec.z);
        }
        if (!isMove)
        {
            Rigidbody.velocity = new Vector3(0, Rigidbody.velocity.y, 0);
        }

        // --- �W�����v���� ---
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            isGround = false;
            Rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    void HandleFlightMovement()
    {
        Vector3 flyVelocity = Vector3.zero;

        // �O�㍶�E
        if (Input.GetKey(KeyCode.W))
            flyVelocity += new Vector3(viewVec.x, 0, viewVec.z).normalized * flightSpeed;
        if (Input.GetKey(KeyCode.S))
            flyVelocity += new Vector3(-viewVec.x, 0, -viewVec.z).normalized * flightSpeed;
        if (Input.GetKey(KeyCode.A))
            flyVelocity += new Vector3(-viewVec.z, 0, viewVec.x).normalized * flightSpeed;
        if (Input.GetKey(KeyCode.D))
            flyVelocity += new Vector3(viewVec.z, 0, -viewVec.x).normalized * flightSpeed;

        // �㏸�E���~
        if (Input.GetKey(KeyCode.Space))
            flyVelocity += Vector3.up * flightSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            flyVelocity += Vector3.down * flightSpeed;

        Rigidbody.velocity = flyVelocity;
    }

    void Respawn()
    {
        Rigidbody.velocity = Vector3.zero;
        transform.position = respawnPoint;
        Debug.Log("���X�|�[�����܂���");
    }

    void LockCursor(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        isGround = true;
    }

    void OnTriggerExit(Collider coll)
    {
        isGround = false;
    }

    void LookAtSet()
    {
        camPos = this.transform.position + viewVec;
        this.transform.LookAt(new Vector3(camPos.x, this.transform.position.y, camPos.z));
        camera.transform.position = new Vector3(camPos.x, this.transform.position.y + 0.5f, camPos.z);
        camera.transform.LookAt(camera.transform.position + viewVec);
    }
}
