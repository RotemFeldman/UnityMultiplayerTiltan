using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine.UI;

public class PlayersList : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI _playersList;
    [SerializeField] private Button _startGameButton;

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        UpdatePlayersList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        UpdatePlayersList();
    }

    public override void OnJoinedRoom()
    {
        UpdatePlayersList();
    }

    private void UpdatePlayersList()
    {
        _playersList.text = "Current Players:\n";
        
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log(player.Value.NickName);
            _playersList.text += player.Value.NickName + "\n";
        }
        
        StartGameButtonUpdate();
    }
    
    //TODO return minimum players to start to 2
    private void StartGameButtonUpdate()
    {
        _startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        if(!PhotonNetwork.IsMasterClient)
            return;
        
        _startGameButton.interactable = (PhotonNetwork.CurrentRoom.PlayerCount >= 2);
    }

    private void Start()
    {
        UpdatePlayersList();
    }
}