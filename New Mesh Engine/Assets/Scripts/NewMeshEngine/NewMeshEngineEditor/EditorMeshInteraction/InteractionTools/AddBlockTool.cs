using System;
using UnityEngine;

public class AddBlockTool : SelectionTool
{
    Vector3Int selectedWorldPosition;
    private Func<BlockType> getBlockTypeAction;

    public AddBlockTool(Func<BlockType> getBlockType)
    {
        getBlockTypeAction = getBlockType;
    }

    public override void Tick(IInput input)
    {
        if (input.PointerClick())
        {
            selectedWorldPosition = input.GetPointerGridPositionPosition(false);
            InvokeFinnished(new AddBlockCommand(selectedWorldPosition, getBlockTypeAction()));
        }
    }
}