using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    private bool status = true;
    public GameObject obj;
    public Material mat;
    Timer timer;
    public float duration = 10.0F;


    // Update is called once per frame
    void Update()
    {
        if (status == true)
        {
            var lerp = Mathf.PingPong(Time.time, duration) / duration;
            mat.SetColor("_Color", Color.Lerp(Color.green, Color.red, lerp));
        }

    }
    public void resetColor()
    {
        var lerp = Mathf.PingPong(Time.time, duration) / duration;
        mat.SetColor("_Color", Color.Lerp(Color.green, Color.red, lerp));
        status = true;
    }
    public void pauseColor()
    {
        status = false;
    }

}
