using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using InputSystemPlayerInput = UnityEngine.InputSystem.PlayerInput;

//Hub for gameplay input. Other scripts ask this class instead of reading devices themselves.
[RequireComponent(typeof(InputSystemPlayerInput))]
public class PlayerInput : Singleton<PlayerInput>
{
    private InputAction pointAction;
    private InputAction clickAction;

    private bool clickWasPressed;
    private bool clickIsPressed;
    private bool clickWasReleased;
    private bool clickWasPressedLastFrame;
    private int clickSampleFrame = -1;

    private int pointerQueryFrame = -1;
    private bool pointerHitValid;
    private Collider pointerCollider;

    private void Start()
    {
        //The Player Input component owns the InputSystem_Actions asset. CardGameplay is enabled here so Point and Click stay active alongside its default map.
        InputSystemPlayerInput input = GetComponent<InputSystemPlayerInput>();
        InputActionMap cardGameplay = input.actions.FindActionMap("CardGameplay", true);
        pointAction = cardGameplay.FindAction("Point", true);
        clickAction = cardGameplay.FindAction("Click", true);
        cardGameplay.Enable();
    }

    public Vector2 GetPointerScreenPosition()
    {
        if (pointAction.expectedControlType == "Vector2")
        {
            return pointAction.ReadValue<Vector2>();
        }

        return pointAction.ReadValue<Vector3>();
    }

    public bool WasClickPressedThisFrame()
    {
        SampleClick();
        return clickWasPressed;
    }

    public bool IsClickHeld()
    {
        SampleClick();
        return clickIsPressed;
    }

    public bool WasClickReleasedThisFrame()
    {
        SampleClick();
        return clickWasReleased;
    }

    //One raycast per frame, shared by every card.
    public bool TryGetColliderUnderPointer(out Collider collider)
    {
        if (pointerQueryFrame != Time.frameCount)
        {
            pointerQueryFrame = Time.frameCount;
            pointerHitValid = false;
            pointerCollider = null;

            Camera mainCamera = Camera.main;
            if (mainCamera != null && !IsPointerOverUI())
            {
                Ray ray = mainCamera.ScreenPointToRay(GetPointerScreenPosition());
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    pointerHitValid = true;
                    pointerCollider = hit.collider;
                }
            }
        }

        collider = pointerCollider;
        return pointerHitValid;
    }

    private void SampleClick()
    {
        if (clickSampleFrame == Time.frameCount)
        {
            return;
        }

        clickSampleFrame = Time.frameCount;
        //Click is a Pass Through button, so the held state comes from its value.
        bool pressed = clickAction.ReadValue<float>() > 0f;
        clickWasPressed = pressed && !clickWasPressedLastFrame;
        clickWasReleased = !pressed && clickWasPressedLastFrame;
        clickIsPressed = pressed;
        clickWasPressedLastFrame = pressed;
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        Pointer pointer = Pointer.current;
        if (pointer == null)
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        return EventSystem.current.IsPointerOverGameObject(pointer.deviceId);
    }
}
