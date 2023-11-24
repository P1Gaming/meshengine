using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AddBlockBoxCommand : ICommand
{
    //Vector3 minPos;
    //Vector3 maxPos;
    BlockType blockType;
    BlockType[,,] previousBlockTypes;
    private List<BlockTypeWithPosition> previousBlockTypesWithPosition = new List<BlockTypeWithPosition>();
    BoundsInt bounds;

    public AddBlockBoxCommand(Vector3 minPos, Vector3 maxPos, BlockType blockType)
    {
        bounds = new BoundsInt();
        bounds.min = Vector3Int.RoundToInt(minPos);
        bounds.max = Vector3Int.RoundToInt(maxPos);
        this.blockType = blockType;
    }

    public string GetCommandName()
    {
        return "Block box";
    }

    public bool TryExecute()
    {
        if (!WorldInfo.IsPositionInsideWorld(bounds.min) && !WorldInfo.IsPositionInsideWorld(bounds.max))
        {
            return false;
        }

        previousBlockTypes = new BlockType[bounds.size.x, bounds.size.y, bounds.size.z];
        Vector3Int minPos = Vector3Int.RoundToInt(bounds.min);
        List<BlockTypeWithPosition> blockTypeWithPositions = new();

        var me = ResourceReferenceKeeper.GetResource<IMeshEngine>();
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                for (int z = 0; z < bounds.size.z; z++)
                {
                    Vector3Int position = new Vector3Int(x, y, z) + minPos;
                    previousBlockTypesWithPosition.Add(new BlockTypeWithPosition(me.GetBlockType(position), position));
                    //previousBlockTypes[x, y, z] = me.GetBlockType(position);
                    blockTypeWithPositions.Add(new BlockTypeWithPosition(blockType, position));
                    //ResourceReferenceKeeper.GetResource<IMeshEngine>().TryAddBlock(Vector3Int.RoundToInt(position), blockType);
                }
            }
        }

        me.AddBlocks(blockTypeWithPositions);
        return true;
    }

    public void Undo()
    {
        Vector3Int minPos = Vector3Int.RoundToInt(bounds.min);
        var me = ResourceReferenceKeeper.GetResource<IMeshEngine>();
        List<Vector3Int> blockPositionsToRemove = new();
        foreach (var BlockType in previousBlockTypesWithPosition)
        {
            blockPositionsToRemove.Add(BlockType.Position);
        }

        me.TryRemoveBlocks(blockPositionsToRemove,out List<BlockTypeWithPosition> blockTypeAtPosition);
        me.AddBlocks(previousBlockTypesWithPosition);
    }

}
