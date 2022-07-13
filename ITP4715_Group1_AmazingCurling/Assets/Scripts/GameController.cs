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
    [SerializeField] private GameObject venue;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject mountain;
    // Start is called before the first frame update

    private void Update()
    {
        int venue = PlayerPrefs.GetInt("venue");
        if(venue == 0)
        {
            wall.SetActive(false);
            mountain.SetActive(true);
        }
        if(venue == 1)
        {
            wall.SetActive(true);
            mountain.SetActive(false);
        }
    }
    public void SwapScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void actVenue(bool status)
    {
        venue.SetActive(status);
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
    public void SetVenue(int val)
    {
        if(val == 1)
        {
            wall.SetActive(true);
            mountain.SetActive(false);
            PlayerPrefs.SetInt("venue", val);
        }
        if(val == 0)
        {
            wall.SetActive(false);
            mountain.SetActive(true);
            PlayerPrefs.SetInt("venue", val);
        }

    }
}
