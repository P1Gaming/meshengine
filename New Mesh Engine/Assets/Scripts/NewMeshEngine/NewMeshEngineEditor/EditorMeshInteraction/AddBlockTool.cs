using System;
using UnityEngine;

public class AddBlockTool : SelectionTool
{
    WorldPositionSelection worldPositionSelection;
    private Func<BlockType> GetBlockTypeAction;
    private Vector3Int selectedWorldPosition;

    public AddBlockTool(WorldPositionSelection worldPositionSelection, Func<BlockType> GetBlockType)
    {
        this.worldPositionSelection = worldPositionSelection;
        GetBlockTypeAction = GetBlockType;
    }

    public override ICommand GetResults()
    {
        return new AddBlockCommand(selectedWorldPosition, GetBlockTypeAction());
    }

    
    public override void Tick(IInput input)
    {
        if(input.PointerClick())
        {
            selectedWorldPosition = worldPositionSelection.GetClosestHitPoint(30);
            InvokeFinnished(new SelectionToolsEventArgs( SelectionToolsEventArgs.SelectionResult.Completed));
        }
        
    }
}

public class BoxAddBlockTool : SelectionTool
{
    WorldPositionSelection worldPositionSelection;
    private Func<BlockType> GetBlockTypeAction;
    private Vector3Int? selectedFirstPosition;
    private Vector3Int selectedSecondPosition;

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
