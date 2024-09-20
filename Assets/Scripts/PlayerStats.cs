using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private float health = 100;
    private string head = "head";
    private string arm = "arm";
    private string leg = "leg";
    [SerializeField]    
    private float headDamage = 20;
    [SerializeField]
    private float bodyDamage = 10;
    [SerializeField]
    private float limbDamage = 5;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Death();
        }
    }

    public void Damage(float damage, string damagedBodyPart)
    {
        if (damagedBodyPart == head)
        {
            health -= damage + headDamage;
        } else if (damagedBodyPart == arm || damagedBodyPart == leg) 
        {
            health -= damage + limbDamage;
        } else
        {
            health -= damage + bodyDamage;
        }
    }

    private void Death()
    {

    }
}
