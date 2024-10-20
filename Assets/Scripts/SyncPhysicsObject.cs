using UnityEngine;

/// <summary>
/// Class to sync the rotation of the physical joints during animation.
/// This creates the active ragdoll effect.
/// </summary>
public class SyncPhysicsObject : MonoBehaviour
{
    // Rigidbody of the body part from which the target location is taken.
    [Tooltip("Rigidbody of the body part from which the target location is taken.")]
    [SerializeField]
    private Rigidbody animatedRigidbody3D;
    // Configurable joint whose rotation will be updated to match the rotation of the animated body part.
    [Tooltip("Configurable joint whose rotation will be updated to match the rotation of the animated body part.")]
    [SerializeField]
    private ConfigurableJoint joint;
    // Boolean to activate rotation sync from the animation.
    [Tooltip("Boolean to activate rotation sync from the animation.")]
    [SerializeField]
    private bool syncAnimation = false;
    // Starting rotation of the body part.
    private Quaternion startLocalRotation;

    /// <summary>
    /// We save the starting rotation of the body part on the start.
    /// </summary>
    private void Awake()
    {
        startLocalRotation = transform.localRotation;
    }

    /// <summary>
    /// Method that sets the target rotation of the Configurable joint to match the current rotation of the Rigidbody.
    /// </summary>
    public void UpdateJointFromAnimation()
    {
        // If sync is disabled then exit.
        if (!syncAnimation)
            return;

        ConfigurableJointExtensions.SetTargetRotationLocal(joint, animatedRigidbody3D.transform.localRotation, startLocalRotation);
    }
}
