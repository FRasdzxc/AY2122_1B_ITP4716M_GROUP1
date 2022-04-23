using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerBarManager : MonoBehaviour
{
    [SerializeField] Slider powerSlider;

    void Start()
    {
        if (!PlayerPrefs.HasKey("power"))
        {
            PlayerPrefs.SetFloat("power", 0);
            Load();
        }
        else
        {
            Load();
        }
    }

    public void ChangePower()
    {
        //?? = powerSlider.value;
        Save();
    }
    public void Load()
    {
        powerSlider.value = PlayerPrefs.GetFloat("power");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("power", powerSlider.value);
    }
}
