using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinematicCamera cinematicCamera;
    [SerializeField] private CinematicCamera2 cinematicCamera2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
            cinematicCamera.activateCamera();

        if (Input.GetKey(KeyCode.Alpha2))
            cinematicCamera2.activateCamera();

        if (Input.GetKey(KeyCode.Alpha0))
        {
            cinematicCamera.deactivateCamera();
            cinematicCamera2.deactivateCamera();
        }
    }
}
