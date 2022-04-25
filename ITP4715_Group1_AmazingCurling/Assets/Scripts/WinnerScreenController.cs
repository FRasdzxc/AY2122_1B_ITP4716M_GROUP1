using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinnerScreenController : MonoBehaviour
{
    [SerializeField] private Text winner;
    [SerializeField] private Text score;

    public void SetWinner(string w, Color32 c)
    {
        winner.text = w;
        winner.color = c;
    }

    public void SetScore(int s)
    {
        score.text = s.ToString();
    }
}
