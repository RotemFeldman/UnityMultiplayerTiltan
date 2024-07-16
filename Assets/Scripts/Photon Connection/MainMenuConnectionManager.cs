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
    private const string GAME_SCENE = "GameScene";
    
    private UIManager _uiManager;
    
    public void Connect()
    {
        PhotonNetwork.NickName = _uiManager.NickNameInputField.text;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby(new TypedLobby(_uiManager.LobbyInputField.text, LobbyType.Default));
    }

    public void CreateRoom()
    {
        RoomOptions ro = new RoomOptions();
        if (int.TryParse(_uiManager.RoomMaxPlayersInputField.text, out int result) && result > 0)
        {
            ro.MaxPlayers = result;
            PhotonNetwork.CreateRoom(_uiManager.RoomNameInputField.text,ro);
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
        
        _uiManager.SwitchUIScreen(UIManager.UIScreen.JoinLobby);
        
        if(!PhotonNetwork.InLobby)
            JoinLobby();
            
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnected from master. " + cause);
        
        _uiManager.SwitchUIScreen(UIManager.UIScreen.Connect);
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Successfully joined lobby " + PhotonNetwork.CurrentLobby.Name);
        
        _uiManager.SwitchUIScreen(UIManager.UIScreen.JoinRoom);
        
    }
    
    public static void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Successfully joined room " + PhotonNetwork.CurrentRoom);
        
        _uiManager.SwitchUIScreen(UIManager.UIScreen.InRoom);
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
        
        _uiManager.SwitchUIScreen(UIManager.UIScreen.JoinRoom);
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        Debug.Log("left lobby");
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(GAME_SCENE);
        }
    }

    private void Start()
    {
        _uiManager = UIManager.Instance;
        PhotonNetwork.AutomaticallySyncScene = true;
    }


}
