using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decelerate : MonoBehaviour
{
    private Rigidbody rB;

    private void OnCollisionEnter(Collision collision)
    {
        rB = collision.gameObject.GetComponent<Rigidbody>();
        Debug.Log(rB.velocity);
        rB.AddForce(rB.gameObject.transform.TransformDirection(rB.velocity) * 0.25f, ForceMode.Impulse); // replace 0.25f with any float smaller than 1
    }
}
