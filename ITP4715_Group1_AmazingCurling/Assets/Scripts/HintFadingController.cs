using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintFadingController : MonoBehaviour
{
    private Image image;
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        image = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        color = new Color(0.58f, 1, 0.58f);
        color.a = Mathf.Cos(Time.time * 7.5f) / 2;
        image.color = color;
    }
}
