using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneController : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;

    [SerializeField] private GameObject stone1;
    [SerializeField] private GameObject stone2;

    [SerializeField] private Transform spawnPos;
    [SerializeField] private Transform validArea;

    private int round;                          
    private enum Turn { red, yellow };          
    private Turn turn;

    private GameObject clone;
    private bool cloneDraggable;
    private Plane plane;
    private Transform selectionTrans;
    private RaycastHit rcHit;
    private float dist;

    [SerializeField] private int throwPower;

    private float time;

    private ScoreController sC;

    // Start is called before the first frame update
    void Start()
    {
        // Hi! How are you?
        plane = new Plane(Vector3.up, Vector3.up * 0);

        SetupGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && cloneDraggable)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (selectionTrans)
            {
                if (plane.Raycast(ray, out dist) && Physics.Raycast(ray, out rcHit))
                {
                    selectionTrans.position = ray.GetPoint(dist);

                    if (Vector3.Distance(selectionTrans.transform.position, spawnPos.position) > 1.5f)
                    {
                        selectionTrans.position = (clone.transform.position - spawnPos.position).normalized * 1.5f + spawnPos.position;
                    }

                    rcHit.rigidbody.isKinematic = true;
                }
            }
            else
            {
                selectionTrans = clone.transform;
            }
        }
        else if (Input.GetMouseButtonUp(0) && cloneDraggable)
        {
            Rigidbody rB = selectionTrans.GetComponent<Rigidbody>();
            rB.isKinematic = false;
            selectionTrans = null;

            Vector3 throwOutput = -(clone.transform.position - spawnPos.position) * throwPower;
            rB.AddForce(throwOutput, ForceMode.Impulse);

            cloneDraggable = false;
        }

        if (clone && !cloneDraggable)
        {
            mainCamera.transform.position = clone.transform.position + new Vector3(0, 2, -2.75f);
            time += Time.deltaTime;

            if (clone.GetComponent<Rigidbody>().velocity.magnitude > 0f && clone.GetComponent<Rigidbody>().velocity.magnitude < 0.01f)
            {
                if (!clone.GetComponent<Collider>().bounds.Intersects(validArea.GetComponent<Collider>().bounds))
                    Destroy(clone);

                SwitchTurn();

                time = 0f;
            }
            else if (clone.GetComponent<Rigidbody>().velocity.magnitude > 0f && time > 7f)
            {
                Destroy(clone);
                SwitchTurn();

                time = 0f;
            }
        }

        if (Input.GetMouseButtonDown(2)) // will be changed to GUI event
        {
            SetupGame();
            sC = GameObject.FindGameObjectWithTag("ScoreController").GetComponent<ScoreController>();
            sC.SetupController();
        }
    }

    private void SetupGame()
    {
        RemoveClones();

        round = 1;
        turn = Turn.red;
        cloneDraggable = false;
        time = 0f;

        SpawnClone();
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

        cloneDraggable = true;

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
        if (round == 8 && turn == Turn.yellow)
        {
            sC = GameObject.FindGameObjectWithTag("ScoreController").GetComponent<ScoreController>();
            sC.CountScore();
        }
        else
        {
            if (turn == Turn.red)
            {
                turn = Turn.yellow;
            }
            else
            {
                round++;
                turn = Turn.red;
            }

            SpawnClone();
        }
    }
    public int GetThrowPower()
    {

    }
}
  