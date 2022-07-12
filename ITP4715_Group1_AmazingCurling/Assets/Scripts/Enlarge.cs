using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enlarge : MonoBehaviour
{
    private Rigidbody rB;

    private void OnTriggerStay(Collider collider)
    {
        rB = collider.gameObject.GetComponent<Rigidbody>();
        Vector3 maxScale = new Vector3(3, 3, 3);

        if (rB && rB.transform.localScale != maxScale)
            rB.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
    }
}
