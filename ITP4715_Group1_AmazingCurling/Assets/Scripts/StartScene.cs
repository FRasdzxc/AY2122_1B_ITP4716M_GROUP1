using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public Text username;
    public GameObject start;
    void Update()
    {
        if(username.text.Length >= 3)
        {
            start.SetActive(true);
        }
    }
}
