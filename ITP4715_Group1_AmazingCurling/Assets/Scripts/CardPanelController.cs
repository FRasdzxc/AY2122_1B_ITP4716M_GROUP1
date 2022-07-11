using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CardPanelController : MonoBehaviour
{
    public async void CloseCardPanel()
    {
        gameObject.SetActive(false);
        await Task.Delay(100);
        Time.timeScale = 1;
    }

    public async void SelectCard(Button card)
    {
        card.interactable = false;
        gameObject.SetActive(false);
        await Task.Delay(100);
        Time.timeScale = 1;
    }
}
