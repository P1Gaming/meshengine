using System;
using UnityEngine;

public class AddBlockTool : SelectionTool
{
    WorldPositionSelection worldPositionSelection;
    private Func<BlockType> GetBlockTypeAction;
    private Vector3 selectedWorldPosition;
    Transform indicator;

    public AddBlockTool(WorldPositionSelection worldPositionSelection, Func<BlockType> GetBlockType, Transform indicator)
    {
        this.worldPositionSelection = worldPositionSelection;
        GetBlockTypeAction = GetBlockType;
        this.indicator = indicator;
    }

    public override ICommand GetResults()
    {
        return new AddBlockCommand(selectedWorldPosition, GetBlockTypeAction());
    }


    public override void Tick(IInput input)
    {
        selectedWorldPosition = worldPositionSelection.GetClosestHitPoint(30);
        indicator.position = Vector3Int.RoundToInt(selectedWorldPosition);
        if (input.PointerClick())
        {

            InvokeFinnished(new SelectionToolsEventArgs(SelectionToolsEventArgs.SelectionResult.Completed));
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

    public override ICommand GetResults()
    {
        throw new NotImplementedException();
    }
}
