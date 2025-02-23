using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float strafeSpeed = 3f;
    public float jumpForce = 10f;
    public float rotationSpeed = 100f; // Sensitivitas rotasi

    public Rigidbody hips;
    public bool isGrounded;

    public Animator anim;

    private Vector2 movementInput;
    private bool isSprinting;
    private bool isJumping;

    public ConfigurableJoint hipJoint;
    public ConfigurableJoint stomachJoin;
    public float stomachOffset;
    public PlayerInput playerInput;
    private float xRotation = 0f; // Menyimpan nilai rotasi vertikal

    private void Start()
    {
        hips = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        if (Gamepad.all.Count >= playerInput.playerIndex + 1)
        {
            playerInput.SwitchCurrentControlScheme("Gamepad", Gamepad.all[playerInput.playerIndex]);
        }
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
            anim.SetBool("IsWalk", true);
            anim.speed = isSprinting ? 1.5f : 1f;

        }
        else
        {
            if (anim == null) return;

            anim.SetBool("IsWalk", false);
        }

        // Jump

    }

    private void Update()
    {

        if (playerInput.devices.Count > 0)
        {
            Debug.Log($"{playerInput.gameObject.name} controlled by {playerInput.devices[0].displayName}");
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }




}
