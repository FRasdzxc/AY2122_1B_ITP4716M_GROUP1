using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonMultiPlayer : MonoBehaviour
{
    [SerializeField] private string VersioName = "0.1";

    [SerializeField] private GameObject UsernameMenu;

    [SerializeField] private GameObject ConnecyPanel;

    [SerializeField] private GameObject CreateButton;

    [SerializeField] private InputField UsernameInput;

   [SerializeField] private InputField CreatGameInput;

    [SerializeField] private InputField JoinGameInput;

    [SerializeField] private GameObject StartButton;
    // Start is called before the first frame update
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(VersioName);


    }
    private void Start()
    {
        UsernameMenu.SetActive(true);

    }


    // Update is called once per frame
    private void OnConnectedToMaster()

    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected");
    }

    public void ChangeUserNameInput()
    {
        if (UsernameInput.text.Length >= 3)
        {
            StartButton.SetActive(true);
        }
        else
        {
            StartButton.SetActive(false);
        }
    }

    public void SetUserName()
    {
        UsernameMenu.SetActive(false);
        PhotonNetwork.playerName = UsernameInput.text;
    }
    public void CreatGame()
    {
        PhotonNetwork.CreateRoom(CreatGameInput.text, new RoomOptions() { maxPlayers = 5 }, null);
    }
    public void JoinGame()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.maxPlayers = 5;
        PhotonNetwork.JoinOrCreateRoom(JoinGameInput.text, roomOptions,TypedLobby.Default);
 
       
    }
    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("OlympicGames");
    }
}





