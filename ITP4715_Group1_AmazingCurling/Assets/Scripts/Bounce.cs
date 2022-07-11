using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    private Rigidbody rB;
    [SerializeField] private float multiplier = 15f; // default: 15f;

    private void OnTriggerStay(Collider collider)
    {
        rB = collider.gameObject.GetComponent<Rigidbody>();

        if (rB)
            rB.AddForce(transform.up * multiplier, ForceMode.Impulse);
    }
}
