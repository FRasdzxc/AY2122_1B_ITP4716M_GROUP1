using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    private GameObject g;
    public GameObject pauseMenu;
    [SerializeField] private ParticleSystem snow;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
    }
    public async void ResumeGame()
    {
        await Task.Delay(100); // prevents Resume button to overlap with stone throwing
        Time.timeScale = 1;
    }

    public void SwapScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void swapStatus(string name)
    {
        g = GameObject.Find(name);
        if (g.activeSelf == false)
            g.SetActive(true);
        else
            g.SetActive(false);
    }
}
