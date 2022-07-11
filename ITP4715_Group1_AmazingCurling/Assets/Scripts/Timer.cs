using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private bool status = true;
    public float myTimer = 20f;
    public Slider slider;
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        slider.maxValue = myTimer;
        slider.minValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (status == true)
        {
            if (myTimer > 0)
                myTimer -= Time.deltaTime;
            slider.value = myTimer;
            text.text = Mathf.Ceil(myTimer).ToString("00");
        }
    }
    public void resetTimer()
    {
        myTimer = 20f;
        status = true;
    }
    public void pauseTimer()
    {
        status = false;
    }

    public void addTime(float duration)
    {
        myTimer += duration;
    }
}
