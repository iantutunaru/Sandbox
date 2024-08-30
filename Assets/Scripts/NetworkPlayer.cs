using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : MonoBehaviour
{
    [SerializeField]
    Rigidbody rigidbody3D;

    [SerializeField]
    ConfigurableJoint mainJoint;

    [SerializeField]
    Animator animator;

    [SerializeField]
    float rotationSpeed;

    [SerializeField]
    float runSpeed;

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
    bool isGrounded = false;
    bool isAttacking = false;

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

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        syncPhysicsObjects = GetComponentsInChildren<SyncPhysicsObject>();
        SetRagdollParts();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        lightAttack = playerControls.Player.LightAttack;
        lightAttack.Enable();
        lightAttack.performed += LightAttack;

        jump = playerControls.Player.Jump;
        jump.Enable();
        jump.performed += Jump;
    }

    private void OnDisable()
    {
        move.Disable();
        lightAttack.Disable();
        jump.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //Move input
        //moveInputVector.x = Input.GetAxis("Horizontal");
        //moveInputVector.y = Input.GetAxis("Vertical");

        moveInputVector = move.ReadValue<Vector2>();

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
        //Assume that we are not grounded
        isGrounded = false;

        //Check if we are grounded
        int numberOfHits = Physics.SphereCastNonAlloc(rigidbody3D.position, 0.1f, transform.up * -1, raycastHits, 0.5f);

        // Check for valid results
        for (int i = 0; i < numberOfHits; i++)
        {
            //Ignore self hits
            if (raycastHits[i].transform.root == transform)
                continue;

            isGrounded = true;

            animator.SetBool("Jump", false);
            animator.SetBool("Grounded", true);

            break;
        }
        /*
        if (isGrounded)
        {
            animator.SetBool("Grounded", false);
            animator.SetBool("FreeFall", false);
            animator.SetBool("Jump", false);
        }*/
        

        Debug.Log(rigidbody3D.velocity.magnitude);
        if (rigidbody3D.velocity.magnitude > maxSpeed)
        {
            rigidbody3D.velocity = Vector3.ClampMagnitude(rigidbody3D.velocity, maxSpeed);
        }

        //Apply extra gravity to character to make it less floaty
        if (!isGrounded)/*
            animator.SetBool("Grounded", false);
            animator.SetBool("Jump", false);
            animator.SetBool("FreeFall", true);*/
            rigidbody3D.AddForce(Vector3.down * 10);

        float inputMagnitued = moveInputVector.magnitude;

        Vector3 localVelocityVsForward = transform.forward * Vector3.Dot(transform.forward, rigidbody3D.velocity);

        float localForwardVelocity = localVelocityVsForward.magnitude;

        if (inputMagnitued != 0)
        {
            Quaternion desiredDirection = Quaternion.LookRotation(new Vector3(moveInputVector.x, 0, moveInputVector.y * -1), transform.up);

            // Rotate target towards direction
            mainJoint.targetRotation = Quaternion.RotateTowards(mainJoint.targetRotation, desiredDirection, Time.fixedDeltaTime * rotationSpeed);

            if (localForwardVelocity < maxSpeed)
            {
                //Move character in the direction it is facing
                rigidbody3D.AddForce(transform.forward * inputMagnitued * runSpeed);
            }
        }

        if (isGrounded && isJumpButtonPressed)
        {
            animator.SetBool("Grounded", false);
            animator.SetBool("FreeFall", false);
            animator.SetBool("Jump", true);

            rigidbody3D.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);

            isJumpButtonPressed = false;
        }

        if (isAttackButtonPressed)
        {
            animator.SetTrigger("Attacking");

            isAttackButtonPressed = false;
        }

        animator.SetFloat("Speed", localForwardVelocity * 0.4f);

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
            Debug.Log("Touched itself");
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
}
