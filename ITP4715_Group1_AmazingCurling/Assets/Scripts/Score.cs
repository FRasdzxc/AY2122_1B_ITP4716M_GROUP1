using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Score : MonoBehaviour
{
    private string team;
    private int score;
    //private int end;
    //private bool canUpdate;

    private ScoreController sC;

    public async void SetScore()
    {
        sC = GameObject.FindGameObjectWithTag("ScoreController").GetComponent<ScoreController>();
        sC.CountScore();
        await Task.Delay(1000);
        team = sC.GetTeam();
        score = sC.GetScore();
    }

    public string GetTeam()
    {
        return team;
    }

    public int GetScore()
    {
        return score;
    }
}
