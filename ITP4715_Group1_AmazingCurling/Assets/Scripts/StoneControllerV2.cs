using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoneControllerV2 : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject stone1;
    [SerializeField] private GameObject stone2;
    [SerializeField] private GameObject scoreGobj;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private Transform throwDir;
    [SerializeField] private Transform validArea;
    [SerializeField] private TMP_Text currentpower;
    [SerializeField] private Timer timer;
    [SerializeField] private ColorChange cc;
    [SerializeField] private GameObject timerSlider;
    [SerializeField] private GameObject winnerScreen;
    [SerializeField] private Text PingText;

    private bool stoneShot = false;
    private GameObject clone;
    private bool cloneActive;
    private float power;
    private float delay;
    private float turnTime;
    private float thrownTime;

    private enum Turn { red, yellow };
    private Turn turn;
    private int round;
    private int maxRound;
    private int end;
    private int maxEnd;

    private LineRenderer lR;
    private Score[] score;
    private ZoomController zC;
    private Message uiM;
    private WinnerScreenController wSC;

    public Slider powerbar;
    public AudioSource SlidingAudio;
    public AudioSource SlidingAudio1;



    // Start is called before the first frame update
    void Start()
    {
        SetupController();
       
    }

    // Update is called once per frame
    void Update()
    {
       
        if (cloneActive)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.up * 0);
            float dist;
            Rigidbody rB = clone.GetComponent<Rigidbody>();

            if (plane.Raycast(ray, out dist))
            {
                lR.enabled = true;
                throwDir.position = ray.GetPoint(dist);
                throwDir.position = (throwDir.position - clone.transform.position).normalized * 1.5f + clone.transform.position;
            }

            if (Input.GetMouseButton(0))
            {
                //powerbar
                powerbar.value = power / 2;
                currentpower.text = powerbar.value.ToString("#"); // # remove decimal
                power += Time.deltaTime * 75;

            }
            else if (Input.GetMouseButtonUp(0))
            {
                stoneShot = true;
                timerSlider.SetActive(false);
                if (power > 200)
                {
                    power = 200;
                }


                rB.AddForce((throwDir.position - clone.transform.position) * power, ForceMode.Impulse);


                cloneActive = false;
                power = 1f;
            }

            if (Input.GetMouseButton(1))
            {
                lR.enabled = false;
                clone.transform.position = ray.GetPoint(dist);

                if (Vector3.Distance(spawnPos.position, clone.transform.position) > 1.5f)
                {
                    clone.transform.position = (clone.transform.position - spawnPos.position).normalized * 1.5f + spawnPos.position;
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                lR.enabled = true;
            }

            if (turnTime <= 0)
            {
                uiM.SetMessage("Turn timeout. Switching team...");
                Destroy(clone);
                SwitchTurn();
            }

            lR.SetPosition(0, clone.transform.position + new Vector3(0, 0.1f, 0));
            lR.SetPosition(1, throwDir.position + new Vector3(0, 0.1f, 0));

            turnTime -= Time.deltaTime;

            if (stoneShot == true)
            {
                if(stone1)
                {
                    SlidingAudio.Play();
                    Debug.Log(SlidingAudio);
                }
                else
                {
                    SlidingAudio1.Play();
                    Debug.Log(SlidingAudio1);
                } 
            }
            else
            {
                SlidingAudio.Stop();
                SlidingAudio1.Stop();
            }

        }
        else
        {
            if (clone)
            {
                mainCamera.transform.position = clone.transform.position + new Vector3(0, 2, -2.75f);
                mainCamera.transform.eulerAngles = new Vector3(30f, 0f, 0f);

                Rigidbody rB = clone.GetComponent<Rigidbody>();

                if (rB.velocity.magnitude == 0)
                {
                    stoneShot = false;
                    delay += Time.deltaTime;

                    if (delay >= 0.5f)
                    {
                        if (!clone.GetComponent<Collider>().bounds.Intersects(validArea.GetComponent<Collider>().bounds))
                        {
                            uiM.SetMessage("Stone removed: Invalid.");
                            Destroy(clone);
                        }

                        clone = null;
                        SwitchTurn();
                        delay = 0f;
                    }

                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        rB.AddForce(-Vector3.right, ForceMode.Impulse);
                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        rB.AddForce(Vector3.right, ForceMode.Impulse);
                    }

                }

                if (thrownTime > 10) // in case if stone (clone) is stuck or clips through the plane
                {
                    if (clone)
                        Destroy(clone);

                    uiM.SetMessage("Throw timeout. Switching team...");
                    SwitchTurn();
                }

                thrownTime += Time.deltaTime;
            }

            lR.enabled = false;
        }

        PingText.text = "Ping: " + PhotonNetwork.GetPing();
    }

    private void SpawnClone()
    {
        if (turn == Turn.red)
        {
            clone = Instantiate(stone1, spawnPos.position, spawnPos.rotation);
            clone.tag = "RedClone";
            uiM.SetMessage("Red Team's turn.");
        }
        else
        {
            clone = Instantiate(stone2, spawnPos.position, spawnPos.rotation);
            clone.tag = "YellowClone";
            uiM.SetMessage("Yellow Team's turn.");
        }

        timerSlider.SetActive(true);
        powerbar.value = 0;
        currentpower.text = "0";
        zC = GameObject.FindGameObjectWithTag("ZoomController").GetComponent<ZoomController>();
        zC.canZoomIn = false;
        mainCamera.fieldOfView = 70;
        cloneActive = true;
        turnTime = 20f;
        thrownTime = 0f;

        mainCamera.transform.position = clone.transform.position + new Vector3(0, 2, -2.75f);
        mainCamera.transform.eulerAngles = new Vector3(30f, 0f, 0f);
    }

    private void RemoveClones()
    {
        GameObject[] clones = GameObject.FindGameObjectsWithTag("RedClone");
        for (int i = 0; i < clones.Length; i++)
        {
            Destroy(clones[i]);
        }

        clones = GameObject.FindGameObjectsWithTag("YellowClone");
        for (int i = 0; i < clones.Length; i++)
        {
            Destroy(clones[i]);
        }

        clones = GameObject.FindGameObjectsWithTag("InvalidClone");
        for (int i = 0; i < clones.Length; i++)
        {
            Destroy(clones[i]);
        }
    }

    private async void SwitchTurn()
    {
        await Task.Delay(1500);

        if (round == maxRound && turn == Turn.yellow) // round ends
        {
            score[end - 1].SetScore();
            await Task.Delay(1100);
            uiM.SetMessage(score[end - 1].GetTeam() + " won: " + score[end - 1].GetScore() + " score(s).");

            if (end == maxEnd)
            {
                uiM.SetMessage("Game ended.");

                await Task.Delay(1100);

                int redScore = 0, yellowScore = 0;
                for (int i = 0; i < end; i++)
                {
                    Debug.Log("End " + (i + 1) + ", " + score[i].GetTeam() + " won, Score " + score[i].GetScore());

                    if (score[i].GetTeam() == "Red Team")
                        redScore += score[i].GetScore();
                    else if (score[i].GetTeam() == "Yellow Team")
                        yellowScore += score[i].GetScore();
                }

                if (redScore > yellowScore)
                {
                    wSC.SetWinner("Red Team", new Color32(200, 0, 0, 255));
                    wSC.SetScore(redScore);
                }
                else if (redScore < yellowScore)
                {
                    wSC.SetWinner("Yellow Team", new Color32(200, 150, 0, 255));
                    wSC.SetScore(yellowScore);
                }
                else
                {
                    wSC.SetWinner("None", new Color32(255, 255, 255, 255));
                    wSC.SetScore(0);
                }

                winnerScreen.SetActive(true);
            }
            else
            {
                await Task.Delay(1000);
                RemoveClones();
                end++;
                round = 1;
                turn = Turn.red;
                SpawnClone();
            }
        }
        else
        {
            if (turn == Turn.red)
            {
                turn = Turn.yellow;
            }
            else
            {
                turn = Turn.red;
                round++;
            }
            SpawnClone();
            Debug.Log("end " + end + ", round " + round + ", turn " + turn); // remove this after gui has added
            // gui tracking end, round and turn
        }
        timer.resetTimer();
        cc.resetColor();
    }

    private void SetupController()
    {
        RemoveClones();

        power = 1f;
        delay = 0f;
        turnTime = 20f;
        thrownTime = 0f;
        turn = Turn.red;
        round = 1; // must be 1
        maxRound = 8; // default: 8
        end = 1; // must be 1
        maxEnd = PlayerPrefs.GetInt("round"); // player preference (set from StartMenu)
        lR = spawnPos.GetComponent<LineRenderer>();
        uiM = GameObject.FindGameObjectWithTag("UIMessage").GetComponent<Message>();
        wSC = winnerScreen.GetComponent<WinnerScreenController>();
        winnerScreen.SetActive(false);

        score = new Score[maxEnd];
        for (int i = 0; i < maxEnd; i++)
        {
            score[i] = Instantiate(scoreGobj).GetComponent<Score>();
        }

        SpawnClone();
    }
}
