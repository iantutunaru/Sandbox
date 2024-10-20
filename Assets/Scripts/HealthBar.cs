using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that is used to change the health bar displayed on the player's screen.
/// </summary>
public class HealthBar : MonoBehaviour
{
    // Slider object that is used as the health bar.
    [Tooltip("Slider object that is used as the health bar.")]
    [SerializeField]
    private Slider slider;
    // Number that is displayed on the health bar.
    [Tooltip("Number that is displayed on the health bar.")]
    [SerializeField]
    private TextMeshProUGUI healthText;

    /// <summary>
    /// Set both the displayed health and maximum value of displayed health on the health bar.
    /// </summary>
    /// <param name="health"> Health value that will be dispalyed on the health bar and maximum possible value of the health bar. </param>
    public void SetMaxHealth(int health)
    {
        health = CheckIfHealthIsCorrect(health, true);

        slider.maxValue = health;
        slider.value = health;
        healthText.text = health.ToString();
    }

    /// <summary>
    /// Set displayed health on the health bar to the parsed value.
    /// </summary>
    /// <param name="health"> Health value that will be displayed on the health bar. </param>
    public void SetHealth(int health)
    {
        health = CheckIfHealthIsCorrect(health, false);

        slider.value = health;
        healthText.text = health.ToString();
    }

    /// <summary>
    /// Method to check that the health that we are trying to set is in the correct bounds and that a correct display will be presented to the user.
    /// </summary>
    /// <param name="health"> Health that we are trying to set. </param>
    /// <returns> Health param after checks. Modified if it's bellow accepted values. </returns>
    private int CheckIfHealthIsCorrect(int health, bool maxHealth)
    {
        // If parsed health is less than 1 and we are trying to set max health then set health to 1 and print warning.
        // Otherwise, if we are setting health and it goes below 0 then set dispalyed health to 0 as death occurs below 0 HP.
        if (health < 1 && maxHealth)
        {
            Debug.LogWarning("Attempted to set the max health to less than 1");
            health = -1;
        } else if (health < 0 && !maxHealth)
        {
            health = 0;
        }

        return health;
    }
}
