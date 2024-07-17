using System;
using Photon.Pun;
using UnityEngine;

public class MultiplayerGameManager : MonoBehaviour
{
    private const string PlayerPrefabName = "Prefabs/Player";

    private void Start()
    {
        PhotonNetwork.Instantiate(PlayerPrefabName, Vector3.zero, Quaternion.identity);
    }
}
