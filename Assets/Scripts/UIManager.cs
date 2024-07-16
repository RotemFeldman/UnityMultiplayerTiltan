using System;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class UIManager : MonoBehaviourPunCallbacks
{

    public static UIManager Instance;
    
    // connect server
    [Header("UI")]
    [SerializeField] private GameObject _connectScreen;
    [SerializeField] public TMP_InputField NickNameInputField;
    
    // join lobby
    [Space(10)]
    [SerializeField] private GameObject _joinLobbyScreen;
    [SerializeField] public TMP_InputField LobbyInputField;
    
    // join room
    [Space(10)]
    [SerializeField] private GameObject _joinRoomScreen;
    [SerializeField] public TMP_InputField RoomNameInputField;
    [SerializeField] public TMP_InputField RoomMaxPlayersInputField;
    
    // in room
    [Space(10)]
    [SerializeField] private GameObject _inRoomScreen;
    [SerializeField] private Button _startGameButton;
    
    [Space(10)]
    [SerializeField] private TextMeshProUGUI _debugPhotonText;
    

    public enum UIScreen
    {
        Connect,
        JoinLobby,
        JoinRoom,
        InRoom
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
       
        SwitchUIScreen(UIScreen.Connect);

        if (!PhotonNetwork.IsMasterClient)
        {
            _startGameButton.enabled = false;
        }
        
    }
    
    private void Update()
    {
        _debugPhotonText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public void SwitchUIScreen(UIScreen screen)
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
                _startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
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
