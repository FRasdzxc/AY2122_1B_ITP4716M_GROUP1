using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [SerializeField] private GameObject[] obstacles;
    private GameObject clone;
    private List<GameObject> clones;

    void Awake()
    {
        clones = new List<GameObject>();
        hideObstacles();
    }

    public void showObstacle()
    {
        int index = Random.Range(0, obstacles.Length);
        Vector3 position = new Vector3(Random.Range(-1.5f, -4.5f), obstacles[index].transform.position.y, Random.Range(-8.75f, 8.75f));
        Quaternion rotation = new Quaternion(obstacles[index].transform.rotation.x, Random.Range(0f, 1f), obstacles[index].transform.rotation.z, obstacles[index].transform.rotation.w);

        clone = Instantiate(obstacles[index], position, rotation);
        clone.SetActive(true);
        clones.Add(clone);
    }

    public void hideObstacles()
    {
        for (int i = 0; i < obstacles.Length; i++)
            obstacles[i].SetActive(false);

        for (int i = 0; i < clones.Count; i++)
            Destroy(clones[i]);
    }
}
