using UnityEngine;

/// <summary>
/// This class handles rotation and movement of the camera in the scene when following the player character.
/// Additionaly, it checks if camera is colliding with any obstacles during movement and corrects movement or rotation accordingly.
/// </summary>
public class CameraManager : MonoBehaviour
{
    // Input manager that parses player input to the camera manager.
    [Tooltip("Input manager that parses player input to the camera manager.")]
    [SerializeField]
    private InputManager inputManager;

    [Header("Transform variables")]
    // Target that the camera is following.
    [Tooltip("Target that the camera is following.")]
    [SerializeField]
    private Transform targetTransform;
    // Transform of the camera.
    [Tooltip("Transform of the camera.")]
    [SerializeField]
    private Transform cameraTransform;
    // The main pivot of the camera.
    [Tooltip("The main pivot of the camera.")]
    [SerializeField]
    private Transform cameraPivot;

    [Header("Collision variables")]
    // Offset to make sure camera doesn't end up in another onbject when moving.
    [Tooltip("")]
    [SerializeField]
    private float cameraCollisionOffSet = 0.2f;
    // The minimum rotation/movement distance that camera can rotate/move.
    [Tooltip("The minimum rotation/movement distance that camera can rotate/move.")]
    [SerializeField]
    private float minimumCollisionOffSet = 0.2f;
    // Radius of the raycast that camera uses to check if it touches an object during movement.
    [Tooltip("Radius of the raycast that camera uses to check if it touches an object during movement.")]
    [SerializeField]
    private float cameraCollisionRadius = 2;
    // Layers that cameral can collide with.
    [Tooltip("Layers that cameral can collide with.")]
    [SerializeField]
    private LayerMask collisionLayers;

    [Header("Speeds")]
    // Dampening speed on the camera when following the player.
    [Tooltip("Dampening speed on the camera when following the player.")]
    [SerializeField]
    private float cameraFollowSpeed = 0.2f;
    // Speed of camera rotation on the horizontal axis.
    [Tooltip("Speed of camera rotation on the horizontal axis.")]
    [SerializeField]
    private float cameraLookSpeed = 2;
    // Speed of camera rotation on vertical axis.
    [Tooltip("Speed of camera rotation on vertical axis.")]
    [SerializeField]
    private float cameraPivotSpeed = 2;

    [Header("Angles")]
    // Horizontal angle rotation of the camera.
    [Tooltip("Horizontal angle rotation of the camera.")]
    [SerializeField]
    private float lookAngle;
    // Current pivot angle of the camera.
    [Tooltip("Current pivot angle of the camera.")]
    [SerializeField]
    private float pivotAngle;
    // Minimum pivot angle of the camera.
    [Tooltip("Minimum pivot angle of the camera.")]
    [SerializeField]
    private float minimumPivotAngle = -35;
    // The maximum pivot angle of the camera.
    [Tooltip("Maximum pivot angle of the camera.")]
    [SerializeField]
    private float maximumPivotAngle = 35;

    // Float representation of the camera position on the Z axis when the camera manager is loaded.
    private float defaultPosition;
    // Camera follow velocity.
    private Vector3 cameraFollowVelocity = Vector3.zero;
    // New camera position after all calculations.
    private Vector3 cameraVectorPosition;

    /// <summary>
    /// We save the camera's default postion when the manager is loaded.
    /// </summary>
    private void Awake()
    {
        defaultPosition = cameraTransform.localPosition.z;
    }

    /// <summary>
    /// Method that is invoked when moving the camera.
    /// </summary>
    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    /// <summary>
    /// Method that allows the camera to follow the target smoothly using the SmoothDamp method of Vector 3;
    /// </summary>
    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPosition;
    }

    /// <summary>
    /// Method to rotate the camera by calculating the rotation angle on horizontal and vertical axises, and then rotating both the camera and pivot to the rotation position.
    /// </summary>
    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle += (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle -= (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    /// <summary>
    /// Method that checks if camera is going to collide with anything in the direction it's moving using raycast.
    /// If there is collision then camera is moved before the collision.
    /// </summary>
    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        // Check if camera collides with anything in it's movement path and if it does then set the target position before the collision.
        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out RaycastHit hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffSet);
        }

        // Check if new position is less than the minimum off set and if it is then set then decrease the target position by the minimum off set.
        if (Mathf.Abs(targetPosition) < minimumCollisionOffSet)
        {
            targetPosition -= minimumCollisionOffSet;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
