using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassNumber : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.SetInt("venue", 0);
    }
    public float ran;
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
