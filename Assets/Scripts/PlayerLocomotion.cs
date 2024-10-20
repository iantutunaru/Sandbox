using UnityEngine;

/// <summary>
/// This class is responsible for moving the player's avatar by applying force on the player's rigdibody and playing the needed animations using the animator manager and the animator.
/// </summary>
public class PlayerLocomotion : MonoBehaviour
{
    [Header("Player Variables")]
    // Rigidbody of the player character.
    [Tooltip("Player is moved by applying force onto this Rigidbody.")]
    [SerializeField]
    private Rigidbody playerRigidbody;
    // Transform of the camera that is used to move the player relative to the direction which the camera is facing.
    [SerializeField]
    [Tooltip("Transform of the camera that is used to move the player relative to the direction which the camera is facing.")]
    private Transform cameraObject;
    // The first bone of the player which is attached to the player's main Rigidbody.
    [Tooltip("The first bone of the player which is attached to the player's main Rigidbody.")]
    [SerializeField]
    private ConfigurableJoint mainHoldingBone;
    [Header("Falling Variables")]
    // Float used to measure how much time the player spent falling. Higher value increases fall speed.
    [Tooltip("Timer that starts as soon as player starts falling.")]
    [SerializeField]
    private float inAirTimer = 0;
    // Float that is used to add force to the player when they walk off an edge of a surface.
    [Tooltip("Force applied to the player when they walk off an edge of a surface.")]
    [SerializeField]
    private float leapingVelocity = 10;
    // Float that is used in the add force method to push the player down when they are falling.
    [Tooltip("Force applied to the player when they are falling.")]
    [SerializeField]
    private float fallingVelocity = 10;
    // Amount of offset on the starting point from which Ray Cast is made.
    [Tooltip("Raycast starting point offset on the Y axis.")]
    [SerializeField]
    private float rayCastHeightOffset = 0.9f;
    // Max distance that raycast will check against.
    [Tooltip("Max distance that the raycast will check against.")]
    [SerializeField]
    private float maxDistance = 0.9f;
    
    // Layer of what is considered ground in the game.
    [Tooltip("Layer which the raycast is cast against to find if player has touched the ground.")]
    [SerializeField]
    private LayerMask groundLayer;

    // TO-DO: Create set and get methods for state booleans. Make booleans private.
    [Header("State Flags")]
    // Boolean to show if the player is currently sprinting.
    [SerializeField]
    public bool isSprinting;
    // Boolean to show if the player is currently on the ground.
    [SerializeField]
    public bool isGrounded;
    // Boolean to show if the player us currently jumping.
    [SerializeField]
    public bool isJumping;
    // Boolean to show if the player is currently attacking using light attack.
    [SerializeField]
    public bool isAttacking;
    // Boolean to show if the player us currently attacking using heavy attack.
    public bool isHeavyAttacking;
    
    [Header("Movement Speeds")]
    // Speed of the player when moving using a gamepad and joystick is half pressed.
    [Tooltip("Speed of the player when moving using a gamepad and joystick is half pressed.")]
    [SerializeField]
    private float walkingSpeed = 100;
    // Speed of the player when moving using a gamepad and joystick is fully pressed. Default move speed when using a keyboard.
    [Tooltip("Speed of the player when moving using a gamepad and joystick if fully pressed. Default move speed when using a keyboard.")]
    [SerializeField]
    private float runningSpeed = 150;
    // Speed of the player when the sprint button is pressed.
    [Tooltip("Speed of the player when the sprint button is pressed.")]
    [SerializeField]
    private float sprintingSpeed = 200;
    // Speed at which the player character is rotated towards the camera direction.
    [Tooltip("Float variable that controls how fast the player is rotated towards the camera direction.")]
    [SerializeField]
    private float rotationSpeed = 10;
    // Additional speed applied to the player if they are falling.
    [Tooltip("Speed of the player when they are falling and input is applied.")]
    [SerializeField]
    private float fallingSpeed = 50;

