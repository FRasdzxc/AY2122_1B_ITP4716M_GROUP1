using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardController : MonoBehaviour
{
    [SerializeField] private GameObject sb1;
    [SerializeField] private GameObject sb3;
    [SerializeField] private GameObject sb5;
    [SerializeField] private Text p1r1;
    [SerializeField] private Text p2r1;
    [SerializeField] private Text p1r2;
    [SerializeField] private Text p2r2;
    [SerializeField] private Text p1r3;
    [SerializeField] private Text p2r3;
    [SerializeField] private Text p1r4;
    [SerializeField] private Text p2r4;
    [SerializeField] private Text p1r5;
    [SerializeField] private Text p2r5;
    private int round;
    Text[] text = new Text[10];
    // Start is called before the first frame update
    void Start()
    {
        round = PlayerPrefs.GetInt("round");
        text[0] = p1r1;
        text[1] = p2r1;
        text[2] = p1r2;
        text[3] = p2r2;
        text[4] = p1r3;
        text[5] = p2r3;
        text[6] = p1r4;
        text[7] = p2r4;
        text[8] = p1r5;
        text[9] = p2r5;
    }

    // Update is called once per frame
    void Update()
    {
        if(round == 1)
        {
            sb1.SetActive(true);
        }else if(round == 3)
        {
            sb3.SetActive(true);
        }
        else
        {
            sb5.SetActive(true);
        }

    }
    public void setScore(int team, int score)
    {
        text[team].text = "" + score;
    }
}
