using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    /* Events Triggered By Player Input */
    public static Action<Vector2> moveAction;
    public static Action<Vector2> lookAction;
    public static Action<bool> jumpAction;
    public static Action<bool> sprintAction;
    public static Action menuAction;
    public static Action creativeModeAction;
    public static Action nameSlimeAction;
    public static Action noClipAction;
    public static Action openDebugScreenAction;
    public static Action<float> scrollInventoryAction;


    /* ***
     * New controls can be set up in the InputSettings object. Each one should then fire an Action set up on this script, 
     * which can then be subscribed to by interested parties.
     * ***/

    public void OnMove(InputAction.CallbackContext context)
    {
        moveAction?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookAction?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumpAction?.Invoke(context.performed);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        sprintAction?.Invoke(!context.canceled);
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
            menuAction?.Invoke();
    }

    public void OnCreativeMode(InputAction.CallbackContext context)
    {
        if (context.performed)
            creativeModeAction?.Invoke();
    }

    public void OnNameSlime(InputAction.CallbackContext context)
    {
        if (context.performed)
            nameSlimeAction?.Invoke();
    }

    public void OnNoClip(InputAction.CallbackContext context)
    {
        if (context.performed)
            noClipAction?.Invoke();
    }

    public void OnDebugScreen(InputAction.CallbackContext context)
    {
        if (context.performed)
            openDebugScreenAction?.Invoke();
    }

    public void OnScrollInventory(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        if (value != 0)
            scrollInventoryAction?.Invoke(value);
    }
}

