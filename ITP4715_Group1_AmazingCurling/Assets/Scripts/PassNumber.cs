using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassNumber : MonoBehaviour
{

    public float ran;
    private void Start()
    {
        PlayerPrefs.SetInt("venue", 0);
    }
    
    public void passRound(int num)
    {
        PlayerPrefs.SetInt("round", num);
    }
    public void randomRound()
    {
        ran = Random.Range(1, 10);
        PlayerPrefs.SetFloat("round", ran);
    }
    
}
