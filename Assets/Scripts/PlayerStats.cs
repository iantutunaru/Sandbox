using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private float health = 1000;
    private float startHealth = 1000;
    private string head = "head";
    private string arm = "arm";
    private string leg = "leg";
    [SerializeField]    
    private float headDamage = 20;
    [SerializeField]
    private float bodyDamage = 10;
    [SerializeField]
    private float limbDamage = 5;
    [SerializeField]
    PlayerManager playerManager;
    [SerializeField]
    NetworkPlayer player;

    [SerializeField]
    private float timer = 0;
    [SerializeField]
    private int waitingTime = 100;

    private bool isDead = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0 && !isDead)
        {
            Death();
        }

        if (isDead)
        {
            timer += Time.deltaTime;

            if (timer > waitingTime)
            {
                Respawn();
                timer = 0;
            }
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
        isDead = true;
        playerManager.isDead = true;
        player.Death();
    }

    private void Respawn()
    {
        isDead = false;
        playerManager.isDead = false;
        health = startHealth;
        player.Respawn();
    }
}
