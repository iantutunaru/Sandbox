using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInputActions playerControls;
    NetworkPlayer playerLocomotion;
    AnimatorManager animatorManager;

    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float cameraInputX;
    public float cameraInputY;

    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    public bool sprint_Input = false;
    public bool jump_Input = false;
    public bool attack_Input = false;
    public bool heavyAttack_input = false;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<NetworkPlayer>();
    }

    private void OnEnable()
    {
        //if (playerControls ==  null)
        //{
        //    Debug.Log("New player joined");
        //    playerControls = new PlayerInputActions();

        //    playerControls.Player.Move.performed += i => movementInput = i.ReadValue<Vector2>();
        //    playerControls.Player.Look.performed += i => cameraInput = i.ReadValue<Vector2>();

        //    playerControls.Player.Sprint.performed += i => sprint_Input = true;
        //    playerControls.Player.Sprint.canceled += i => sprint_Input = false;
        //    playerControls.Player.Jump.performed += i => jump_Input = true;
        //    playerControls.Player.Attack.performed += i => attack_Input = true;
        //    playerControls.Player.HeavyAttack.performed += i => heavyAttack_input = true;
        //}

        //playerControls.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void Look(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
    }

    public void Jump (InputAction.CallbackContext context)
    {
        jump_Input = true;
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            sprint_Input = true;
        } else if (context.canceled)
        {
            sprint_Input = false;
        }
    }

    //public void SprintStart(InputAction.CallbackContext context)
    //{
    //    sprint_Input = true;
    //}

    //public void SprintFinish(InputAction.CallbackContext context)
    //{
    //    sprint_Input = false;
    //}

    public void Attack(InputAction.CallbackContext context)
    {
        attack_Input = true;
    }

    public void HeavyAttack(InputAction.CallbackContext context)
    {
        heavyAttack_input = true;
    }

    private void OnDisable()
    {
        //playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpingInput();
        HandleAttackingInput();
        HandleHeavyAttackingInput();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputY = cameraInput.y;
        cameraInputX = cameraInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSprinting);
    }

    private void HandleSprintingInput()
    {
        if (sprint_Input && moveAmount > 0.5f)
        {
            playerLocomotion.isSprinting = true;
        } 
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }

    private void HandleJumpingInput()
    {
        if (jump_Input)
        {
            jump_Input = false;
            playerLocomotion.HandleJumping();
        }
    }

    private void HandleAttackingInput()
    {
        if (attack_Input)
        {
            attack_Input = false;
            playerLocomotion.HandleAttacking();
        }
    }

    private void HandleHeavyAttackingInput()
    {
        if (heavyAttack_input)
        {
            heavyAttack_input = false;
            playerLocomotion.HandleHeavyAttacking();
        }
    }
}
