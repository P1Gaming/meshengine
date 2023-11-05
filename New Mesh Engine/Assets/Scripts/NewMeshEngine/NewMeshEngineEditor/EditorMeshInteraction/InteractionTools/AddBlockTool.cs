using System;
using UnityEngine;

public class AddBlockTool : SelectionTool
{
    Vector3 selectedWorldPosition;
    private Func<BlockType> GetBlockTypeAction;
    float distance = 30;

    public AddBlockTool(Func<BlockType> GetBlockType)
    {
        GetBlockTypeAction = GetBlockType;
    }

    public override void Tick(IInput input)
    {
        if (input.PointerClick())
        {
            selectedWorldPosition = input.GetPointerPosition();
            InvokeFinnished(new AddBlockCommand(selectedWorldPosition, GetBlockTypeAction()));
        }
    }
}