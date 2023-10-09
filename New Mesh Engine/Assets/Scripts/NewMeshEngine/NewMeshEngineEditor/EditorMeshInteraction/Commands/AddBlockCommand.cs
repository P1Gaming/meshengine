using UnityEngine;
using UnityEditor;

public class AddBlockCommand : ICommand
{
    private Vector3Int position;
    private BlockType blockType;

    private BlockType previousBlockType;
    public AddBlockCommand(Vector3Int position, BlockType blockType)
    {
        this.position = position;
        this.blockType = blockType;
    }
    public bool TryExecute()
    {
        previousBlockType = ResourceReferenceKeeper.GetResource<IMeshEngine>().GetBlockType(position);
        if (ResourceReferenceKeeper.GetResource<IMeshEngine>().TryAddBlock(position, blockType))
        {
            return true;
        }
        return false;
        
    }

    public void Undo()
    {
        ResourceReferenceKeeper.GetResource<MeshEngineHandler>().TryAddBlock(position, previousBlockType);
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

