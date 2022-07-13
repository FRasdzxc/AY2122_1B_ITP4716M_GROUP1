using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// made for tutorial only
public class StoneControllerTutorial : MonoBehaviour
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
    [SerializeField] private float maxPower = 200f; // default: 200f;
    [SerializeField] private float powerSpeed = 0.75f; // default 0.75f; how fast the power adds up (e.g. 1 = 100%, 1.5 = 150%)
    private float delay;
    private bool canDelay;
    private float turnTime;
    private float thrownTime;

    private enum Turn { red, yellow };
    private Turn turn;

    private LineRenderer lR;
    private ZoomController zC;

    public Slider powerbar;
    public AudioSource SlidingAudio;

    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Text tutorialBody;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private GameObject tutorialImage;
    [SerializeField] private RectTransform hint;
    [SerializeField] [TextArea] private string[] tutorialDialogues;
    private int tutorialDialogueIndex = -1;
    private bool nextStepActive = false;
    private bool canThrow = false;
    private bool canChangeDirection = false;
    private bool canPickUp = false;
    private bool canCountdown = false;
    private bool canIntersect = false;
    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject topDownCamera;
    [SerializeField] GameObject[] stoneSetups;

    // Start is called before the first frame update
    void Start()
    {
        RemoveClones();

        power = 1f;
        delay = 0f;
        turnTime = 20f;
        thrownTime = 0f;
        turn = Turn.red;
        lR = spawnPos.GetComponent<LineRenderer>(); // arrow

        timer.pauseTimer();
        Time.timeScale = 0;
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

            if (Input.GetMouseButton(0)) // build power
            {
                if (canThrow)
                {
                    //powerbar
                    powerbar.value = (power / maxPower) * 100;
                    currentpower.text = powerbar.value.ToString("#"); // # remove decimal
                    power += Time.deltaTime * (powerSpeed * 100); // can be changed in the editor

                    if (tutorialDialogueIndex == 1 && powerbar.value >= 40)
                    {
                        tutorialDialogueIndex++;
                        NextStep();
                    }
                    else if (tutorialDialogueIndex == 2 && powerbar.value >= 40)
                        NextStep();
                }
            }
            else if (Input.GetMouseButtonUp(0)) // throw stone
            {
                if (canThrow)
                {
                    if (power > maxPower) // upper-limit for power
                    {
                        power = maxPower;
                    }

                    if (canChangeDirection)
                    {
                        stoneShot = true;
                        timerSlider.SetActive(false); // pause timer
                        rB.AddForce((throwDir.position - clone.transform.position) * power, ForceMode.Impulse);
                        cloneActive = false;
                    }
                    else
                    {
                        if (powerbar.value >= 40)
                        {
                            stoneShot = true;
                            timerSlider.SetActive(false); // pause timer
                            rB.AddForce(0, 0, 1.5f * power, ForceMode.Impulse);
                            cloneActive = false;
                        }
                        else if (tutorialDialogueIndex == 1)
                            NextStep();
                    }

                    if (tutorialDialogueIndex == 9)
                    {
                        tutorialDialogueIndex++;
                        NextStep();
                    }
                    else if (tutorialDialogueIndex == 10)
                        NextStep();

                    power = 1f;
                }
            }

            if (canChangeDirection)
            {
                if (plane.Raycast(ray, out dist)) // arrow head follows mouse position
                {
                    lR.enabled = true;
                    throwDir.position = ray.GetPoint(dist);
                    throwDir.position = (throwDir.position - clone.transform.position).normalized * 1.5f + clone.transform.position;
                }

                lR.SetPosition(0, clone.transform.position + new Vector3(0, 0.1f, 0));
                lR.SetPosition(1, throwDir.position + new Vector3(0, 0.1f, 0));

                if (canPickUp)
                {
                    if (Input.GetMouseButton(1)) // rmb: re-position the stone
                    {
                        lR.enabled = false;
                        clone.transform.position = ray.GetPoint(dist);

                        if (Vector3.Distance(spawnPos.position, clone.transform.position) > 1.5f)
                        {
                            // forces stone re-positioning to certain area, cannot go beyond the blue circle
                            clone.transform.position = (clone.transform.position - spawnPos.position).normalized * 1.5f + spawnPos.position;
                        }

                        if (tutorialDialogueIndex == 6)
                            NextStep();
                    }
                    else if (Input.GetMouseButtonUp(1))
                    {
                        lR.enabled = true;
                        mainCamera.transform.position = clone.transform.position + new Vector3(0, 2, -2.75f); // camera focuses on the stone
                        mainCamera.transform.eulerAngles = new Vector3(30f, 0f, 0f);

                        if (tutorialDialogueIndex == 7)
                            NextStep();
                    }

                    if (canCountdown)
                        turnTime -= Time.deltaTime;
                }
            }

            if (turnTime <= 0) // timeout
            {
                lR.enabled = false;
                Destroy(clone);

                if (tutorialDialogueIndex == 9 || tutorialDialogueIndex == 10)
                {
                    if (tutorialDialogueIndex == 9)
                        NextStep();

                    timer.resetTimer();
                    RemoveClones();
                    SpawnClone();
                }
                //SwitchTurn();
            }
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
                        canDelay = false;
                        delay = 0;

                        if (canIntersect)
                        {
                            // if stone does not stop at ValidArea (scoring area)
                            if (!clone.GetComponent<Collider>().bounds.Intersects(validArea.GetComponent<Collider>().bounds))
                            {
                                Debug.Log("Does not intersect");

                                if (tutorialDialogueIndex == 15 || tutorialDialogueIndex == 16)
                                {
                                    if (tutorialDialogueIndex == 15)
                                        NextStep();

                                    RemoveClones();
                                    SpawnClone();
                                    timerSlider.SetActive(false);
                                }
                            }
                            else
                            {
                                if (tutorialDialogueIndex == 15)
                                {
                                    tutorialDialogueIndex++;
                                    NextStep();
                                }
                                else
                                    NextStep();
                            }
                        }

                        if (!(tutorialDialogueIndex == 15 || tutorialDialogueIndex == 16))
                            NextStep();
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

    public async void NextStep()
    {
        nextStepActive = true;

        if (nextStepActive)
        {
            if (tutorialDialogueIndex != tutorialDialogues.Length - 1)
            {
                tutorialDialogueIndex++;
                tutorialBody.text = tutorialDialogues[tutorialDialogueIndex];

                if (tutorialDialogueIndex == 1)
                {
                    await Task.Delay(100);
                    canThrow = true;
                    SpawnClone();
                    SetHint(442, -445, 1020, 170);
                    Time.timeScale = 1;
                }

                if (tutorialDialogueIndex == 4)
                {
                    canChangeDirection = true;
                }

                if (tutorialDialogueIndex == 5)
                {
                    await Task.Delay(100);
                    RemoveClones();
                    SpawnClone();
                }

                if (tutorialDialogueIndex == 6)
                {
                    canThrow = false;
                    canPickUp = true;
                    RemoveClones();
                    SpawnClone();
                }

                if (tutorialDialogueIndex == 8)
                    SetHint(-607, 410, 710, 150);

                if (tutorialDialogueIndex == 9)
                {
                    await Task.Delay(100);
                    timer.resetTimer();
                    canThrow = true;
                    canCountdown = true;
                    canDelay = true;
                    //Time.timeScale = 1;
                }

                if (tutorialDialogueIndex == 13)
                    SetHint(-696, -107, 338, 865);

                if (tutorialDialogueIndex == 14)
                    SetHint(-696, -205, 338, 565);

                if (tutorialDialogueIndex == 15)
                {
                    await Task.Delay(100);
                    canCountdown = false;
                    canDelay = true;
                    canIntersect = true;
                    RemoveClones();
                    SpawnClone();
                    timerSlider.SetActive(false);
                }

                if (tutorialDialogueIndex == 18)
                    SetHint(-696, -50, 100, 100);

                if (tutorialDialogueIndex == 19)
                {
                    RemoveClones();
                    gameCanvas.SetActive(false);
                    topDownCamera.SetActive(false);
                    await Task.Delay(100);
                    mainCamera.transform.position = new Vector3(-3, 5, 18);
                    mainCamera.transform.eulerAngles = new Vector3(90f, 0f, 0f);
                    stoneSetups[0].SetActive(true);
                    SetHint(-82, -68, 100, 100);
                }

                if (tutorialDialogueIndex == 21)
                {
                    stoneSetups[0].SetActive(false);
                    stoneSetups[1].SetActive(true);
                }

                if (tutorialDialogueIndex == 23)
                    SetHint(82, -330, 100, 100);

                if (tutorialDialogueIndex == 24)
                {
                    stoneSetups[1].SetActive(false);
                    stoneSetups[2].SetActive(true);
                    SetHint(105, -155, 100, 100);
                }

                if ((tutorialDialogueIndex >= 1 && tutorialDialogueIndex <= 3) || (tutorialDialogueIndex >= 5 && tutorialDialogueIndex <= 7) || (tutorialDialogueIndex >= 9 && tutorialDialogueIndex <= 11) || tutorialDialogueIndex == 15 || tutorialDialogueIndex == 16)
                {
                    Color color = new Color(1, 1, 1, 0.5f);
                    tutorialPanel.GetComponent<Image>().color = color;
                    tutorialButton.enabled = false;
                    tutorialImage.SetActive(false);
                }
                else
                {
                    Color color = new Color(1, 1, 1, 1);
                    tutorialPanel.GetComponent<Image>().color = color;
                    tutorialButton.enabled = true;
                    tutorialImage.SetActive(true);
                }
            }
            else
                SceneManager.LoadScene("StartMenu");
        }
    }

    public async void SetHint(float x, float y, float width, float height)
    {
        hint.anchoredPosition = new Vector2(x, y);
        hint.sizeDelta = new Vector2(width, height);

        hint.gameObject.SetActive(true);
        await Task.Delay(2500);
        hint.gameObject.SetActive(false);
    }
}
