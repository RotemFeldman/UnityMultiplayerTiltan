using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chat;
using DefaultNamespace;
using NUnit.Framework;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Unity.Cinemachine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

public class MultiplayerGameManager : MonoBehaviourPunCallbacks
{
    private const string PlayerPrefabName = "Prefabs/Player";
    private const string BoostPrefabName = "Prefabs/RoomObject";
    private const string ClientIsReady_RPC = nameof(ClientIsReady);
    private const string SetSpawnPoint_RPC = nameof(SetSpawnPoint);
    private const string SetBoostSpawner_RPC = nameof(SetBoostSpawner);
    private const string GameStarted_RPC = nameof(GameStarted);
    private const string EndGame_RPC = nameof(EndGame);
    private const string GetAvailableCharacters_RPC = nameof(GetAndRefreshAvailableCharacters);
    private const string SetNextSpawnPoint_RPC = nameof(SetNextSpawnPoint);
    private PlayerController _myPlayerController;
    private int _playersReady;

    [Header("Spawn Points")]
    [SerializeField] private SpawnPoint[] spawnPoints;

    [Header("Boost Spawners")]
    [SerializeField] private BoostSpawnPoint[] boostSpawners;

    [Header("Characters")]
    [SerializeField] private SelectableCharacter[] characters;

    [SerializeField] private GameObject characterSelectionScreen;

    private SelectableCharacter _selectableCharacter;
    private SpawnPoint _nextSpawnPoint;

    [Header("Chat")] [SerializeField] private ChatManager chat;

    [Header("Camera")] [SerializeField] private CinemachineCamera cinemachineCam;
    
    private void Start()
    {
        GetAndRefreshAvailableCharacters(99);
        if (PhotonNetwork.IsMasterClient)
        {
            SetNextSpawnPoint();
            StartCoroutine((WaitTenSecondsAndSpawn(boostSpawners.Length)));
        }
    }

    private void NotifyIsReadyToMasterClient()
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
    
    private void SetNextSpawnPoint()
    {
        _nextSpawnPoint = GetRandomSpawnPoint();
        if (_nextSpawnPoint != null)
        {
            photonView.RPC(SetNextSpawnPoint_RPC, RpcTarget.All, _nextSpawnPoint.Id);
        }
    }

    private BoostSpawnPoint GetRandomBoostSpawner()
    {
        List<BoostSpawnPoint> availableBoostSpawners = new();

        foreach (var spawner in boostSpawners)
        {
            if (!spawner.IsTaken)
            {
                availableBoostSpawners.Add(spawner);
            }
        }

        if (availableBoostSpawners.Count == 0)
        {
            Debug.Log("all spawners taken");
            return null;
        }
        
        int randomBoostSpawnerIndex = Random.Range(0, availableBoostSpawners.Count);
        return availableBoostSpawners[randomBoostSpawnerIndex];
    }

    private void SpawnBooster(BoostSpawnPoint boost)
    {
        if (boost != null)
        {
            boost.Take();
            GameObject item = PhotonNetwork.Instantiate(BoostPrefabName, boost.transform.position, boost.transform.rotation); 
            item.GetComponent<Boost>().spawner = boost;
        }
    }

    private void SpawnPlayer(SpawnPoint point)
    {
        point.Take();
        GameObject player = PhotonNetwork.Instantiate(PlayerPrefabName, point.transform.position, point.transform.rotation);
        player.transform.LookAt(Vector3.zero);
        
        var meshRend = player.GetComponent<MeshRenderer>();

        //set camera follow target
        if (player.GetPhotonView().IsMine)
        {
            cinemachineCam.Follow = player.transform;
            Debug.Log("follow " + PhotonNetwork.NickName);
        }

        //set color to selected
        meshRend.material.color = _selectableCharacter.charColor.color;
        chat.playerColor = _selectableCharacter.charColor.color;
        
        //set local playerController and disable until game starts
        _myPlayerController = player.GetComponent<PlayerController>();
        chat.playerController = _myPlayerController;
        _myPlayerController.enabled = false;
    }

    private IEnumerator WaitTenSecondsAndSpawn(int maxSpawns)
    {
        int spawnCount = 0;
        while (spawnCount < maxSpawns)
        {
            SpawnBooster(GetRandomBoostSpawner());
            SetNextSpawnPoint();
            spawnCount++;
            yield return new WaitForSeconds(10f);
        }
    }

    #region Character Selection
    
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
    
    #endregion
    
    #region Connection RPC's

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
    private void SetNextSpawnPoint(int spawnPointID)
    {
        _nextSpawnPoint = spawnPoints.FirstOrDefault(point => point.Id == spawnPointID);
    }

    [PunRPC]
    private void SetBoostSpawner(int boostSpawnID)
    {
        foreach (var boostSpawn in boostSpawners)
        {
            if (boostSpawn.Id == boostSpawnID)
            {
                SpawnBooster(boostSpawn);
                break;
            }
        }
    }

    [PunRPC]
    private void GameStarted()
    {
        _myPlayerController.enabled = true;
        _myPlayerController.OnLastPlayerRemaining.AddListener(OnEndGame);
    }

    private void OnEndGame()
    {
        photonView.RPC(EndGame_RPC,RpcTarget.All);
    }
    
    [PunRPC]
    private void EndGame()
    {
       _myPlayerController.enabled = false;
    }

    #endregion
    
    #region MasterClient Handling

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        if (PhotonNetwork.IsMasterClient)
        {
            SetNextSpawnPoint();
            StartCoroutine(WaitTenSecondsAndSpawn(boostSpawners.Length));
        }

        base.OnMasterClientSwitched(newMasterClient);
        Debug.Log($"The new master client is {newMasterClient}");
    }

    [ContextMenu("Switch Master Client")]
    public void ChangeMasterClient()
    {
        Player MasterClientCandidate = PhotonNetwork.LocalPlayer.GetNext();

        bool success = PhotonNetwork.SetMasterClient(MasterClientCandidate);
        Debug.Log($"New Master Setting secces is {success}");
    }

    public void AssignNewManager(Player newManager)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.SetMasterClient(newManager);
        }
    }

    #endregion

    #region Player Join/Leave Callbacks

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        chat.NewChatMessage("System", $"{newPlayer.NickName} has joined the room", 1f, 1f, 0f);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log($"player has been disconnected. Player Active/Inactive {otherPlayer.IsInactive}");

        if (otherPlayer.IsMasterClient)
        {
            Debug.Log("player was the master client, changing master...");
            ChangeMasterClient();
        }
    }

    [ContextMenu("Disconnect")]
    public void Disconnect()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion
}