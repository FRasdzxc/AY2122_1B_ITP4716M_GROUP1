using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    private List<GameObject> stones;
    private CapsuleCollider col;
    private bool canExpand;
    private float time;
    private string winTag;
    private string winTeam;
    private int winScore;
    private bool canAddScore;
    private int teamCode;

    // Start is called before the first frame update
    void Start()
    {
        SetupController();
    }

    // Update is called once per frame
    void Update()
    {
        if (canExpand)
        {
            time += Time.deltaTime;
            col.radius = Mathf.Lerp(col.radius, 7, time);

            if (col.radius >= 7 && !canAddScore)
            {
                canAddScore = true;

                if (stones.Count > 0)
                {
                    winTag = stones[0].tag;

                    for (int i = 1; i < stones.Count; i++)
                    {
                        if (stones[i].tag == winTag)
                            winScore++;
                        else
                            break;
                    }

                    if (winTag == "RedClone")
                    {
                        winTeam = "Red Team";
                        teamCode = 0;
                    }

                    else
                    {
                        winTeam = "Yellow Team";
                        teamCode = 1;
                    }


                    //Debug.Log(winTeam + " wins! Score: " + winScore);
                }
                else
                {
                    winTeam = "No Team";
                    winScore = 0;
                    //Debug.Log("no one scored :c");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RedClone" || other.gameObject.tag == "YellowClone")
            stones.Add(other.gameObject);
    }

    public void SetupController() // will be changed to private
    {
        stones = new List<GameObject>();
        col = gameObject.GetComponent<CapsuleCollider>();
        col.radius = 0f;
        col.enabled = false;
        canExpand = false;
        time = 0f;
        winTag = null;
        winScore = 1;
        canAddScore = false;
    }

    public void CountScore()
    {
        SetupController();
        col.enabled = true;
        canExpand = true;
    }

    public int GetTeamCode()
    {
        return teamCode;
    }

    public string GetTeam()
    {
        return winTeam;
    }

    public int GetScore()
    {
        return winScore;
    }
}
