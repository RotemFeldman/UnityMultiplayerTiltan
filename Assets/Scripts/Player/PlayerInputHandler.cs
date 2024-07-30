using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [HideInInspector] public Vector3 movementInput;
    [HideInInspector] public Vector3 rotateInput;
    
    [HideInInspector] public UnityEvent onShootInput;

    

    private Vector2 _workspaceMove;
    public void OnMove(InputAction.CallbackContext context)
    {
        
        _workspaceMove = context.ReadValue<Vector2>();
        movementInput.x = _workspaceMove.x;
        movementInput.z = _workspaceMove.y;
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            onShootInput?.Invoke();
        }
            
    }
    

   
}
