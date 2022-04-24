using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassNumber : MonoBehaviour
{
    public void passRound(int num)
    {
        PlayerPrefs.SetInt("round", num);
    }
}
