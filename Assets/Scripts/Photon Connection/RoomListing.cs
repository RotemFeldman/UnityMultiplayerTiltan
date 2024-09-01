using Photon_Connection;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

namespace DefaultNamespace
{
    public class RoomListing : MonoBehaviour
    {
        [HideInInspector] public RoomInfo Info;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _players;

        public void SetRoomInfo(RoomInfo roomInfo)
        {
            Info = roomInfo;
            UpdateRoomInfo();
        }

        private void UpdateRoomInfo()
        {
            _name.text = Info.Name;
            _players.text = Info.PlayerCount.ToString() + "/" + Info.MaxPlayers.ToString();
        }
        
        public void JoinRoom()
        {
            MainMenuConnectionManager.JoinRoom(Info.Name);
        }
    }
}