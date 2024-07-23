using Photon.Pun;
using UnityEngine;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject chatWindow;
    private const string ChatMessagePrefab = "Resources/Prefabs/Chat Message";

    private const string ChatMessage_RPC = nameof(NewChatMessage);

    public void SendMessage(string chatterName, string msg, Color color)
    {
        photonView.RPC(ChatMessage_RPC,RpcTarget.All,chatterName,msg,color.r,color.g,color.b);
    }

    [PunRPC]
    private void NewChatMessage(string chatterName, string msg, float colorR,float colorG,float colorB)
    {
        object[] data = new object[] { chatterName, msg,colorR,colorG,colorB};
        var go = PhotonNetwork.Instantiate(ChatMessagePrefab, Vector3.zero, Quaternion.identity,0,data);
        go.transform.SetParent(chatWindow.transform);
    }
}
