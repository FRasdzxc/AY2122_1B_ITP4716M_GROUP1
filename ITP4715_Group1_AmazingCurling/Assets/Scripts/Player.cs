using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Photon.MonoBehaviour
{

    public PhotonView photonView;
    public GameObject PlayerCamera;
    public Text PlayerNameText;
    // Start is called before the first frame update
    private void Awake()
    {
        if (photonView.isMine)
        {
            PlayerCamera.SetActive(true);
        }
    }
    private void CheckInput()
    {

    }
}
