using TMPro;
using UnityEngine;

namespace Chat
{
    public class ChatMessage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI msgText;
        
        public void InitMessage(string playerName, string msg, params float[] colorValues)
        {
            nameText.text = $"[{playerName}]: ";
            msgText.text = msg;

            Color c = new Color(colorValues[0], colorValues[1], colorValues[2]);
            nameText.color = c;
        }
    }
}