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

    private Dictionary<int,Player> _players = new Dictionary<int, Player>();

    private void Update()
    {
        UpdatePlayers();
    }

    private void UpdatePlayers()
    {
        if (PhotonNetwork.CurrentRoom.Players.Count != _players.Count)
        {
            _playersList.text = string.Empty;
            
            _players = PhotonNetwork.CurrentRoom.Players;

            foreach (var p in _players)
            {
                _playersList.text += p.Value.ToString() + "\n";
            }
        }
    }

}
