using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float strafeSpeed = 3f;
    public float jumpForce = 10f;
    public float rotationSpeed = 100f; // Sensitivitas rotasi

    private Rigidbody hips;
    public bool isGrounded;

    public Animator anim;

    public Vector2 movementInput;
    private bool isSprinting;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isJumping;
    public CinemachineVirtualCamera virtualCamera;
    public ConfigurableJoint hipJoint;
    public ConfigurableJoint stomachJoin;
    public float stomachOffset;
    private PlayerInput playerInput;
    private float xRotation = 0f; // Menyimpan nilai rotasi vertikal

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        GameManager.Instance.player.Add(this.gameObject);
        if (GameManager.Instance.player.Count == 1)
        {
            int layer = LayerMask.NameToLayer("Player1Layer");
            int layerRemove = LayerMask.NameToLayer("Player2Layer");
            virtualCamera.gameObject.layer = layer;
            playerInput.camera.cullingMask &= ~(1 << layerRemove);
        }
        else
        {
            int layer = LayerMask.NameToLayer("Player2Layer");
            int layerRemove = LayerMask.NameToLayer("Player1Layer");
            virtualCamera.gameObject.layer = layer;
            playerInput.camera.cullingMask &= ~(1 << layerRemove);
        }

        hips = GetComponent<Rigidbody>();
        hipJoint = GetComponent<ConfigurableJoint>();

        GameManager.Instance.cameraController.Add(playerInput.camera);

    }

    // Input System Callbacks
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
            isSprinting = true;
        else if (context.canceled)
            isSprinting = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            isJumping = true;
        }
    }

    private void FixedUpdate()
    {
        if (hips == null) return;

        // Movement
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        float currentSpeed = isSprinting ? speed * 1.5f : speed;
        hips.AddForce(moveDirection * currentSpeed);

        if (isJumping)
        {
            hips.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            isGrounded = false;
            isJumping = false;
        }

        if (anim == null) return;
        // Animation
        if (movementInput.magnitude > 0)
        {
            isMoving = true;
            anim.SetBool("IsWalk", true);
            anim.speed = isSprinting ? 1.5f : 1f;

        }
        else
        {
            isMoving = false;
            if (anim == null) return;
            
            anim.SetBool("IsWalk", false);
        }

        // Jump

    }

    private void Update()
    {
        if (transform.position.y <= -10)
        {
            GameOver();
        }

        if (isGrounded)
        {
            anim.SetBool("IsJump", false);

        }
        else
        {
            anim.SetBool("IsJump", true);
            /*anim.SetBool("IsWalk", false);*/


        }
        // Rotasi karakter mengikuti arah pergerakan
        if (movementInput.magnitude > 0)
        {
            // Dapatkan rotasi berdasarkan input
            float targetRotation = Mathf.Atan2(movementInput.x, movementInput.y) * Mathf.Rad2Deg;
            hipJoint.targetRotation = Quaternion.Euler(0, -targetRotation, 0); // Rotasi horizontal

            // Terapkan rotasi pada stomachJoin (bagian tubuh)
            stomachJoin.targetRotation = Quaternion.Euler(-xRotation + stomachOffset, 0f, 0f);
        }
    }



    public void GameOver()
    {
        transform.position = GameManager.Instance.spawnPoint.transform.position;
        transform.rotation = GameManager.Instance.spawnPoint.transform.rotation;
    }


}
