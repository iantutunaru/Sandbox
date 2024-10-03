using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private TextMeshProUGUI healthText;

    public void SetMaxHealth(int health)
    {
        if (health < 0)
        {
            health = 0;
        }

        slider.maxValue = health;
        slider.value = health;
        healthText.text = health.ToString();
    }

    public void SetHealth(int health)
    {
        if (health < 0)
        {
            health = 0;
        }

        slider.value = health;
        healthText.text = health.ToString();
    }
}
