using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    [SerializeField]
    Collider thisCollider;

    [SerializeField]
    Collider[] collidersToIgnore;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Collider otherCollider in collidersToIgnore) 
        {
            Physics.IgnoreCollision(thisCollider, otherCollider, true);
        }
    }
}
