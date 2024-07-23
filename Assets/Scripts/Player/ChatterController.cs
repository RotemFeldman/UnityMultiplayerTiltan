using System;
using Photon.Pun;
using UnityEngine;
using TMPro;

namespace DefaultNamespace.Player
{
    public class ChatterController : MonoBehaviour
    {
        [SerializeField] private GameObject chatWindow;
        [SerializeField] private TMP_InputField inputField;

        [SerializeField] private PlayerInputHandler inputHandler;
        [SerializeField] private ChatManager chatManager;

        public Color PlayerColor;

        private void Start()
        {
           chatWindow.SetActive(false);
        }

        void EnterPressed()
        {
            if (!chatWindow.activeSelf)
            {
                chatWindow.SetActive(true);
            }
            else
            {
                if (inputField.text.Length > 0)
                {
                    chatManager.SendMessage(PhotonNetwork.NickName,inputField.text,PlayerColor);
                }
            }
        }

    }
}