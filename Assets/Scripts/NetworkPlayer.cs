using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class NetworkPlayer : MonoBehaviour
{
    [SerializeField]
    Transform footPosition;
    [SerializeField]
    Rigidbody playerRigidbody;

    [SerializeField]
    ConfigurableJoint mainJoint;

    [SerializeField]
    Animator animator;

    [SerializeField]
    float rotationSpeed;

    [Header("Falling")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffSet = 0.5f;
    public LayerMask groundLayer;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;

    [Header("Movement Speeds")]
    [SerializeField]
    float walkingSpeed;
    [SerializeField]
    float runningSpeed;
    [SerializeField]
    float sprintingSpeed;

    [Header("Jumping Speeds")]
    public float jumpHeight = 3;
    public float gravityIntensity = -15;

    [SerializeField]
    float jumpSpeed;

    //Input
    Vector2 moveInputVector = Vector2.zero;
    [SerializeField]
    bool isJumpButtonPressed = false;
    [SerializeField]
    bool isAttackButtonPressed = false;

    // Controller settings
    [SerializeField]
    float maxSpeed;

    // States
    //bool isGrounded = false;
    //bool isAttacking = false;

    // Raycasts
    RaycastHit[] raycastHits = new RaycastHit[10];

    //Syncing of physics objects
    SyncPhysicsObject[] syncPhysicsObjects;

    [SerializeField]
    List<Collider> ragdollParts = new List<Collider>();

    public List<Collider> collidingParts = new List<Collider>();

    public PlayerInputActions playerControls;
    private InputAction move;
    private InputAction jump;
    private InputAction lightAttack;
    private InputAction look;

    InputManager inputManager;
    AnimatorManager animatorManager;
    PlayerManager playerManager;

    Vector3 moveDirection;
    Transform cameraObject;

    [SerializeField]
    Camera cam;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        playerControls = new PlayerInputActions();
        syncPhysicsObjects = GetComponentsInChildren<SyncPhysicsObject>();
        SetRagdollParts();

        inputManager = GetComponent<InputManager>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();

        if (playerManager.isInteracting)
        {
            return;
        }

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (isJumping)
        {
            return;
        }

        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (isSprinting)
        {
            moveDirection = moveDirection * sprintingSpeed;
        } else
        {
            if (inputManager.moveAmount >= 0.5f)
            {
                moveDirection = moveDirection * runningSpeed;
            }
            else
            {
                moveDirection = moveDirection * walkingSpeed;
            }
        }

        Vector3 movementVelocity = moveDirection;
        playerRigidbody.velocity = movementVelocity;
    }

    private void HandleRotation()
    {
        if (isJumping) 
        { 
            return; 
        }

        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = footPosition.position;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffSet;

        if (!isGrounded && !isJumping)
        {
            Debug.Log("In air");
            if (!playerManager.isInteracting)
            {
                Debug.Log("Falling 1");
                animatorManager.PlayTargetAnimation("Falling", true);
            }

            Debug.Log("Falling 2");
            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, 0.5f, groundLayer))
        {
            Debug.Log("Raycast hit");
            if (!isGrounded && playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Landing", true);
            }

            inAirTimer = 0;
            isGrounded = true;
            playerManager.isInteracting = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void HandleJumping()
    {
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRigidbody.velocity = playerVelocity;
        }
    }

    private void OnEnable()
    {
        //move = playerControls.Player.Move;
        //move.Enable();

        //lightAttack = playerControls.Player.LightAttack;
        //lightAttack.Enable();
        //lightAttack.performed += LightAttack;

        //jump = playerControls.Player.Jump;
        //jump.Enable();
        //jump.performed += Jump;
    }

    private void OnDisable()
    {
        //move.Disable();
        ////look.Disable();
        //lightAttack.Disable();
        //jump.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //Move input
        //moveInputVector.x = Input.GetAxis("Horizontal");
        //moveInputVector.y = Input.GetAxis("Vertical");

        //moveInputVector = move.ReadValue<Vector2>();

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log("Jump button pressed.");
        //    isJumpButtonPressed = true;
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    isAttackButtonPressed = true;
        //}
    }

    private void FixedUpdate()
    {
        ////Assume that we are not grounded
        //isGrounded = false;

        ////Check if we are grounded
        //int numberOfHits = Physics.SphereCastNonAlloc(playerRigidbody.position, 0.1f, transform.up * -1, raycastHits, 0.5f);

        //// Check for valid results
        //for (int i = 0; i < numberOfHits; i++)
        //{
        //    //Ignore self hits
        //    if (raycastHits[i].transform.root == transform)
        //        continue;

        //    isGrounded = true;

        //    animator.SetBool("Jump", false);
        //    animator.SetBool("Grounded", true);

        //    break;
        //}
        ///*
        //if (isGrounded)
        //{
        //    animator.SetBool("Grounded", false);
        //    animator.SetBool("FreeFall", false);
        //    animator.SetBool("Jump", false);
        //}*/


        //Debug.Log(playerRigidbody.velocity.magnitude);
        //if (playerRigidbody.velocity.magnitude > maxSpeed)
        //{
        //    playerRigidbody.velocity = Vector3.ClampMagnitude(playerRigidbody.velocity, maxSpeed);
        //}

        ////Apply extra gravity to character to make it less floaty
        //if (!isGrounded)/*
        //    animator.SetBool("Grounded", false);
        //    animator.SetBool("Jump", false);
        //    animator.SetBool("FreeFall", true);*/
        //    playerRigidbody.AddForce(Vector3.down * 10);

        //float inputMagnitude = moveInputVector.magnitude;

        //Vector3 localVelocityVsForward = transform.forward * Vector3.Dot(transform.forward, playerRigidbody.velocity);

        //float localForwardVelocity = localVelocityVsForward.magnitude;

        //if (inputMagnitude != 0)
        //{
        //    Quaternion desiredDirection = Quaternion.LookRotation(new Vector3(moveInputVector.x, 0, moveInputVector.y * -1), transform.up);

        //    // Rotate target towards direction
        //    mainJoint.targetRotation = Quaternion.RotateTowards(mainJoint.targetRotation, desiredDirection, Time.fixedDeltaTime * rotationSpeed);

        //    //var forward = cam.transform.TransformDirection(Vector3.forward);
        //    //forward.y = 0;

        //    //var right = cam.transform.TransformDirection(Vector3.right);
        //    //Vector3 targetDirection = moveInputVector.x * right + moveInputVector.y * forward;

        //    //Vector3 lookDirection = targetDirection.normalized;
        //    //Quaternion freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
        //    //var diferenceRotation = freeRotation.eulerAngles.y - transform.eulerAngles.y;
        //    //var eulerY = transform.eulerAngles.y;

        //    //if (diferenceRotation < 0 || diferenceRotation > 0)
        //    //    eulerY = freeRotation.eulerAngles.y;
        //    //var euler = new Vector3(0, eulerY, 0);

        //    //mainJoint.targetRotation = Quaternion.RotateTowards(mainJoint.targetRotation, Quaternion.Euler(euler), rotationSpeed * Time.deltaTime);

        //    if (localForwardVelocity < maxSpeed)
        //    {
        //        //Move character in the direction it is facing
        //        playerRigidbody.AddForce(cam.transform.forward * inputMagnitude * movementSpeed);
        //    }
        //}

        //if (isGrounded && isJumpButtonPressed)
        //{
        //    animator.SetBool("Grounded", false);
        //    animator.SetBool("FreeFall", false);
        //    animator.SetBool("Jump", true);

        //    playerRigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);

        //    isJumpButtonPressed = false;
        //}

        //if (isAttackButtonPressed)
        //{
        //    animator.SetTrigger("Attacking");

        //    isAttackButtonPressed = false;
        //}

        //animator.SetFloat("Speed", localForwardVelocity * 0.4f);

        //Update the joints roation based on the animations
        for (int i = 0; i < syncPhysicsObjects.Length; i++)
        {
            syncPhysicsObjects[i].UpdateJointFromAnimation();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ragdollParts.Contains(other))
        {
            //Debug.Log("Touched itself");
            return;
        }

        /*
        NetworkPlayer control = other.transform.root.GetComponent<NetworkPlayer>();

        if (control == null)
        {
            return;
        }

        if (other.gameObject == control.gameObject)
        {
            return;
        }*/

        if (!collidingParts.Contains(other))
        {
            collidingParts.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (collidingParts.Contains(other))
        {
            collidingParts.Remove(other);
        }
    }

    private void SetRagdollParts()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != this.gameObject)
            {
                ragdollParts.Add(collider);
            }
        }
    }

    private void LightAttack(InputAction.CallbackContext context)
    {
        isAttackButtonPressed = true;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump button pressed.");
        isJumpButtonPressed = true;
    }

    //private void Look(InputAction.CallbackContext context)
    //{
    //    //Quaternion desiredDirection = Quaternion.LookRotation(new Vector3(moveInputVector.x, 0, moveInputVector.y * -1), transform.up);

    //    //// Rotate target towards direction
    //    //mainJoint.targetRotation = Quaternion.RotateTowards(mainJoint.targetRotation, desiredDirection, Time.fixedDeltaTime * rotationSpeed);

    //}
}
