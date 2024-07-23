using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [HideInInspector] public Vector3 movementInput;

    public UnityAction OnShootInput;
    private UnityEvent _onShootInput;

    public UnityAction OnEnterPressed;
    private UnityEvent _onEnterPressed;

    public UnityAction OnEscapePressed;
    private UnityEvent _onEscapePressed;

    private void OnEnable()
    {
        _onShootInput.AddListener(OnShootInput);
        _onEnterPressed.AddListener(OnEnterPressed);
        _onEscapePressed.AddListener(OnEscapePressed);
    }

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
            _onShootInput?.Invoke();
        }
            
    }

    public void OnEnterKey(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _onEnterPressed?.Invoke();
        }
    }

    public void OnEscapeKey(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _onEscapePressed?.Invoke();
        }
    }
}
