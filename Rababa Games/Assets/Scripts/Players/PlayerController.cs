using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private PlayerInput playerInput; // drag dari Inspector
    [SerializeField] private string moveActionName = "Move Player 1";
    [SerializeField] private string throwActionName = "Throw";

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Rocket Settings")]
    [SerializeField] private float throwForce = 20f;

    private Rigidbody rb;

    private InputAction moveAction;
    private InputAction throwAction;

    private Vector3 moveInput;
    private Vector3 velocity;

    private bool isGrounded;

    private PlayerStatus playerStatus;
    private Animator animator;
    private PickupRocket heldRocket;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        playerStatus = GetComponent<PlayerStatus>();

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput belum di-assign di inspector!", this);
            enabled = false;
            return;
        }

        moveAction = playerInput.actions[moveActionName];
        throwAction = playerInput.actions[throwActionName];
    }

    private void Start()
    {
        animator = playerStatus.Animator;
    }

    private void OnEnable()
    {
        throwAction.performed += OnThrowPerformed;
        moveAction.Enable();
        throwAction.Enable();
    }

    private void OnDisable()
    {
        throwAction.performed -= OnThrowPerformed;
        moveAction.Disable();
        throwAction.Disable();
    }

    private void Update()
    {
        ReadInput();
        AnimateMovement();
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyGravity();
    }

    private void ReadInput()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        moveInput = new Vector3(input.x, 0f, input.y);
    }

    private void MovePlayer()
    {
        Vector3 move = moveInput.normalized * moveSpeed;
        Vector3 newVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        rb.linearVelocity = newVelocity;

        if (moveInput != Vector3.zero)
        {
            transform.forward = moveInput;
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }
    }

    private void AnimateMovement()
    {
        animator.SetBool("IsRunning", moveInput.sqrMagnitude > 0.01f);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void OnThrowPerformed(InputAction.CallbackContext context)
    {
        if (heldRocket == null) return;

        animator.SetTrigger("IsThrowing");
        heldRocket.Throw(transform.forward);
        heldRocket = null;
    }

    public void SetHeldRocket(PickupRocket rocket)
    {
        heldRocket = rocket;
    }
}
