using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputReader : SingletonPersistent<InputReader>, GameInputs.IPlayerActions
{
    public GameInputs GameInputs { get; private set; }
    public Vector2 Movement => GameInputs.Player.Move.ReadValue<Vector2>();
    public Vector2 LookDelta => GameInputs.Player.Look.ReadValue<Vector2>();


    public delegate void InputCallbackWithContextData(InputAction.CallbackContext context);

    public event InputCallbackWithContextData OnAttackEvent;
    public event InputCallbackWithContextData OnInteractEvent;
    public event InputCallbackWithContextData OnMouseLookEvent;
    public event InputCallbackWithContextData OnCrouchEvent;
    public event InputCallbackWithContextData OnJumpEvent;
    public event InputCallbackWithContextData OnSprintEvent;

    private void OnEnable()
    {
        if (GameInputs != null)
        {
            GameInputs.Enable();
        }
        else
        {
            GameInputs = new GameInputs();

            GameInputs.Player.SetCallbacks(this);
            GameInputs.Enable();
        }
    }
    private void OnDisable()
    {
        if (GameInputs != null)
        {
            GameInputs.Player.RemoveCallbacks(this);
            GameInputs.Disable();
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {

    }

    public void OnLook(InputAction.CallbackContext context)
    {
        OnMouseLookEvent?.Invoke(context);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        OnAttackEvent?.Invoke(context);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        OnInteractEvent?.Invoke(context);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        OnCrouchEvent?.Invoke(context);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        OnJumpEvent?.Invoke(context);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        OnSprintEvent?.Invoke(context);
    }
}
