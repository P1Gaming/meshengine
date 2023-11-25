using System.Collections.Generic;
using UnityEngine;

public class AddBlocksCommand : ICommand
{
    private string commandName;
    private List<Vector3Int> positions;
    private BlockType blockType;

    private List<BlockTypeWithPosition> previousBlockTypes = new List<BlockTypeWithPosition>();
    public AddBlocksCommand(List<Vector3Int> positions, BlockType blockType, string commandName = "Add blocks")
    {
        this.positions = positions;
        this.blockType = blockType;
        this.commandName = commandName;
    }
    
    public bool TryExecute()
    {
        var me = ResourceReferenceKeeper.GetResource<IMeshEngine>();
        List<BlockTypeWithPosition> newBlocks = new();

        foreach (var position in positions)
        {
            previousBlockTypes.Add(new BlockTypeWithPosition(me.GetBlockType(position),position));
            newBlocks.Add(new BlockTypeWithPosition(blockType,position));
        }

        List<bool> successes = me.AddBlocks(newBlocks);

        for (int i = 0; i < successes.Count; i++)
        {
            if (successes[i])
            {
                return true;
            }
        }
        
        return false;
    }

    public void Undo()
    {
        var me = ResourceReferenceKeeper.GetResource<IMeshEngine>();
        me.TryRemoveBlocks(positions, out List<BlockTypeWithPosition> removedBlocks);
        me.AddBlocks(previousBlockTypes);
    }
    
    public string GetCommandName() => commandName;
}