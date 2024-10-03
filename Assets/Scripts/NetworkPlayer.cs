using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    // Rigidbody of the player character.
    [Tooltip("Player is moved by applying force onto this Rigidbody.")]
    [SerializeField]
    private Rigidbody playerRigidbody;
    // Speed at which the player character is rotated towards the camera direction.
    [Tooltip("Float variable that controls how fast the player is rotated towards the camera direction.")]
    [SerializeField]
    private float rotationSpeed;
    // Float used to measure how much time the player spent falling. Higher value increases fall speed.
    [Tooltip("Timer that starts as soon as player starts falling.")]
    [Header("Falling")]
    [SerializeField]
    private float inAirTimer;
    // Float that is used to add force to the player when they walk off an edge of a surface.
    [Tooltip("Force applied to the player when they walk off an edge of a surface.")]
    [SerializeField]
    private float leapingVelocity;
    // Float that is used in the add force method to push the player down when they are falling.
    [Tooltip("Force applied to the player when they are falling.")]
    [SerializeField]
    private float fallingVelocity;
    // Amount of offset on the starting point from which Ray Cast is made.
    public float rayCastHeightOffset;
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
    [SerializeField]
    bool isJumpButtonPressed = false;
    [SerializeField]
    bool isAttackButtonPressed = false;

    // Controller settings
    [SerializeField]
    float maxSpeed;

    //Syncing of physics objects
    SyncPhysicsObject[] syncPhysicsObjects;

    [SerializeField]
    List<Collider> ragdollParts = new List<Collider>();

    public List<Collider> collidingParts = new List<Collider>();

    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private AnimatorManager animatorManager;
    [SerializeField]
    private PlayerManager playerManager;

    Vector3 moveDirection;
    Vector3 playerVelocity;
    [SerializeField]
    Transform cameraObject;
    [SerializeField]
    private Transform animatedAvatarTransform;
    [SerializeField]
    private Transform physicsAvatarTransform;
    [SerializeField]
    private Collider playerCollider;

    [SerializeField]
    Camera cam;

    [SerializeField]
    ConfigurableJoint hips;

    private ConfigurableJoint[] joints;
    private JointDrive[] jointDriveBackup;
    bool jumpInProgress = false;

    private void Awake()
    {
        syncPhysicsObjects = GetComponentsInChildren<SyncPhysicsObject>();
        joints = this.gameObject.GetComponentsInChildren<ConfigurableJoint>();
        SetRagdollParts();
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
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
            moveDirection.y = jumpingVelocity;
            playerRigidbody.AddForce(moveDirection);
        } else
        {
            Vector3 movementVelocity = moveDirection;
            playerRigidbody.AddForce(movementVelocity);
        }
    }

    private void HandleRotation()
    {
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
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffset;
        targetPosition = transform.position;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }

            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(Vector3.down * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, maxDistance, groundLayer))
        {
            if (!isGrounded && playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Landing", true);
            } 

            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
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
            playerRigidbody.AddForce(transform.up * jumpHeight, ForceMode.Impulse);

            float jumpingvelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            playerVelocity = moveDirection;
            playerVelocity.y = jumpingvelocity;
            playerRigidbody.AddForce(playerVelocity);
        }
    }

    public void HandleAttacking()
    {
        isAttacking = true;
        animatorManager.PlayTargetAnimation("Attack", false);
        animatorManager.animator.SetBool("isAttacking", true);
    }

    public void HandleHeavyAttacking()
    {
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

    public void Death()
    {
        jointDriveBackup = new JointDrive[joints.Length];

        for (int i = 0; i < joints.Length; i++)
        {
            Debug.Log("Saving joint " + i);
        }

        JointDrive newDrive = new JointDrive();
        newDrive.positionSpring = 0;

        hips.zMotion = ConfigurableJointMotion.Free;

        for (int i = 0; i < joints.Length; i++)
        {
            jointDriveBackup[i] = joints[i].slerpDrive;
            joints[i].slerpDrive = newDrive;
        }
    }

    public void Respawn()
    {
        hips.zMotion = ConfigurableJointMotion.Locked;
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].slerpDrive = jointDriveBackup[i];
            Debug.Log("III. Loading joint " + i);
        }
    }
}
