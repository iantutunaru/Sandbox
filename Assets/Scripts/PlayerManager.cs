using UnityEngine;

/// <summary>
/// This class handles input, animation and state calls of the player.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    // Reference to the animator of the player.
    [Tooltip("Reference to the animator of the player.")]
    [SerializeField]
    private Animator animator;
    // Player locomotion class to handle animation calls and movement relative to the inputs.
    [Tooltip("Player locomotion class to handle animation calls and movement relative to the inputs.")]
    [SerializeField]
    PlayerLocomotion playerLocomotion;

    [Header("Managers")]
    // Reference to the input manager of the player.
    [Tooltip("Reference to the input manager of the player.")]
    [SerializeField]
    private InputManager inputManager;
    // Reference to the camera manager of the player.
    [Tooltip("Reference to the camera manager of the player.")]
    [SerializeField]
    private CameraManager cameraManager;


    // TO-DO: Create setters and getters for the booleans and make them private.
    [Header("States")]
    // Boolean that is true when the player is performing an action during which other actions can't be performed.
    [Tooltip("Boolean that is true when the player is performing an action during which other actions can't be performed.")]
    public bool isInteracting;
    // Boolean that is true when the player is dead.
    [Tooltip("Boolean that is true when the player is dead.")]
    [SerializeField]
    public bool isDead = false;

    /// <summary>
    /// Here we are handling inputs from the player only if they are not dead.
    /// </summary>
    private void Update()
    {
        // Check if player is not dead so they are allowed to move.
        if (!isDead)
        {
            inputManager.HandleAllInputs();
        }
    }

    /// <summary>
    /// Update player movement every player update.
    /// </summary>
    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }

    /// <summary>
    /// Here we are calling on the camera manager to update camera movement.
    /// After, we are updating the states in this class, playerLocomotion and the animator.
    /// </summary>
    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();

        isInteracting = animator.GetBool("isInteracting");
        playerLocomotion.isJumping = animator.GetBool("isJumping");
        playerLocomotion.isAttacking = animator.GetBool("isAttacking");
        playerLocomotion.isHeavyAttacking = animator.GetBool("isHeavyAttacking");
        animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }
}
