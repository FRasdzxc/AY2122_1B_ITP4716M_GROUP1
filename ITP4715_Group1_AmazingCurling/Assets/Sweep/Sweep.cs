using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sweep : MonoBehaviour
{
    [SerializeField] private GameObject SweepE;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SweepE.SetActive(true);
            SweepE.transform.position =Vector3.MoveTowards(SweepE.transform.position, B.transform.position, 0.01f);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            SweepE.SetActive(false);
        }
    }
}
