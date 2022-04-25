using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetMessage(string m)
    {
        this.GetComponent<Text>().text = m;
    }
}
