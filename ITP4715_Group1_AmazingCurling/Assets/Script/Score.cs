using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Score : MonoBehaviour
{
    private string team;
    private int score;
    private int end;
    private bool canUpdate;

    private ScoreController sC;

    public async void SetScore(int e)
    {
        sC = GameObject.FindGameObjectWithTag("ScoreController").GetComponent<ScoreController>();
        sC.CountScore();
        await Task.Delay(1000);
        end = e;
        team = sC.GetTeam();
        score = sC.GetScore();
        Debug.Log("From Score: end " + end + ", " + team + " won, score " + score);
    }

    public void GetScore(int e)
    {
        Debug.Log("Game ended: From Score: end " + end + ", " + team + " won, score " + score);
    }
}
