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
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        round = PlayerPrefs.GetInt("round");
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
    public void setScore(Text name, int score)
    {
        text.GetComponent<Text>().text = "" + score;
    }
}
