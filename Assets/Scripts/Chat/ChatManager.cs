using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Chat
{
    public class ChatManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject chatWindow;
        [SerializeField] private GameObject chatMessagePrefab;

        private const string ChatMessage_RPC = nameof(NewChatMessage);

        [SerializeField] private Transform messageHolder;
        [SerializeField] private TMP_InputField inputField;

        [SerializeField] private ChatInputHandler inputHandler;
        [HideInInspector] public Color playerColor;
        [HideInInspector] public PlayerController playerController;

        private void Start()
        {
            chatWindow.SetActive(false);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            inputHandler.onEnterPressed.AddListener(EnterPressed);
            inputHandler.onEscapePressed.AddListener(EscapePressed);
        }
        
        public override void OnDisable()
        {
            base.OnEnable();
            inputHandler.onEnterPressed.RemoveListener(EnterPressed);
            inputHandler.onEscapePressed.RemoveListener(EscapePressed);
        }

        private void EscapePressed()
        {

            if (chatWindow.activeSelf)
            {
                Debug.Log("closed chat");
                chatWindow.SetActive(false);
                playerController.enabled = true;
            }
        }

        void EnterPressed()
        {
            Debug.Log("chat window " +chatWindow.activeSelf);
            
            if (!chatWindow.activeSelf)
            {
                Debug.Log("open chat");
                chatWindow.SetActive(true);
                inputField.ActivateInputField();
                playerController.enabled = false;
            }
            else
            {
                Debug.Log("no message to send");
                if (inputField.text.Length > 0)
                {
                    Debug.Log("sending message");
                    SendMessage(PhotonNetwork.NickName,inputField.text,playerColor);
                    inputField.ActivateInputField();
                    inputField.text = string.Empty;
                }
            }
        }
    
        private void SendMessage(string chatterName, string msg, Color color)
        {
            photonView.RPC(ChatMessage_RPC,RpcTarget.All,chatterName,msg,color.r,color.g,color.b);
        }

        [PunRPC]
        private void NewChatMessage(string chatterName, string msg, float colorR,float colorG,float colorB)
        {
            Debug.Log(chatterName+ ": " + msg);
            var go = Instantiate(chatMessagePrefab, Vector3.zero, Quaternion.identity,messageHolder);
            go.GetComponent<ChatMessage>().InitMessage(chatterName,msg,colorR,colorG,colorB);
        }
    }
}