    [Header("Jumping Variables")]
    // Used to calculate the initial jump height and movement direction during jumping.
    [Tooltip("Used to calculate the initial jump height and movement direction during jumping.")]
    [SerializeField]
    private float jumpHeight = 100;
    // Used to simulate gravity pressure when jumping. Amount of force that is used to raise the player upwards.
    [Tooltip("Used to simulate gravity pressure when jumping. Amount of force that is used to raise the player upwards.")]
    [SerializeField]
    private float gravityIntensity = -15;

    [Header("Player Managers")]
    // Class that handles input. Here it's used to gain info vertical and horizontal input.
    [Tooltip("Class that handles input. Here it's used to gain info vertical and horizontal input.")]
    [SerializeField]
    private InputManager inputManager;
    // Class that handles calls to the Animator of the player.
    [Tooltip("Class that handles calls to the Animator of the player.")]
    [SerializeField]
    private AnimatorManager animatorManager;
    // Class that handles calls to update camera movement, and updates bools between this class and Animator.
    [Tooltip("Class that handles calls to update camera movement, and updates bools between this class and Animator.")]
    [SerializeField]
    private PlayerManager playerManager;

    // Direction in which the player is moved.
    Vector3 moveDirection;
    // Direction in which the player is moved during jumping;
    Vector3 playerVelocity;
    // Array of all Configurable Joints of the player used when triggering Ragdoll state.
    private ConfigurableJoint[] joints;
    // Array backup of Joint Drives of all Configurable Joints of the player before triggering Ragdoll state.
    private JointDrive[] jointDriveBackup;
    // Joint Drive that is used to reset the position spring of all Configrable Joints of the player's physical avatar in the case of death.
    private JointDrive newDrive;
    // List of all physical player parts whose rotation in space is updated from the animated avatar's skeleton.
    private SyncPhysicsObject[] syncPhysicsObjects;


    /// <summary>
    /// Populates the lists of animated body parts, and configurable joints of the player's avatar.
    /// </summary>
    private void Awake()
    {
        syncPhysicsObjects = GetComponentsInChildren<SyncPhysicsObject>();
        joints = GetComponentsInChildren<ConfigurableJoint>();
        jointDriveBackup = new JointDrive[joints.Length];

        newDrive = new()
        {
            positionSpring = 0
        };
    }

    /// <summary>
    /// Handle player movement, rotation, falling, and ground checks.
    /// </summary>
    public void HandleAllMovement()
    {
        HandleFallingAndLanding();
        HandleMovement();
        HandleRotation();
    }

    /// <summary>
    /// Method that handles the movement of the character in the following steps:
    ///     1. Get movement direction from the camera direction and player input.
    ///     1.1. Normalize the values as we only need to know the direction that we need to move. Set vertical movement to zero so don't move up.
    ///     2. Check if the player is sprinting, walking or running and adjust the movement vector accordingly.
    ///     3. If the player is jumping then set the vertical axis of the movement vector using a math formula for projectile motion.
    /// </summary>
    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection += cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        // Check if the player is sprinting, falling or moving noramlly and adjust movement speed.
        if (isSprinting)
        {
            moveDirection *= sprintingSpeed;
        } else if (!isGrounded)
        {
            moveDirection *= fallingSpeed;
        }
        else
        {
            // Check if the player wants to run by checking how much the left stick is tilted.
            if (inputManager.moveAmount >= 0.5f)
            {
                moveDirection *= runningSpeed;
            }
            else
            {
                moveDirection *= walkingSpeed;
            }
        }

