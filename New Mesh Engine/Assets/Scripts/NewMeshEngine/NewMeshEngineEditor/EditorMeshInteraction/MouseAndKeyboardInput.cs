using UnityEngine;

public class MouseAndKeyboardInput : IInput
{
    public bool Cancel()
    {
        return Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape);
    }

    public bool PointerClick()
    {
        return Input.GetMouseButtonDown(0);
    }

    public float LowerHigherInput()
    {
        // return scrollwheel input
        return Input.mouseScrollDelta.y;
    }
}