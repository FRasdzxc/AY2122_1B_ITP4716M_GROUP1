using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject g;
    // Start is called before the first frame update
    public void SwapScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void swapStatus()
    {
        g.SetActive(true);
    }
}
