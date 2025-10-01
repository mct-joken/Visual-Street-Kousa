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

    [Header("リスポーン設定")]
    public Vector3 respawnPoint = new Vector3(0, 300, 0);
    public float fallThreshold = -200f;

    [Header("最大落下速度")]
    public float fallSpeedLimit = -50f;

    [Header("フライト設定")]
    public float flightSpeed = 8f;
    public float doubleTapTime = 0.3f; // ダブルタップ判定時間

    private Vector3 viewVec = new Vector3(1, 0, 0);
    private Vector3 moveVec;
    private Vector3 camPos;
    private Rigidbody Rigidbody;
    private BoxCollider groundCollider;
    private bool isGround = false;
    private bool cursorLocked = true;

    // フライト関連
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
        // --- Escでカーソル状態を切り替え ---
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLocked = !cursorLocked;
            LockCursor(cursorLocked);
        }

        if (cursorLocked)
        {
            Rigidbody.isKinematic = false; // 再開

            // --- カメラ操作 ---
            float mouse_x = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouse_y = Input.GetAxis("Mouse Y") * mouseSensitivity;
            Vector3 nomVec = new Vector3(viewVec.z, viewVec.y, -viewVec.x);
            viewVec += nomVec.normalized * mouse_x;
            nomVec = Vector3.up;
            viewVec += nomVec.normalized * mouse_y;
            viewVec = viewVec.normalized;

            // --- フライト切替（ダブルスペース検出） ---
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Time.time - lastSpaceTime <= doubleTapTime)
                {
                    isFlying = !isFlying;
                    Rigidbody.useGravity = !isFlying;
                    if (isFlying)
                        Rigidbody.velocity = Vector3.zero; // 浮遊開始時に慣性を消す
                }
                lastSpaceTime = Time.time;
            }

            if (isFlying)
                HandleFlightMovement();
            else
                HandleNormalMovement();

            LookAtSet();

            // --- 落下速度制限 ---
            if (!isFlying && Rigidbody.velocity.y < fallSpeedLimit)
            {
                Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, fallSpeedLimit, Rigidbody.velocity.z);
            }

            // --- 落下判定 ---
            if (transform.position.y < fallThreshold)
            {
                Respawn();
            }
        }
        else
        {
            // UI操作中は完全停止
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.isKinematic = true;
        }
    }

    void HandleNormalMovement()
    {
        // --- 移動処理 ---
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

        // --- ジャンプ処理 ---
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            isGround = false;
            Rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    void HandleFlightMovement()
    {
        Vector3 flyVelocity = Vector3.zero;

        // 前後左右
        if (Input.GetKey(KeyCode.W))
            flyVelocity += new Vector3(viewVec.x, 0, viewVec.z).normalized * flightSpeed;
        if (Input.GetKey(KeyCode.S))
            flyVelocity += new Vector3(-viewVec.x, 0, -viewVec.z).normalized * flightSpeed;
        if (Input.GetKey(KeyCode.A))
            flyVelocity += new Vector3(-viewVec.z, 0, viewVec.x).normalized * flightSpeed;
        if (Input.GetKey(KeyCode.D))
            flyVelocity += new Vector3(viewVec.z, 0, -viewVec.x).normalized * flightSpeed;

        // 上昇・下降
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
        Debug.Log("リスポーンしました");
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
