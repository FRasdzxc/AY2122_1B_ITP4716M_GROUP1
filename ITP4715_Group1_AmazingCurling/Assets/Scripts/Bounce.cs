using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    private Rigidbody rB;

    private void OnCollisionEnter(Collision collision)
    {
        rB = collision.gameObject.GetComponent<Rigidbody>();
        Debug.Log(rB.velocity);
        rB.AddForce(rB.gameObject.transform.TransformDirection(rB.velocity) * -25, ForceMode.Impulse); // replace -25 with any -ve number
    }
}