        // Check if the player is jumping and smooth out their ascent according to the math formula of projectile motion. 
        if (isJumping)
        {
            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            moveDirection.y = jumpingVelocity;
            playerRigidbody.AddForce(moveDirection);
        } else
        {
            playerRigidbody.AddForce(moveDirection);
        }
    }

    /// <summary>
    /// Method that rotates the avatar according to the camera direction and the player input.
    /// </summary>
    private void HandleRotation()
    {
        Vector3 targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection += cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        // If target direction wasn't changed then player is facing forward.
        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    /// <summary>
    /// Method that handles falling and landing of the avatar in the following way:
    ///     1. Checks if player is falling and if they are then play falling animation.
    ///     1.1. Increase player fall speed the longer they are falling.
    ///     2. Check if player hit the ground and play landing animation if they did.
    /// </summary>
    private void HandleFallingAndLanding()
    {
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += rayCastHeightOffset;

        // Check if player is not grounded and is not jumping, then that means that they are falling.
        if (!isGrounded && !isJumping)
        {
            // If any animation is not currently playing, then start playing the falling animation.
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }

            inAirTimer += Time.deltaTime;
            // If player walks off a ledge give him a little boost to make it seem that he leaped off a ledge.
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            // Accelerate player the longer they are falling.
            playerRigidbody.AddForce(fallingVelocity * inAirTimer * Vector3.down);
        }

        // Check if Ray Cast hits the ground, otherwise player is still falling.
        if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out _, maxDistance, groundLayer))
        {
            // If player is not grounded and animation is playing, then that means that they are falling. Play landing animation.
            if (!isGrounded && playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Landing", true);
            } 

            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    /// <summary>
    /// Method that plays jumping animation and moves the player's avatar according to the math formula o projectile motion.
    /// </summary>
    public void HandleJumping()
    {
        // Check if player is grounded which means that they can jump.
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

    /// <summary>
    /// Method that handles attack calls and calls on the animator and animator manager to start the attack animation.
    /// </summary>
    public void HandleAttacking()
    {
        isAttacking = true;
        animatorManager.PlayTargetAnimation("Attack", false);
        animatorManager.animator.SetBool("isAttacking", true);
    }

    /// <summary>
    /// Method that handles heavy attack calls and calls on the animator and animator manager to start the heavy attack animation.
    /// </summary>
    public void HandleHeavyAttacking()
    {
        isHeavyAttacking = true;
        animatorManager.animator.SetBool("isHeavyAttacking", true);
        animatorManager.PlayTargetAnimation("HeavyAttack", false);
    }

    /// <summary>
    /// In the FixedUpdate we are only calling on the method to update the player's avatar physical body according to the current animation.
    /// </summary>
    private void FixedUpdate()
    {
        UpdateJoints();
    }

    /// <summary>
    /// Method that updates the current rotation of all animated physical parts of the player's body.
    /// </summary>
    private void UpdateJoints()
    {
        //Update the joints roation based on the animations.
        for (int i = 0; i < syncPhysicsObjects.Length; i++)
        {
            syncPhysicsObjects[i].UpdateJointFromAnimation();
        }
    }

    /// <summary>
    /// Method that is called when the player dies.
    /// It looses the hips by allowing them to move along the Z axis and reset the position spring of each of the Configurable Joints of the player's avatar.
    /// </summary>
    public void Death()
    {
        mainHoldingBone.zMotion = ConfigurableJointMotion.Free;

        //Save the current slerp drive of all joints and then set them to 0 to create the "Ragdoll" effect.
        for (int i = 0; i < joints.Length; i++)
        {
            jointDriveBackup[i] = joints[i].slerpDrive;
            joints[i].slerpDrive = newDrive;
        }
    }

    /// <summary>
    /// Method that is called when the player respawns.
    /// It restores the Slerp Drive of all Configurable Joints in the palyer's avatar physical body.
    /// </summary>
    public void Respawn()
    {
        mainHoldingBone.zMotion = ConfigurableJointMotion.Locked;

        // Restore the slerp drive of all joints to the pre-death state.
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].slerpDrive = jointDriveBackup[i];
        }
    }
}
