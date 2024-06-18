using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class MainMenuConnectionManager : MonoBehaviourPunCallbacks
{
    [HideInInspector] public static string NickName { get; private set; }
    
    
    [Header("UI")]
    [SerializeField] private GameObject _connectScreen;
    [SerializeField] private TMP_InputField _nickNameInputField;
    
    [Space(10)]
    [SerializeField] private GameObject _joinLobbyScreen;
    [SerializeField] private TMP_InputField _lobbyInputField;
    
    [Space(10)]
    [SerializeField] private GameObject _joinRoomScreen;
    [SerializeField] private TMP_InputField _roomNameInputField;
    [SerializeField] private TMP_InputField _roomMaxPlayersInputField;
    
    [Space(10)]
    [SerializeField] private GameObject _inRoomScreen;
    
    [Space(10)]
    [SerializeField] private TextMeshProUGUI _debugPhotonText;
    

    private enum UIScreen
    {
        Connect,
        JoinLobby,
        JoinRoom,
        InRoom
    }
    

    public void Connect()
    {
        PhotonNetwork.NickName = _nickNameInputField.text;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby(new TypedLobby(_lobbyInputField.text, LobbyType.Default));
    }

    public void CreateRoom()
    {
        RoomOptions ro = new RoomOptions();
        if (int.TryParse(_roomMaxPlayersInputField.text, out int result) && result > 0)
        {
            ro.MaxPlayers = result;
            PhotonNetwork.CreateRoom(_roomNameInputField.text,ro);
        }
        else
        {
            Debug.LogWarning("Max Players InputField must contain an integer greater than 0");
        }
    }

    

    public override void OnConnectedToMaster()
    {
        Debug.Log(this + "Photon connection successful");
        base.OnConnectedToMaster();
        
        
        SwitchUIScreen(UIScreen.JoinLobby);
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnected from master. " + cause.ToString());
        
        SwitchUIScreen(UIScreen.Connect);
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Successfully joined lobby " + PhotonNetwork.CurrentLobby.Name);
        
        SwitchUIScreen(UIScreen.JoinRoom);
        
    }
    
    public static void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Successfully joined room " + PhotonNetwork.CurrentRoom.ToString());
        
        SwitchUIScreen(UIScreen.InRoom);
    }
    

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log($"Connection to room failed because {message}. return code: {returnCode}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log($"Room creation failed because {message}. return code: {returnCode}");
    }

    public void LeaveRoom()
    {
        if (!PhotonNetwork.InRoom)
            return;
        
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Left Room");
        
        SwitchUIScreen(UIScreen.JoinRoom);
    }


    private void Start()
    {
        SwitchUIScreen(UIScreen.Connect);
        
    }

    private void Update()
    {
        _debugPhotonText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    private void SwitchUIScreen(UIScreen screen)
    {
        _connectScreen.SetActive(false);
        _joinRoomScreen.SetActive(false);
        _joinLobbyScreen.SetActive(false);
        _inRoomScreen.SetActive(false);

        switch (screen)
        {
            case UIScreen.Connect:
                _connectScreen.SetActive(true);
                break;
            case UIScreen.InRoom:
                _inRoomScreen.SetActive(true);
                break;
            case UIScreen.JoinLobby:
                _joinLobbyScreen.SetActive(true);
                break;
            case UIScreen.JoinRoom:
                _joinRoomScreen.SetActive(true);
                break;
            
        }
    }
    
}
