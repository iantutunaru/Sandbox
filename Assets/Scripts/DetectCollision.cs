using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    [SerializeField]
    private string bodyPart;
    [SerializeField]
    private NetworkPlayer player;
    [SerializeField]
    private PlayerStats playerStats;
    private float normalAttack = 10;
    private float heavyAttack = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string getBodyPartName()
    {
        Debug.Log("Player body part:"+ bodyPart);
        return bodyPart;
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.Log("Contact");

            if (contact.otherCollider.gameObject.layer == 3)
            {
                NetworkPlayer otherPlayer = contact.otherCollider.gameObject.GetComponentInParent<NetworkPlayer>();

                if (otherPlayer != player)
                {
                    PlayerStats otherPlayerStats = otherPlayer.GetComponent<PlayerStats>();

                    string otherPlayerBodyPart = contact.otherCollider.gameObject.GetComponent<DetectCollision>().getBodyPartName();
                    Debug.Log("Other player body part:" + otherPlayerBodyPart);

                    if (player.isAttacking)
                    {
                        otherPlayerStats.Damage(normalAttack, otherPlayerBodyPart);
                    }
                    else if (player.isHeavyAttacking)
                    {
                        otherPlayerStats.Damage(heavyAttack, otherPlayerBodyPart);
                    }
                }
            }
        }
    }
}
