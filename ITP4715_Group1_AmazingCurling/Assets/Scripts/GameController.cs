using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject modeSelect;
    [SerializeField] private GameObject setting;
    [SerializeField] private GameObject roundSelect;
    [SerializeField] private GameObject createjoin;
    [SerializeField] private ParticleSystem snow;
    // Start is called before the first frame update


    public void SwapScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void actMode(bool status)
    {
        modeSelect.SetActive(status);
    }
    public void actRound(bool status)
    {
        roundSelect.SetActive(status);
    }
    public void actSetting(bool status)
    {
        setting.SetActive(status);
    }
    public void actJoin(bool status)
    {
        createjoin.SetActive(status);
    }
}
