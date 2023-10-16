using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshEngineHandler : IMeshEngine
{
    internal IRequestHandler _requestHandler;

    public static Action<Bounds> OnChunkLoaded;
    public static Action<Bounds> OnChunkUnloaded;
    public static Action OnWorldLoaded;

    /// <summary>
    /// Add the given blocks to their assigned positions.
    /// </summary>
    /// <param name="blocksToAdd">Blocks to add at the position.</param>
    public List<bool> AddBlocks(List<BlockTypeWithPosition> blocksToAdd)
    {
        List<bool> successfullyAdded = new List<bool>();

        foreach(BlockTypeWithPosition blocksWithPosition in blocksToAdd)
        {
            successfullyAdded.Add(TryAddBlock(blocksWithPosition));
        }

        return successfullyAdded;
    }

    public BlockType GetBlockType(Vector3Int position)
    {
        return ResourceReferenceKeeper.GetResource<IRequestHandler>().GetBlockAtPosition(position);
    }

    public bool IsBlockAtPosition(Vector3Int position)
    {
        return ResourceReferenceKeeper.GetResource<IRequestHandler>().IsBlockAtPosition(position);
    }

    /// <summary>
    /// Remove from the mesh/save the blocks at the given positions.
    /// </summary>
    /// <param name="positionOfBlocksToRemove">The positions to remove the blocks from.</param>
    /// <param name="blocks">The Blocks that were removed</param>
    /// <returns></returns>
    public List<bool> TryRemoveBlocks(List<Vector3Int> positionOfBlocksToRemove, out List<BlockTypeWithPosition> blocks)
    {
        List<bool> successfulBlockRemovals = new List<bool>();
        List<BlockTypeWithPosition> blocksRemoved = new List<BlockTypeWithPosition>();

        foreach(Vector3Int position in positionOfBlocksToRemove)
        {
            bool wasRemoved = TryRemoveBlock(position, out BlockTypeWithPosition removedBlock);
            successfulBlockRemovals.Add(wasRemoved);
            if(wasRemoved)
                blocksRemoved.Add(removedBlock);
            else
                blocksRemoved.Add(new BlockTypeWithPosition(BlockType.Air, position));
        }

        blocks = blocksRemoved;
        return successfulBlockRemovals;
    }

    /// <summary>
    /// Add block to the mesh/save at position given.
    /// </summary>
    /// <param name="position">The location to add the block at.</param>
    /// <param name="blockType">The block to add.</param>
    /// <returns></returns>
    public bool TryAddBlock(Vector3Int position, BlockType blockType) => TryAddBlock(new BlockTypeWithPosition(blockType, position));

    /// <summary>
    /// Add block to the mesh/save at position given.
    /// </summary>
    /// <param name="block">The block with position to be added</param>
    /// <returns>If the block was succesfully added.</returns>
    public bool TryAddBlock(BlockTypeWithPosition block)
    {
        return ResourceReferenceKeeper.GetResource<IRequestHandler>().AddBlockAtPosition(block);
    }
    
    public bool TryRemoveBlock(Vector3Int position) => TryRemoveBlock(position, out BlockTypeWithPosition removed);

    public bool TryRemoveBlock(Vector3Int position, out BlockTypeWithPosition removedType)
    {
        removedType = ResourceReferenceKeeper.GetResource<IRequestHandler>().RemoveBlockAtPosition(position);

        return removedType.BlockType != BlockType.Air;
    }
}
