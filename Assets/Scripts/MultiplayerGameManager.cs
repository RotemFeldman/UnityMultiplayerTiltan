using System;
using System.Collections.Generic;
using DefaultNamespace;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class MultiplayerGameManager : MonoBehaviourPun
{
    private const string PlayerPrefabName = "Prefabs/Player";
    private const string ClientIsReady_RPC = nameof(ClientIsReady);
    private const string SetSpawnPoint_RPC = nameof(SetSpawnPoint);
    private const string GameStarted_RPC = nameof(GameStarted);

    private PlayerController myPlayerController;
    private int playersReady;

    [Header("Spawn Points")]
    [SerializeField] private SpawnPoint[] spawnPoints;

    private void Start()
    {
        NotifyIsReadyToMasterClient();
    }

    public void NotifyIsReadyToMasterClient()
    {
        photonView.RPC(ClientIsReady_RPC,RpcTarget.MasterClient);
    }

    private SpawnPoint GetRandomSpawnPoint()
    {
        List<SpawnPoint> availablePoints = new();

        foreach (var point in spawnPoints)
        {
            if (!point.IsTaken)
            {
                availablePoints.Add(point);
            }
        }

        if (availablePoints.Count == 0)
        {
            Debug.Log("all points are taken");
            return null;
        }
        
        int rnd = Random.Range(0, availablePoints.Count);
        return availablePoints[rnd];
    }

    private void SpawnPlayer(SpawnPoint point)
    {
        point.Take();
        GameObject player = PhotonNetwork.Instantiate(PlayerPrefabName, point.transform.position, point.transform.rotation);
        myPlayerController = player.GetComponent<PlayerController>();
        myPlayerController.enabled = false;
    }

    #region RPC's

    [PunRPC]
    private void ClientIsReady(PhotonMessageInfo info)
    {
        Debug.Log( info.Sender + " is ready");
        SpawnPoint randomSpawnPoint = GetRandomSpawnPoint();
        randomSpawnPoint.Take();
            
        info.photonView.RPC(SetSpawnPoint_RPC, info.Sender, randomSpawnPoint.Id);

        playersReady++;
        if (playersReady >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            photonView.RPC(GameStarted_RPC, RpcTarget.All);
        }
    }
    
    [PunRPC]
    private void SetSpawnPoint(int spawnPointID)
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.Id == spawnPointID)
            {
                SpawnPlayer(spawnPoint);
                break;
            }
        }
    }
    
    [PunRPC]
    private void GameStarted()
    {
        myPlayerController.enabled = true;
        Debug.Log("The might master client has the Game Started");
    }

    #endregion
}
