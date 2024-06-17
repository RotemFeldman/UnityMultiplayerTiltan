using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using Unity.VisualScripting;

public class PlayersList : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI _playersList;


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

    private void UpdatePlayersList()
    {
        _playersList.text = "Current Players:\n";
        
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log(player.Value.NickName);
            _playersList.text += player.Value.NickName + "\n";
        }
    }

    private void Start()
    {
       UpdatePlayersList();
    }
}
