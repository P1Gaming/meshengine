using UnityEngine;

public class MouseAndKeyboardInput : IInput
{
    private WorldPositionSelection worldPositionSelection;

    public MouseAndKeyboardInput(WorldPositionSelection worldPositionSelection)
    {
        this.worldPositionSelection = worldPositionSelection;
    }

    public Vector3Int GetPointerGridPositionPosition(bool showIndicator)
    {
        return worldPositionSelection.GetClosestHitPoint();
    }

    public bool Cancel()
    {
        return Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape);
    }

    public bool PointerClick()
    {
        return Input.GetMouseButtonDown(0);
    }

    public float IncreaseOrDecrease()
    {
        float value = Input.mouseScrollDelta.y;
        worldPositionSelection.ChangeDistance(value);
        return value;
    }
}