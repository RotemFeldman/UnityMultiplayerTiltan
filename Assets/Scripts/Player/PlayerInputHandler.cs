using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [HideInInspector] public Vector3 movementInput;
    [HideInInspector] public UnityEvent onShootInput; 

    private Vector2 _workspace;
    public void OnMove(InputAction.CallbackContext context)
    {
        
        _workspace = context.ReadValue<Vector2>();
        movementInput.x = _workspace.x;
        movementInput.z = _workspace.y;
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            onShootInput?.Invoke();
        }
            
    }
}
