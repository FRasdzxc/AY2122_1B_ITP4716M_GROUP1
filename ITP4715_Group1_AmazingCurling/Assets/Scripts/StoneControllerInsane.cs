using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoneControllerInsane: MonoBehaviour
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
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private GameObject curlingSheet;
    [SerializeField] private Text PingText;

    private bool stoneShot = false;
    private GameObject clone;
    private bool cloneActive;
    private float power;
    [SerializeField] private float maxPower = 200f; // default: 200f;
    [SerializeField] private float powerSpeed = 0.75f; // default 0.75f; how fast the power adds up (e.g. 1 = 100%, 1.5 = 150%)
    private float delay;
    private bool canDelay;
    private float turnTime;
    private float thrownTime;


    private enum Turn { red, yellow };
    private Turn turn;
    private int round;
    [SerializeField] private int maxRound = 8; // default = 8
    private int end;
    private int maxEnd;

    private LineRenderer lR;
    private Score[] score;
    private ZoomController zC;
    private Message uiM;
    private WinnerScreenController wSC;
    private ScoreboardController sbc;
    private ScoreController sc;
    private ObstacleController oC;

    public Slider powerbar;
    public AudioSource SlidingAudio;

    public enum PowerUp { HotTemp, HurryUp, LostDirections, MoreTime, SafeZone };
    private PowerUp powerUp;
    private bool powerUpActive = false;

    // Start is called before the first frame update
    void Start()
    {
        SetupController();
    }

    // Update is called once per frame
    void Update()
    {
        if (clone && cloneActive && Time.timeScale == 1) // when clone is not thrown yet
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.up * 0);
            float dist;
            Rigidbody rB = clone.GetComponent<Rigidbody>();

            if (plane.Raycast(ray, out dist)) // arrow head follows mouse position
            {
                lR.enabled = true;
                throwDir.position = ray.GetPoint(dist);
                throwDir.position = (throwDir.position - clone.transform.position).normalized * 1.5f + clone.transform.position;
            }

            if (Input.GetMouseButton(0)) // build power
            {
                //powerbar
                powerbar.value = (power / maxPower) * 100;
                currentpower.text = powerbar.value.ToString("#"); // # remove decimal
                power += Time.deltaTime * (powerSpeed * 100); // can be changed in the editor

            }
            else if (Input.GetMouseButtonUp(0)) // throw stone
            {
                stoneShot = true;
                timerSlider.SetActive(false); // pause timer
                if (power > maxPower) // upper-limit for power
                {
                    power = maxPower;
                }

                rB.AddForce((throwDir.position - clone.transform.position) * power, ForceMode.Impulse);

                cloneActive = false;
                power = 1f;
            }

            if (Input.GetMouseButton(1)) // rmb: re-position the stone
            {
                lR.enabled = false;
                clone.transform.position = ray.GetPoint(dist);

                if (Vector3.Distance(spawnPos.position, clone.transform.position) > 1.5f)
                {
                    // forces stone re-positioning to certain area, cannot go beyond the blue circle
                    clone.transform.position = (clone.transform.position - spawnPos.position).normalized * 1.5f + spawnPos.position;
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                lR.enabled = true;
                mainCamera.transform.position = clone.transform.position + new Vector3(0, 2, -2.75f); // camera focuses on the stone
                mainCamera.transform.eulerAngles = new Vector3(30f, 0f, 0f);
            }

            if (Input.GetKeyDown(KeyCode.E) && !powerUpActive)
            {
                Time.timeScale = 0;
                cardPanel.SetActive(true);
            }

            if (turnTime <= 0) // timeout
            {
                uiM.SetMessage("Turn timeout. Switching team...", new Color32(255, 255, 255, 255));
                lR.enabled = false;
                Destroy(clone);
                SwitchTurn();
            }

            lR.SetPosition(0, clone.transform.position + new Vector3(0, 0.1f, 0));
            lR.SetPosition(1, throwDir.position + new Vector3(0, 0.1f, 0));

            turnTime -= Time.deltaTime;
        }
        else // if clone is thrown
        {
            if (clone)
            {
                mainCamera.transform.position = clone.transform.position + new Vector3(0, 2, -2.75f); // camera focuses on the stone
                mainCamera.transform.eulerAngles = new Vector3(30f, 0f, 0f);

                Rigidbody rB = clone.GetComponent<Rigidbody>();

                if (stoneShot == true) // i moved the stone sound here
                {
                    clone.transform.GetComponent<AudioSource>().Play();
                    stoneShot = false;
                }
                clone.transform.GetComponent<AudioSource>().volume = (rB.velocity.magnitude * 10) / 100; // the stone sound volume corresponds to the magnitude

                if (rB.velocity.magnitude == 0) // if the clone stops after being thrown
                {
                    if (canDelay)
                        delay += Time.deltaTime; // starts delay so there is a small pause after the stone stops
                    
                    if (delay >= 0.5f)
                    {
                        // if stone does not stop at ValidArea (scoring area)
                        if (!clone.GetComponent<Collider>().bounds.Intersects(validArea.GetComponent<Collider>().bounds))
                        {
                            uiM.SetMessage("Stone will be removed: Did not enter scoring area.", new Color32(255, 255, 255, 255));
                            RemoveInvalidClone();
                        }

                        SwitchTurn();
                        canDelay = false;
                        delay = 0;

                        Destroy(clone.GetComponent<DestroyOpponentStone>());
                    }

                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.LeftArrow)) // sweep stone
                    {
                        rB.AddForce(-Vector3.right * 0.5f, ForceMode.Impulse); // sweep left
                    }
                    else if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        rB.AddForce(Vector3.right * 0.5f, ForceMode.Impulse); // sweep right
                    }

                }

                if (thrownTime > 15) // in case if stone (clone) does not stop, is stuck, or is clipping through the plane
                {
                    if (clone)
                        Destroy(clone);

                    uiM.SetMessage("Throw timeout: Stone did not stop.", new Color32(255, 255, 255, 255));
                    SwitchTurn();
                }

                thrownTime += Time.deltaTime;
            }

            lR.enabled = false;
        }
    }

    private void SpawnClone()
    {
        if (turn == Turn.red) // spawn red stone
        {
            clone = Instantiate(stone1, spawnPos.position, stone1.transform.rotation);
            clone.tag = "RedClone";
        }
        else // spawn yellow stone
        {
            clone = Instantiate(stone2, spawnPos.position, stone2.transform.rotation);
            clone.tag = "YellowClone";
        }

        timer.resetTimer();
        timerSlider.SetActive(true);
        powerbar.value = 0;
        currentpower.text = "0";
        zC = GameObject.FindGameObjectWithTag("ZoomController").GetComponent<ZoomController>();
        zC.canZoomIn = false;
        mainCamera.fieldOfView = 70;
        cloneActive = true;
        canDelay = true;
        turnTime = 20f;
        thrownTime = 0f;
        oC.showObstacle();
        lR.startWidth = 1f;
        lR.endWidth = 1f;
        curlingSheet.GetComponent<MeshCollider>().material.dynamicFriction = 0.06f;

        mainCamera.transform.position = clone.transform.position + new Vector3(0, 2, -2.75f);
        mainCamera.transform.eulerAngles = new Vector3(30f, 0f, 0f);
    }

    private void RemoveClones() // for if the player plays multiple ends
    {
        GameObject[] clones = GameObject.FindGameObjectsWithTag("RedClone"); // remove red stones
        for (int i = 0; i < clones.Length; i++)
        {
            Destroy(clones[i]);
        }

        clones = GameObject.FindGameObjectsWithTag("YellowClone"); // remove yellow stones
        for (int i = 0; i < clones.Length; i++)
        {
            Destroy(clones[i]);
        }

        clones = GameObject.FindGameObjectsWithTag("InvalidClone"); // remove any other stones
        for (int i = 0; i < clones.Length; i++)
        {
            Destroy(clones[i]);
        }
    }

    private async void RemoveInvalidClone() // make stone disappear later instead of immediately if it does not enter ValidArea (scoring area)
    {
        await Task.Delay(1450);
        Destroy(clone);
        clone = null;
    }

    public async void CloseCardPanel() // close power-up cards panel
    {
        cardPanel.SetActive(false);
        await Task.Delay(100);
        Time.timeScale = 1;
    }

    public async void SelectCard(Button card) // select card and close power-up card panel
    {
        card.interactable = false;
        cardPanel.SetActive(false);
        await Task.Delay(100);
        Time.timeScale = 1;
    }

    public void SetPowerUp(int powerIndex)
    {
        if ((PowerUp)powerIndex == PowerUp.HotTemp)
        {
            powerUp = PowerUp.HotTemp;
            powerUpActive = true;
        }
        else if ((PowerUp)powerIndex == PowerUp.HurryUp)
        {
            powerUp = PowerUp.HurryUp;
            powerUpActive = true;
        }
        else if ((PowerUp)powerIndex == PowerUp.LostDirections)
        {
            powerUp = PowerUp.LostDirections;
            powerUpActive = true;
        }
        else if ((PowerUp)powerIndex == PowerUp.MoreTime)
        {
            powerUp = PowerUp.MoreTime;
            powerUpActive = true;

            turnTime += 10f;
            timer.addTime(10f);
        }
        else if ((PowerUp)powerIndex == PowerUp.SafeZone)
        {
            powerUp = PowerUp.SafeZone;
            powerUpActive = true;

            oC.hideObstacles();
        }
    }

    private async void SwitchTurn() // this function controls end, round and turn
    {
        if (round != 0)
            await Task.Delay(1500);
        clone = null;

        oC.hideObstacles();

        if (round == maxRound && turn == Turn.yellow) // round ends
        {
            score[end - 1].SetScore();
           /* if (score[end - 1].GetScore() > 0)
            {
                if (sc.GetTeamCode() == 0)
                {
                    sbc.setScore(i + sc.GetTeamCode() + 1, score[end - 1].GetScore());
                    i++; i++;
                }
                else { sbc.setScore(i + sc.GetTeamCode(), score[end - 1].GetScore());
                    i++;i++;
                }
            }
            else if (score[end - 1].GetScore() == 0)
            {
                sbc.setScore(x, 0);
                sbc.setScore(y + end, 0);
                x++;y++;
                x++;y++;
            }*/
            await Task.Delay(1100);
            uiM.SetMessage(score[end - 1].GetTeam() + " won: " + score[end - 1].GetScore() + " score(s).", new Color32(255, 255, 255, 255));

            if (end == maxEnd) // if whole game ends
            {
                uiM.SetMessage("Game ended.", new Color32(255, 255, 255, 255));

                await Task.Delay(1100);

                int redScore = 0, yellowScore = 0;
                for (int i = 0; i < end; i++) // adding scores for winner screen
                {
                    //Debug.Log("End " + (i + 1) + ", " + score[i].GetTeam() + " won, Score " + score[i].GetScore());

                    if (score[i].GetTeam() == "Red Team")
                        redScore += score[i].GetScore();
                    else if (score[i].GetTeam() == "Yellow Team")
                        yellowScore += score[i].GetScore();
                }

                if (redScore > yellowScore) // if red team has a higher score than yellow team
                {
                    wSC.SetWinner("Red Team", new Color32(200, 0, 0, 255));
                    wSC.SetScore(redScore);
                }
                else if (redScore < yellowScore) // if yellow team has a higher score than red team
                {
                    wSC.SetWinner("Yellow Team", new Color32(200, 150, 0, 255));
                    wSC.SetScore(yellowScore);
                }
                else // if both team has the same score, or none of them scored
                {
                    wSC.SetWinner("None", new Color32(255, 255, 255, 255));
                    wSC.SetScore(0);
                }

                winnerScreen.SetActive(true);
            }
            else // if game has not ended yet, new end begins
            {
                stoneShot = false;
                await Task.Delay(1000);
                RemoveClones();
                end++;
                round = 1;
                turn = Turn.red;
                SpawnClone();
            }
        }
        else // if round has not ended yet
        {
            if (turn == Turn.red)  // if turn is red, turn becomes yellow
            {
                if (round == maxRound)
                    uiM.SetMessage("Last Round: Yellow Team's turn.", new Color32(255, 255, 150, 255));
                else
                    uiM.SetMessage("Round " + round + ": Yellow Team's turn.", new Color32(255, 255, 150, 255));
                turn = Turn.yellow;
            }
            else // if turn is yellow, turn becomes red
            {
                if (round == maxRound - 1)
                    uiM.SetMessage("Last Round: Red Team's turn.", new Color32(255, 150, 150, 255));
                else
                    uiM.SetMessage("Round " + (round + 1) + ": Red Team's turn.", new Color32(255, 150, 150, 255));
                turn = Turn.red;
                round++;
            }
            SpawnClone();
            //Debug.Log("end " + end + ", round " + round + ", turn " + turn);
        }
        timer.resetTimer();
        cc.resetColor();

        if (powerUpActive)
        {
            if (powerUp == PowerUp.HotTemp)
            {
                curlingSheet.GetComponent<MeshCollider>().material.dynamicFriction = 0.03f;

                powerUpActive = false;
            }
            else if (powerUp == PowerUp.HurryUp)
            {
                turnTime -= 10f;
                timer.addTime(-10f);

                powerUpActive = false;
            }
            else if (powerUp == PowerUp.LostDirections)
            {
                lR.startWidth = 0f;
                lR.endWidth = 0f;

                powerUpActive = false;
            }
            else if (powerUp == PowerUp.MoreTime)
            {
                powerUpActive = false;
            }
            else if (powerUp == PowerUp.SafeZone)
            {
                powerUpActive = false;
            }
        }
    }

    private void SetupController() // useful for multiple ends (chosen by player in StartMenu)
    {
        RemoveClones();

        power = 1f;
        delay = 0f;
        turnTime = 20f;
        thrownTime = 0f;
        turn = Turn.yellow; // SwitchTurn(): yellow -> red
        round = 0; // must be 0
        end = 1; // must be 1
        maxEnd = PlayerPrefs.GetInt("round"); // player preference (set from StartMenu)
        lR = spawnPos.GetComponent<LineRenderer>(); // arrow
        uiM = GameObject.FindGameObjectWithTag("UIMessage").GetComponent<Message>(); // top-right corner message
        wSC = winnerScreen.GetComponent<WinnerScreenController>(); // winner screen
        winnerScreen.SetActive(false);
        oC = GetComponent<ObstacleController>();

        score = new Score[maxEnd]; // scores for each end (e.g. 3 ends -> 3 scoreGobj)
        for (int i = 0; i < maxEnd; i++)
        {
            score[i] = Instantiate(scoreGobj).GetComponent<Score>();
        }

        SwitchTurn();
    }
}
