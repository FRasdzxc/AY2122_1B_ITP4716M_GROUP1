using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneControllerV2 : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject stone1;
    [SerializeField] private GameObject stone2;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private Transform throwDir;
    [SerializeField] private Transform validArea;
    [SerializeField] private GameObject timers;
    Timer timer;

    private GameObject clone;
    private bool cloneActive;
    private float power;
    private float delay;
    private float turnTime;
    private float thrownTime;

    private enum Turn { red, yellow };
    private Turn turn;
    private int round;
    private int end;
    private int maxEnd;

    private LineRenderer lR;
    private ScoreController[] sC;
    // Start is called before the first frame update
    void Start()
    {
        SetupController();
        timer = timers.GetComponent<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cloneActive)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.up * 0);
            float dist;

            if (plane.Raycast(ray, out dist))
            {
                throwDir.position = ray.GetPoint(dist);
                throwDir.position = (throwDir.position - clone.transform.position).normalized * 1.5f + clone.transform.position;
            }

            if (Input.GetMouseButton(0))
            {
                power += Time.deltaTime * 200;
                timer.pauseTimer();

            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (power > 200)
                {
                    power = 200;
                }
                Debug.Log(power); // remove this

                Rigidbody rB = clone.GetComponent<Rigidbody>();
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

            if (turnTime > 20)
            {
                Debug.Log("turn timeout"); // remove this
                Destroy(clone);
                SwitchTurn();
                // sends a message to gui saying that time has run out for turn, switched turn
            }

            lR.SetPosition(0, clone.transform.position + new Vector3(0, 0.1f, 0));
            lR.SetPosition(1, throwDir.position + new Vector3(0, 0.1f, 0));

            turnTime += Time.deltaTime;
        }
        else
        {
            if (clone)
            {
                mainCamera.transform.position = clone.transform.position + new Vector3(0, 2, -2.75f);
                mainCamera.transform.eulerAngles = new Vector3(30f, 0f, 0f);

                Rigidbody rB = clone.GetComponent<Rigidbody>();

                if (Input.GetMouseButtonDown(0))
                {
                    rB.AddForce(-Vector3.right, ForceMode.Impulse);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    rB.AddForce(Vector3.right, ForceMode.Impulse);
                }

                if (rB.velocity.magnitude == 0)
                {
                    delay += Time.deltaTime;

                    if (delay >= 1f)
                    {
                        if (!clone.GetComponent<Collider>().bounds.Intersects(validArea.GetComponent<Collider>().bounds))
                        {
                            Destroy(clone);
                            // send a message to gui saying that the stone (clone) is not valid
                        }

                        SwitchTurn();
                        delay = 0f;
                    }
                }

                if (thrownTime > 8) // when stone (clone) is stuck or clips through the plane
                {
                    Destroy(clone);
                    SwitchTurn();
                }

                thrownTime += Time.deltaTime;
            }

            lR.enabled = false;
        }
    }

    private void SpawnClone()
    {
        if (turn == Turn.red)
        {
            clone = Instantiate(stone1, spawnPos.position, spawnPos.rotation);
            clone.tag = "RedClone";
        }
        else
        {
            clone = Instantiate(stone2, spawnPos.position, spawnPos.rotation);
            clone.tag = "YellowClone";
        }

        cloneActive = true;
        turnTime = 0f;
        thrownTime = 0f;
        lR.enabled = true;

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
    }

    private void SwitchTurn()
    {
        if (end == maxEnd && round == 8 && turn == Turn.yellow)
        {
            Debug.Log("game ended with " + end + " end(s)"); // remove this

            // gui stating which team wins
            for (int i = 0; i < end; i++)
            {
                //sC[i] = GameObject.FindGameObjectWithTag("ScoreController").GetComponent<ScoreController>();
                Debug.Log("end " + (i + 1) + ", " + sC[i].GetTeam() + " wins, score: " + sC[i].GetScore());
            }
            end = 1;
        }
        else
        {
            if (round == 8 && turn == Turn.yellow) // round ends
            {
                sC[end - 1] = GameObject.FindGameObjectWithTag("ScoreController").GetComponent<ScoreController>();
                sC[end - 1].CountScore();

                end++;
                round = 1;
                turn = Turn.red;
                SpawnClone();
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
                timer.resetTimer();
                Debug.Log("end " + end + ", round " + round + ", turn " + turn); // remove this
                // gui tracking end, round and turn
            }
        }
    }

    private void SetupController()
    {
        RemoveClones();

        power = 1f;
        delay = 0f;
        turnTime = 0f;
        thrownTime = 0f;
        turn = Turn.red;
        round = 1;
        end = 1;
        maxEnd = 1; // player preference
        lR = spawnPos.GetComponent<LineRenderer>();
        sC = new ScoreController[maxEnd];

        SpawnClone();
    }
}
