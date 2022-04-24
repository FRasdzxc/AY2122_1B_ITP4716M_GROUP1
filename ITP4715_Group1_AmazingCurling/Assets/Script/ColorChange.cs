using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    private bool status = true;
    public GameObject obj;
    public Material mat;
    public float duration = 10.0F;
    public float time = 0;


    // Update is called once per frame
    void Update()
    {
        if (status == true)
        {
            time += Time.deltaTime / duration;
            Debug.Log(time);
            mat.SetColor("_Color", Color.Lerp(Color.green, Color.red, time));
        }

    }
    public void resetColor()
    {
        time = 0;
        mat.SetColor("_Color", Color.green);
        status = true;
    }
    public void pauseColor()
    {
        status = false;
    }

}
