using System;
using UnityEngine;

public class AddBlockTool : SelectionTool
{
    WorldPositionSelection worldPositionSelection;
    private Func<BlockType> GetBlockTypeAction;
    private Vector3 selectedWorldPosition;
    Transform indicator;
    float distance = 15;

    public AddBlockTool(WorldPositionSelection worldPositionSelection, Func<BlockType> GetBlockType,
        Transform indicator)
    {
        this.worldPositionSelection = worldPositionSelection;
        GetBlockTypeAction = GetBlockType;
        this.indicator = indicator;
    }

    public override void Tick(IInput input)
    {
        //Change distance depending on higherLowerInput
        
        var higherLowerInput = input.LowerHigherInput();
        if (Mathf.Abs(higherLowerInput) > 0.1)
        {
            distance += higherLowerInput * Time.deltaTime * 10;
            distance = Mathf.Clamp(distance, 0, 100);
        }

        selectedWorldPosition = worldPositionSelection.GetClosestHitPoint(distance);
        indicator.position = Vector3Int.RoundToInt(selectedWorldPosition);
        if (input.PointerClick())
        {
            InvokeFinnished(new AddBlockCommand(selectedWorldPosition, GetBlockTypeAction()));
        }
    }
}

public class BoxAddBlockTool : SelectionTool
{
    WorldPositionSelection worldPositionSelection;
    private Func<BlockType> GetBlockTypeAction;
    private Vector3? selectedFirstPosition;
    private Vector3 selectedSecondPosition;

    public BoxAddBlockTool(WorldPositionSelection worldPositionSelection, Func<BlockType> GetBlockType)
    {
        this.worldPositionSelection = worldPositionSelection;
        GetBlockTypeAction = GetBlockType;
    }

    public override void Tick(IInput input)
    {
        if (input.PointerClick())
        {
            if (selectedFirstPosition == null)
            {
                selectedFirstPosition = worldPositionSelection.GetClosestHitPoint(30);
            }
            else
            {
                selectedSecondPosition = worldPositionSelection.GetClosestHitPoint(30);
            }
        }
    }
}