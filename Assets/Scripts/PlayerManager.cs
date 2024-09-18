using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    Animator animator;
    InputManager inputManager;
    CameraManager cameraManager;
    NetworkPlayer playerLocomotion;

    public bool isInteracting;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        playerLocomotion = GetComponent<NetworkPlayer>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }

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
