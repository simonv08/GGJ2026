using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public enum MaskType
    {
        moretta,
        bautta,
        scaramouche
    }

    public MaskType currentMask;

    [Header("Movement")]
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float gravity = -20f;

    [Header("Jump")]
    [SerializeField] public float jumpForce = 7f;

    [Header("Crouch")]
    [SerializeField] public float crouchHeight = 1f;
    [SerializeField] public float crouchSpeedMultiplier = 0.5f;
    [SerializeField] public float crouchSmoothSpeed = 12f;

    private CharacterController controller;
    private InputSystem_Actions inputActions;

    private Vector2 moveInput;
    private float verticalVelocity;
    private bool isCrouching;

    private float standingHeight;
    private Vector3 standingCenter;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();

        // Store original capsule values
        standingHeight = controller.height;
        standingCenter = controller.center;
    }

    void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Crouch.performed += OnCrouch;
        inputActions.Player.Crouch.canceled += OnCrouch;
    }

    void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Crouch.performed -= OnCrouch;
        inputActions.Player.Crouch.canceled -= OnCrouch;

        inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (controller.isGrounded)
            verticalVelocity = jumpForce;
    }

    private void OnCrouch(InputAction.CallbackContext context)
    {
        isCrouching = context.ReadValueAsButton();
    }

    void Update()
    {
        HandleMovement();
        HandleCrouch();
    }

    private void HandleMovement()
    {
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);

        if (movement.magnitude > 1)
            movement.Normalize();

        float speed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;

        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = movement * speed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        float targetHeight = isCrouching ? crouchHeight : standingHeight;

        // Save foot position BEFORE resizing
        float bottomBefore = transform.position.y + controller.center.y - controller.height / 2f;

        // Smooth height interpolation
        float newHeight = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchSmoothSpeed);
        controller.height = newHeight;

        // Adjust center so capsule shrinks from the top
        float heightOffset = (standingHeight - controller.height) / 2f;
        controller.center = new Vector3(
            standingCenter.x,
            standingCenter.y - heightOffset,
            standingCenter.z
        );

        // Restore foot position AFTER resizing
        float bottomAfter = transform.position.y + controller.center.y - controller.height / 2f;
        transform.position += Vector3.up * (bottomBefore - bottomAfter);
    }
}
