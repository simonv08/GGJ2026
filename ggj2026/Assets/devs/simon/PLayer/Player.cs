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
    
    MorettaMask morettaMask;
    BouttaMask bouttaMask;
    ScaramoucheMask scaramoucheMask;

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

    ShameEnemy currentEnemy;

    [SerializeField] private float health;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();

        // Store original capsule values
        standingHeight = controller.height;
        standingCenter = controller.center;

        health = 100;
        
        scaramoucheMask = gameObject.GetComponent<ScaramoucheMask>();
        bouttaMask = gameObject.GetComponent<BouttaMask>();
        morettaMask = gameObject.GetComponent<MorettaMask>();
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

    void FixedUpdate()
    {
        HandleMovement();
        HandleCrouch();
    }

    void Update()
    {
        ShameEnemy enemyThisFrame = null;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            enemyThisFrame = hit.collider.GetComponent<ShameEnemy>();

            if (enemyThisFrame != null)
            {
                enemyThisFrame.Hide();
            }
        }

        // Als we vorige frame een enemy hadden, maar nu niet meer
        if (currentEnemy != null && currentEnemy != enemyThisFrame)
        {
            currentEnemy.Show(); // terug naar normal
        }

        currentEnemy = enemyThisFrame;
        
        handleState();
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

        // Apply movement
        Vector3 velocity = movement * speed;
        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        // --- Rotate player toward movement direction ---
        Vector3 horizontalMovement = new Vector3(movement.x, 0f, movement.z);

        if (horizontalMovement.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalMovement, Vector3.up);

            // Smooth rotation
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 25f // rotation speed (tweakable)
            );
        }
    }

    private void handleState()
    {
        switch (currentMask)
        {
            case MaskType.moretta:
                morettaMask.enabled = true;
                bouttaMask.enabled = false;
                scaramoucheMask.enabled = false;
                break;
            case MaskType.bautta:
                morettaMask.enabled = false;
                bouttaMask.enabled = true;
                scaramoucheMask.enabled = false;
                break;
            case MaskType.scaramouche:
                morettaMask.enabled = false;
                bouttaMask.enabled = false;
                scaramoucheMask.enabled = true;
                break;
        }
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

    public void DoDamage(float damage)
    {
        health -= damage;
    }



    public void PutMask1()
    {
        currentMask = MaskType.bautta;
    }

    public void PutMask2()
    {
        currentMask = MaskType.moretta;
    }

    public void PutMask3()
    {
        currentMask = MaskType.scaramouche;
    }
}
