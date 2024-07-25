using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Chat
{
    public class ChatInputHandler : MonoBehaviour
    {
        
        [HideInInspector] public UnityEvent onEnterPressed;
        
        [HideInInspector] public UnityEvent onEscapePressed;
        
        public void OnEnterKey(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                onEnterPressed?.Invoke();
            }
        }

        public void OnEscapeKey(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                onEscapePressed?.Invoke();
            }
        }
    }
}