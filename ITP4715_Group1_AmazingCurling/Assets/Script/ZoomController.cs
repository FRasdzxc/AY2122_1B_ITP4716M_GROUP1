using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    public bool canZoomIn;

    // Start is called before the first frame update
    void Start()
    {
        canZoomIn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canZoomIn)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 60, 0.01f);

            if (mainCamera.fieldOfView <= 60.1f)
                canZoomIn = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        canZoomIn = true;
    }
}
