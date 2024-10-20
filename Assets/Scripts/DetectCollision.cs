using UnityEngine;

/// <summary>
/// In this class we handle collisions between the player's body part and body parts of the other player.
/// Damage is deal to the other player if the current player is attacking or heavy attacking.
/// </summary>
public class DetectCollision : MonoBehaviour
{
    // Name of this body part.
    [Tooltip("Name of this body part.")]
    [SerializeField]
    private string bodyPart;
    // Reference to the Player Locomotion script of the current player.
    [Tooltip("Reference to the Player Locomotion script of the current player.")]
    [SerializeField]
    private PlayerLocomotion player;
    // Damage done to the other player during normal attack.
    [Tooltip("Damage done to the other player during normal attack.")]
    [SerializeField]
    private float normalAttack = 10;
    // Damage done to the other player during heavy attack.
    [Tooltip("Damage done to the other player during heavy attack.")]
    [SerializeField]
    private float heavyAttack = 20;
    // Int representation of the Player Layer.
    [Tooltip("Int representation of the Player Layer.")]
    [SerializeField]
    private int playerLayerInt = 3;

    /// <summary>
    /// Getter method to get the name of the current body part.
    /// </summary>
    /// <returns> Name of the current body part. </returns>
    public string GetBodyPartName()
    {
        return bodyPart;
    }

    /// <summary>
    /// Every time the player collides with another collider check if it's a player, and if it's being attacked and damaged.
    /// </summary>
    /// <param name="collision"> Information passed to the collider during collision with other collider. </param>
    void OnCollisionEnter(Collision collision)
    {
        // Go through every contact point in the collision and check if the other object is a player and if it was attacked and damaged.
        foreach (ContactPoint contact in collision.contacts)
        {
            // Check if other game object in the collision belongs to a player layer.
            if (contact.otherCollider.gameObject.layer == playerLayerInt)
            {
                PlayerLocomotion otherPlayer = contact.otherCollider.gameObject.GetComponentInParent<PlayerLocomotion>();
                // Check if the player that collided with the body part is not the current player and then find what their stats and body part is.
                if (otherPlayer != player)
                {
                    PlayerStats otherPlayerStats = otherPlayer.GetComponent<PlayerStats>();
                    string otherPlayerBodyPart = contact.otherCollider.gameObject.GetComponent<DetectCollision>().GetBodyPartName();

                    // Check is player is attacking and apply normal damage to the other player if true.
                    if (player.isAttacking)
                    {
                        otherPlayerStats.Damage(normalAttack, otherPlayerBodyPart);
                    }
                    // Check is player is using a heavy attack and apply heavy damage to the other player if true.
                    else if (player.isHeavyAttacking)
                    {
                        otherPlayerStats.Damage(heavyAttack, otherPlayerBodyPart);
                    }
                }
            }
        }
    }
}
