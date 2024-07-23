using Photon.Pun;
using TMPro;
using UnityEngine;

namespace DefaultNamespace.Chat
{
    public class ChatMessage : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI msgText;
        
        private string _playerNickname;
        private string _msg;
        private Color _color;
        
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] data = info.photonView.InstantiationData;

            _playerNickname = (string)data[0];
            _msg = (string)data[1];

            _color = new Color((float)data[2], (float)data[3], (float)data[4]);
            
            UpdateVisualElements();
        }

        void UpdateVisualElements()
        {
            nameText.text = $"[{_playerNickname}]: ";
            msgText.text = _msg;
            nameText.color = _color;
        }
    }
}