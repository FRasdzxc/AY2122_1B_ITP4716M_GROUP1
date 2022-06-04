using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    public void SetMessage(string m, Color32 c)
    {
        this.GetComponent<Text>().text = m;
        this.GetComponent<Text>().color = c;
    }
}
