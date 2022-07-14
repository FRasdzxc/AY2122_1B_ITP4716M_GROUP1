using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinematicCamera2 : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject topDownCamera;
    [SerializeField] private GameObject focusPoint;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(focusPoint.transform);
        transform.Translate(new Vector3(0, -Time.deltaTime * 1.5f, 0));
    }

    private void resetCamera()
    {
        transform.position = new Vector3(-10, 10, -25);
        transform.eulerAngles = new Vector3(40, 10, 0);
        cam.fieldOfView = 70;
    }

    public void activateCamera()
    {
        canvas.SetActive(false);
        topDownCamera.SetActive(false);
        gameObject.SetActive(true);
        resetCamera();
    }

    public void deactivateCamera()
    {
        canvas.SetActive(true);
        topDownCamera.SetActive(true);
        gameObject.SetActive(false);
    }
}
