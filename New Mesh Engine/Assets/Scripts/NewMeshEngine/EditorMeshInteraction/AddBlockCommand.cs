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
        return true;
        //return ResourceReferenceKeeper.GetResource<MeshEngineHandler>().TryAddBlock(position, blockType);
    }

    public void Undo()
    {
        //ResourceReferenceKeeper.GetResource<MeshEngineHandler>().TryAddBlock(position, previousBlockType);
    }
    public string GetCommandName() => "Add block";
}

