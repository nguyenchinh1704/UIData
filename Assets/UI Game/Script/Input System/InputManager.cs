using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>
{
    private TouchScreen touchScreen;

    public delegate void StartMoveEvent(Vector2 position, float time);
    public event StartMoveEvent OnStartMove;
    public delegate void EndMoveEvent(Vector2 position, float time);
    public event EndMoveEvent OnEndMove;


    private void Awake()
    {
        touchScreen = new TouchScreen();
    }
    private void OnEnable()
    {
        touchScreen.Enable();
        TouchSimulation.Enable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
    }
    private void OnDisable()
    {
        touchScreen.Disable();
        TouchSimulation.Disable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    private void Start()
    {
        touchScreen.Move.TouchPress.started += ctx => StartMove(ctx);
        touchScreen.Move.TouchPress.canceled += ctx => EndMove(ctx);
    }

    private void StartMove(InputAction.CallbackContext context)
    {
        if (OnStartMove != null) OnStartMove(touchScreen.Move.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
    }

    private void EndMove(InputAction.CallbackContext context)
    {
        if (OnEndMove != null) OnEndMove(touchScreen.Move.TouchPosition.ReadValue<Vector2>(), (float)context.time);
    }

    private void FingerDown(Finger finger)
    {
        if (OnStartMove != null) OnStartMove(finger.screenPosition, Time.time);
    }
}
