using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using NUnit.Framework;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class MultiplayerGameManager : MonoBehaviourPun
{
    private const string PlayerPrefabName = "Prefabs/Player";
    private const string ClientIsReady_RPC = nameof(ClientIsReady);
    private const string SetSpawnPoint_RPC = nameof(SetSpawnPoint);
    private const string GameStarted_RPC = nameof(GameStarted);
    private const string EndGame_RPC = nameof(EndGame);

    private const string GetAvailableCharacters_RPC = nameof(GetAndRefreshAvailableCharacters);

    private PlayerController myPlayerController;
    private int _playersReady;


    [Header("Spawn Points")]
    [SerializeField] private SpawnPoint[] spawnPoints;

    [Header("Characters")]
    [SerializeField] private SelectableCharacter[] characters;

    [SerializeField] private GameObject characterSelectionScreen;

    private SelectableCharacter _selectableCharacter;
    
    private void Start()
    {
        GetAndRefreshAvailableCharacters(99);
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
        var meshRend = player.GetComponent<MeshRenderer>();
        meshRend.material.color = _selectableCharacter.charColor.color;
        myPlayerController = player.GetComponent<PlayerController>();
        myPlayerController.enabled = false;
    }

    public void TakeCharacter(SelectableCharacter character)
    {
        
        _selectableCharacter = character.Take();
        characterSelectionScreen.SetActive(false);
        

        photonView.RPC(GetAvailableCharacters_RPC,RpcTarget.All,_selectableCharacter.Id);
        NotifyIsReadyToMasterClient();
    }
    
    [PunRPC]
    private void GetAndRefreshAvailableCharacters(int idTaken)
    {
        if (idTaken < characters.Length)
        {
            characters[idTaken].Take();
        }
        
        foreach (var c in characters)
        {
            if(c.IsTaken)
                c.gameObject.SetActive(false);
        }
        
    }
    

    #region RPC's

    [PunRPC]
    private void ClientIsReady(PhotonMessageInfo info)
    {
        Debug.Log( info.Sender + " is ready");
        SpawnPoint randomSpawnPoint = GetRandomSpawnPoint();
        randomSpawnPoint.Take();
            
        info.photonView.RPC(SetSpawnPoint_RPC, info.Sender, randomSpawnPoint.Id);

        _playersReady++;
        if (_playersReady >= PhotonNetwork.CurrentRoom.PlayerCount)
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
        myPlayerController.OnLastPlayerRemaining.AddListener(OnEndGame);
    }

    private void OnEndGame()
    {
        photonView.RPC(EndGame_RPC,RpcTarget.All);
    }
    
    [PunRPC]
    private void EndGame()
    {
       myPlayerController.enabled = false;
    }

    #endregion
}
