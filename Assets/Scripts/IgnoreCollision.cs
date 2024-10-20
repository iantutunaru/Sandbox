using UnityEngine;

/// <summary>
/// This class disables collision between the selected collider and array of colliders with which we don't want to collide.
/// </summary>
public class IgnoreCollision : MonoBehaviour
{
    // Current collider.
    [Tooltip("Current collider.")]
    [SerializeField]
    private Collider thisCollider;
    // Array of colliders that need to be ignored by the current collider.
    [Tooltip("Array of colliders that need to be ignored by the current collider.")]
    [SerializeField]
    Collider[] collidersToIgnore;

    void Start()
    {
        DisableCollision();
    }

    /// <summary>
    /// Method that goes through each of the colliders that needs to be ignored and disables collisions between them and the current collider.
    /// </summary>
    private void DisableCollision()
    {
        // Disable the collision between current collider and each collider in the array of colliders to ignore.
        foreach (Collider otherCollider in collidersToIgnore)
        {
            Physics.IgnoreCollision(thisCollider, otherCollider, true);
        }
    }
}
