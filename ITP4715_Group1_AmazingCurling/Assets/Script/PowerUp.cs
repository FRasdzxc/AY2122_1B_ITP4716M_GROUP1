using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private Image imagePowerUp;
    [SerializeField]
    private TMP_Text textPowerAmt;

    private bool isPowerUp = false;
    private bool isDirectionUp = true;
    private float amtPower = 0.0f;
    private float powerSpeed = 100.0f;

    // Update is called once per frame
    void Update()
    {
        if (isPowerUp)
        {
            PowerActive();
        }
    }

    void PowerActive()
    {
        if (isDirectionUp)
        {
            amtPower += Time.deltaTime * powerSpeed;
            if(amtPower > 100)
            {
                isDirectionUp = false;
                amtPower = 100.0f;
            }
        }
        else
        {
            amtPower -= Time.deltaTime * powerSpeed;
            if (amtPower < 0)
            {
                isDirectionUp = false;
                amtPower = 0.0f;
            }
        }
        imagePowerUp.fillAmount = amtPower / 100.0f;
    }

    public void StartPowerUp()
    {
        isPowerUp = true;
        amtPower = 0.0f;
        isDirectionUp = true;
        textPowerAmt.text = "";
    }

    public void EndPowerUp()
    {
        isPowerUp = false;
        textPowerAmt.text = amtPower.ToString("F0");
    }
}
