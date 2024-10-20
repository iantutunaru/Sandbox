using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class used to handle movement and action inputs of the player.
/// </summary>
public class InputManager : MonoBehaviour
{
    [Header("Movement Inputs")]
    // Movement input used to move the player.
    [Tooltip("Movement input used to move the player.")]
    [SerializeField]
    private Vector2 movementInput;
    // Movement input on the Y axis.
    [Tooltip("Movement input on the Y axis.")]
    [SerializeField]
    public float verticalInput;
    // Movemement input on the X axis."
    [Tooltip("Movemement input on the X axis.")]
    [SerializeField]
    public float horizontalInput;
    // The total speed at which the player is moving.
    [Tooltip("The total speed at which the player is moving.")]
    [SerializeField]
    public float moveAmount;

    [Header("Camera Inputs")]
    // Rotation input used to rotate the camera around the player.
    [Tooltip("Rotation input used to rotate the camera around the player.")]
    [SerializeField]
    private Vector2 cameraInput;
    // Rotation input on the X axis.
    [Tooltip("Rotation input on the X axis.")]
    [SerializeField]
    public float cameraInputX;
    // Rotation input on the Y axis.
    [Tooltip("Rotation input on the Y axis.")]
    [SerializeField]
    public float cameraInputY;

    [Header("Action Inputs")]
    // Boolean representation of the sprint action.
    [Tooltip("Boolean representation of the sprint action.")]
    [SerializeField]
    private bool sprint_Input = false;
    // Boolean representation of the jump action.
    [Tooltip("Boolean representation of the jump action.")]
    [SerializeField]
    private bool jump_Input = false;
    // Boolean representation of the attack action.
    [Tooltip("Boolean representation of the attack action.")]
    [SerializeField]
    private bool attack_Input = false;
    // Boolean representation of the heavy attack action.
    [Tooltip("Boolean representation of the heavy attack action.")]
    [SerializeField]
    private bool heavyAttack_input = false;

    [Header("References")]
    // Reference to the Player Locomotion script of the player.
    [Tooltip("Reference to the Player Locomotion script of the player.")]
    [SerializeField]
    private PlayerLocomotion playerLocomotion;
    // Reference to the animator manager of the player.
    [Tooltip("Reference to the animator manager of the player.")]
    [SerializeField]
    private AnimatorManager animatorManager;

    /// <summary>
    /// Update the movement input when the OnMove action is triggered.
    /// </summary>
    /// <param name="context"> Context on what triggred the action. </param>
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Update the rotation input when the Look action is triggered.
    /// </summary>
    /// <param name="context"> Context on what triggred the action. </param>
    public void Look(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Set jump boolean to true if jump is triggered.
    /// </summary>
    /// <param name="context"> Context on what triggred the action. </param>
    public void Jump (InputAction.CallbackContext context)
    {
        jump_Input = true;
    }

    /// <summary>
    /// Set sprinting boolean to true while the sprint button is held pressed.
    /// </summary>
    /// <param name="context"> Context on what triggred the action. </param>
    public void Sprint(InputAction.CallbackContext context)
    {
        // If sprint button is held down then set sprinting boolean to true, otherwise set it to false.
        if (context.performed)
        {
            sprint_Input = true;
        } else if (context.canceled)
        {
            sprint_Input = false;
        }
    }

    /// <summary>
    /// Set attack boolean to true if attack is triggered.
    /// </summary>
    /// <param name="context"> Context on what triggred the action. </param>
    public void Attack(InputAction.CallbackContext context)
    {
        attack_Input = true;
    }

    /// <summary>
    /// Set heavy attack boolean to true if heavy attack is triggered.
    /// </summary>
    /// <param name="context"> Context on what triggred the action. </param>
    public void HeavyAttack(InputAction.CallbackContext context)
    {
        heavyAttack_input = true;
    }

    /// <summary>
    /// Calls to handle all types of inputs.
    /// </summary>
    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpingInput();
        HandleAttackingInput();
        HandleHeavyAttackingInput();
    }

    /// <summary>
    /// Handle movement input and camera input. Smooth the movement using the Clamp01 function.
    /// Update the animator values based on the amount moved.
    /// </summary>
    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputY = cameraInput.y;
        cameraInputX = cameraInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSprinting);
    }

    /// <summary>
    /// If sprint is started then set the sprinting boolean to true as long as the player is touching the ground and is already moving at the minimum speed.
    /// </summary>
    private void HandleSprintingInput()
    {
        // If player started a sprint, and they are already moving at a minimum speed and are touching the ground then playe the sprinting animation.
        if (sprint_Input && moveAmount > 0.5f && playerLocomotion.isGrounded)
        {
            playerLocomotion.isSprinting = true;
        } 
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }

    /// <summary>
    /// If jump is triggered then reset the boolean and play the jump animation.
    /// </summary>
    private void HandleJumpingInput()
    {
        // If player triggered a jump, then reset the boolean and play the jump animation.
        if (jump_Input)
        {
            jump_Input = false;
            playerLocomotion.HandleJumping();
        }
    }

    /// <summary>
    /// If attack is triggered then reset the boolean and play the attack animation.
    /// </summary>
    private void HandleAttackingInput()
    {
        // If player triggered an attack, then reset the boolean and play the attack animation.
        if (attack_Input)
        {
            attack_Input = false;
            playerLocomotion.HandleAttacking();
        }
    }

    /// <summary>
    /// If heavy attack is triggered then reset the boolean and play the heavy attack animation.
    /// </summary>
    private void HandleHeavyAttackingInput()
    {
        // If player triggered a heavy attack, then reset the boolean and play the heavy attack animation.
        if (heavyAttack_input)
        {
            heavyAttack_input = false;
            playerLocomotion.HandleHeavyAttacking();
        }
    }
}
