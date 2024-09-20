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

    [Header("Movement")]
    [SerializeField]
    Transform rigidbodyTransform;

    [Header("Falling")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffSet = 0.5f;
    public LayerMask groundLayer;
    public float maxDistance;
    public float fallingSpeed;

    [Header("State Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;
    public bool isAttacking;
    public bool isHeavyAttacking;

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
    Vector3 playerVelocity;
    Transform cameraObject;
    [SerializeField]
    private Transform animatedAvatarTransform;
    [SerializeField]
    private Transform physicsAvatarTransform;
    [SerializeField]
    private Collider playerCollider;

    [SerializeField]
    Camera cam;


    bool jumpInProgress = false;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        playerControls = new PlayerInputActions();
        syncPhysicsObjects = GetComponentsInChildren<SyncPhysicsObject>();
        SetRagdollParts();

        inputManager = GetComponent<InputManager>();
        cameraObject = Camera.main.transform;
        //playerCollider = GetComponent<Collider>();
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();

        //if (playerManager.isInteracting)
        //{
        //    return;
        //}

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        //if (isJumping)
        //{
        //    return;
        //}

        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (isSprinting)
        {
            moveDirection = moveDirection * sprintingSpeed;
        } else if (!isGrounded)
        {
            moveDirection = moveDirection * fallingSpeed;
        }
        else
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

        if (isJumping && jumpInProgress == false)
        {
            jumpInProgress = true; 
            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            //Debug.Log("************Jumping y:" + jumpingVelocity);
            moveDirection.y = jumpingVelocity;
            //playerRigidbody.velocity = moveDirection;
            playerRigidbody.AddForce(moveDirection);

            //return;
        } else
        {
            Vector3 movementVelocity = moveDirection;
            //Debug.Log("Movement y: " + movementVelocity.y);
            //playerRigidbody.velocity = movementVelocity;
            playerRigidbody.AddForce(movementVelocity);
        }

        //Vector3 movementVelocity = moveDirection;
        //Debug.Log("Movement y: " + movementVelocity.y);
        //playerRigidbody.velocity = movementVelocity;
    }

    private void HandleRotation()
    {
        //if (isJumping) 
        //{ 
        //    return; 
        //}

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
        Vector3 rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffSet;
        targetPosition = transform.position;

        if (!isGrounded && !isJumping)
        {
            //Debug.Log("In air");
            if (!playerManager.isInteracting)
            {
                //Debug.Log("Falling 1");
                animatorManager.PlayTargetAnimation("Falling", true);
            }

            //Debug.Log("Falling 2");
            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, maxDistance, groundLayer))
        {
            //Debug.Log("Raycast hit");
            if (!isGrounded && playerManager.isInteracting)
            {
                //animatorManager.animator.StopPlayback();
                //animatorManager.animator.SetBool("isJumping", false);
                //isJumping = false;
                //jumpInProgress = false;
                animatorManager.PlayTargetAnimation("Landing", true);
            } 

            Vector3 rayCastHitPoint = hit.point;
            //Debug.Log("Raycast y = " + rayCastHitPoint.y);
            targetPosition.y = rayCastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;

            //playerManager.isInteracting = false;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void HandleJumping()
    {
        //Debug.Log("JUMPING");
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false);
            playerRigidbody.AddForce(transform.up * jumpHeight, ForceMode.Impulse);

            float jumpingvelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            playerVelocity = moveDirection;
            //Debug.Log("************jumping y:" + jumpingvelocity);
            playerVelocity.y = jumpingvelocity;
            //playerrigidbody.velocity = playervelocity;
            playerRigidbody.AddForce(playerVelocity);
        }
    }

    public void HandleAttacking()
    {
        //Debug.Log("ATTACKING");
        isAttacking = true;
        animatorManager.animator.SetBool("isAttacking", true);
        animatorManager.PlayTargetAnimation("Attack", false);
    }

    public void HandleHeavyAttacking()
    {
        //Debug.Log("HEAVY ATTACK");
        isHeavyAttacking = true;
        animatorManager.animator.SetBool("isHeavyAttacking", true);
        animatorManager.PlayTargetAnimation("HeavyAttack", false);
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
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
            return;
        }

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
}
