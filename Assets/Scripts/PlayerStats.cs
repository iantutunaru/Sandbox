using UnityEngine;

/// <summary>
/// Class that keeps track of all player's stats and provides the following functionality:
///     * Keeps track of player's current health.
///     * Starts and resets the timer in case of player death or respawn.
///     * Applies damage to the player's health depending on the damaged body part.
///     * Applies death state to the player and provides respawn functionality.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("Health Variables")]
    // Current health of the player.
    [Tooltip("Current health of the player.")]
    [SerializeField]
    private float health = 1000;
    // Maximum health of the player.
    [Tooltip("Maximum health of the player.")]
    [SerializeField]
    private float maxHealth = 1000;

    [Header("Body Parts")]
    // String tag for the head body part.
    [Tooltip("String tag for the head body part.")]
    [SerializeField]
    private string head = "head";
    // String tag for the arm body part.
    [Tooltip("String tag for the arm body part.")]
    [SerializeField]
    private string arm = "arm";
    // String tag for the leg body part.
    [Tooltip("String tag for the leg body part.")]
    [SerializeField]
    private string leg = "leg";

    [Header("Damage Variables")]
    // Amount of extra damage that player receives if their head is damaged by an attack.
    [Tooltip("Amount of extra damage that player receives if their head is damaged by an attack.")]
    [SerializeField]    
    private float headDamage = 20;
    // Amount of extra damage that player receives if their body is damaged by an attack.
    [Tooltip("Amount of extra damage that player receives if their body is damaged by an attack.")]
    [SerializeField]
    private float bodyDamage = 10;
    // Amount of extra damage that player receives if their limb is damaged by an attack.
    [Tooltip("Amount of extra damage that player receives if their limb is damaged by an attack.")]
    [SerializeField]
    private float limbDamage = 5;

    [Header("Death Variables")]
    // Current amount of time that passed since player's death.
    [Tooltip("Current amount of time that passed since player's death.")]
    [SerializeField]
    private float timer = 0;
    // Amount of time player needs to wait after death before respawning.
    [Tooltip("Amount of time player needs to wait after death before respawning.")]
    [SerializeField]
    private int waitingTime = 100;
    // Boolean that indicates if the player is currently considered dead or not.
    [Tooltip("Boolean that indicates if the player is currently considered dead or not.")]
    [SerializeField]
    private bool isDead = false;

    [Header("References")]
    // Reference to the Player Manager script of the player.
    [Tooltip("Reference to the Player Manager script of the player.")]
    [SerializeField]
    private PlayerManager playerManager;
    // Reference to the Player Locomotion of the player.
    [Tooltip("Reference to the Player Locomotion of the player.")]
    [SerializeField]
    private PlayerLocomotion player;
    // Reference to the Health Bar of the player.
    [Tooltip("Reference to the Health Bar of the player.")]
    [SerializeField]
    private HealthBar healthBar;


    /// <summary>
    /// On creation of the player, set it's healthbar maximum health to player's max health.
    /// </summary>
    void Start()
    {
        healthBar.SetMaxHealth((int)maxHealth);
    }

    /// <summary>
    /// Check if player is dead, and make them dead if they are not.
    /// Also, run a timer to respawn the player after a set amount of time.
    /// </summary>
    void Update()
    {
        // If player's health is below 0 and they are not dead, then make them dead.
        if (health <= 0 && !isDead)
        {
            Death();
        }

        // Check if the player is dead and start the death timer.
        if (isDead)
        {
            timer += Time.deltaTime;

            // After timer finishes respawn the player and reset the timer.
            if (timer > waitingTime)
            {
                Respawn();
                timer = 0;
            }
        }
    }

    /// <summary>
    /// Method that applies damage to the player based on the damaged body part.
    /// </summary>
    /// <param name="damage"> Initial amount of damage. </param>
    /// <param name="damagedBodyPart"> String name of the damaged body part. </param>
    public void Damage(float damage, string damagedBodyPart)
    {
        // Check the name of the damaged body part and adjust health.
        if (damagedBodyPart == head)
        {
            health -= damage + headDamage;
            healthBar.SetHealth((int)health);
        } else if (damagedBodyPart == arm || damagedBodyPart == leg) 
        {
            health -= damage + limbDamage;
            healthBar.SetHealth((int)health);
        } else
        {
            health -= damage + bodyDamage;
            healthBar.SetHealth((int)health);
        }
    }

    /// <summary>
    /// Method that is called when the player dies.
    /// Sets the isDead booleans of the player and it's manager to true.
    /// Calls the Death method of the Player Locomotion.
    /// </summary>
    private void Death()
    {
        isDead = true;
        playerManager.isDead = true;

        player.Death();
    }

    /// <summary>
    /// Method that is called when the player is respawned.
    /// Sets the isDead booleans of the player and it's player manager to false.
    /// Resets health to max and resets the healthbar.
    /// Calls the Respawn method of the Player Locomotion.
    /// </summary>
    private void Respawn()
    {
        isDead = false;
        playerManager.isDead = false;
        health = maxHealth;

        healthBar.SetHealth((int)health);
        player.Respawn();
    }
}
