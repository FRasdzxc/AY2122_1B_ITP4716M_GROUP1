using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinematicCamera : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject topDownCamera;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        cam.fieldOfView -= Time.deltaTime * 3;
        transform.Rotate(new Vector3(0, 0, Time.deltaTime * 3));
    }

    private void resetCamera()
    {
        transform.position = new Vector3(0, 50, 0);
        transform.eulerAngles = new Vector3(90, 0, 0);
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
