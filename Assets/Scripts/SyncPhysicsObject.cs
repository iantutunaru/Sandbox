using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPhysicsObject : MonoBehaviour
{
    Rigidbody rigidbody3D;
    ConfigurableJoint joint;

    [SerializeField]
    Rigidbody animatedRigidbody3D;

    [SerializeField]
    bool syncAnimation = false;

    //Keep track for starting roation
    Quaternion startLocalRotation;

    private void Awake()
    {
        rigidbody3D = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();

        //Store the starting local rotation
        startLocalRotation = transform.localRotation;
    }

    public void UpdateJointFromAnimation()
    {
        if (!syncAnimation)
            return;

        ConfigurableJointExtensions.SetTargetRotationLocal(joint, animatedRigidbody3D.transform.localRotation, startLocalRotation);
    }
}
