using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerate : MonoBehaviour
{
    private Rigidbody rB;
    [SerializeField] private float multiplier = 5f; // default: 5f;

    private void OnTriggerStay(Collider collider)
    {
        rB = collider.gameObject.GetComponent<Rigidbody>();

        if (rB)
            rB.AddForce(transform.forward * multiplier, ForceMode.Impulse);
    }
}
