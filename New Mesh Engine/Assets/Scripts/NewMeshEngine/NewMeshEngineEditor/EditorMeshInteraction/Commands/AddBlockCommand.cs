using UnityEngine;
using UnityEditor;

public class AddBlockCommand : ICommand
{
    private Vector3 position;
    private BlockType blockType;

    private BlockType previousBlockType;
    public AddBlockCommand(Vector3 position, BlockType blockType)
    {
        this.position = position;
        this.blockType = blockType;
    }
    
    public bool TryExecute()
    {
        previousBlockType = ResourceReferenceKeeper.GetResource<IMeshEngine>().GetBlockType(Vector3Int.RoundToInt(position));
        if (ResourceReferenceKeeper.GetResource<IMeshEngine>().TryAddBlock(Vector3Int.RoundToInt(position), blockType))
        {
            return true;
        }
        return false;
        
    }

    public void Undo()
    {
        ResourceReferenceKeeper.GetResource<IMeshEngine>().TryRemoveBlock(Vector3Int.RoundToInt(position));
        ResourceReferenceKeeper.GetResource<IMeshEngine>().TryAddBlock(Vector3Int.RoundToInt(position), previousBlockType);
    }
    public string GetCommandName() => "Add block";
}


public class NoCommand : ICommand
{
    public string GetCommandName()
    {
        return "No Action";
    }

    public bool TryExecute()
    {
        return true;
    }

    public void Undo()
    {
        
    }
}

