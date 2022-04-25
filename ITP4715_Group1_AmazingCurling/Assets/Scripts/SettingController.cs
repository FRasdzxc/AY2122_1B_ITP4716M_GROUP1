using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingController : MonoBehaviour
{
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject confirmPanel;

    public void SetGraphics(int graphics)
    {
        QualitySettings.SetQualityLevel(graphics);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void CloseSettings()
    {
        settings.SetActive(false);
    }

    public void ExitGame()
    {
        confirmPanel.SetActive(true);
    }

    public void FinalExitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {
        confirmPanel.SetActive(false);
    }
}
