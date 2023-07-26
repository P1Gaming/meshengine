using MeshEngine.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all queries to the save system or the mesh engine.
/// </summary>
internal class RequestHandler : IRequestHandler
{
    /// <summary>
    /// Add the block at that position.
    /// </summary>
    /// <param name="blockToBeAdded">The block to place and the position to place it at.</param>
    /// <returns>If the block could successfully be placed.</returns>
    public bool AddBlockAtPosition(BlockTypeWithPosition blockToBeAdded)
    {
        if (IsBlockAtPosition(blockToBeAdded.Position))
        {
            return false;
        }

        ChunkData chunkDataToBeChanged = ResourceReferenceKeeper.GetResource<IChunkLoader>().GetChunkData(blockToBeAdded.Position);
        int x = blockToBeAdded.Position.x - chunkDataToBeChanged.position.x;
        int y = blockToBeAdded.Position.y - chunkDataToBeChanged.position.y;
        int z = blockToBeAdded.Position.z - chunkDataToBeChanged.position.z;
        var chunkData = chunkDataToBeChanged.Data;

        chunkData[x, y, z] = blockToBeAdded.BlockType;

        chunkDataToBeChanged.OverwriteBlockTypeData(chunkData, false);

        ResourceReferenceKeeper.GetResource<ISaveData>().SaveChunkData(chunkDataToBeChanged);

        return true;
    }

    /// <summary>
    /// Get the block type at the given position.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>The block type of the block at teh position.</returns>
    public BlockType GetBlockAtPosition(Vector3Int position)
    {
        ChunkData chunkDataToBeChanged = ResourceReferenceKeeper.GetResource<IChunkLoader>().GetChunkData(position);
        int x = position.x - chunkDataToBeChanged.position.x;
        int y = position.y - chunkDataToBeChanged.position.y;
        int z = position.z - chunkDataToBeChanged.position.z;
        var chunkData = chunkDataToBeChanged.Data;

        return chunkData[x, y, z];
    }

    /// <summary>
    /// Returns if there is a block at this position (Not Air)
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>If there was a bloc at this position.</returns>
    public bool IsBlockAtPosition(Vector3Int position)
    {
        ChunkData chunkDataToBeChanged = ResourceReferenceKeeper.GetResource<IChunkLoader>().GetChunkData(position);
        int x = position.x - chunkDataToBeChanged.position.x;
        int y = position.y - chunkDataToBeChanged.position.y;
        int z = position.z - chunkDataToBeChanged.position.z;
        var chunkData = chunkDataToBeChanged.Data;

        return chunkData[x, y, z] != BlockType.Air;
    }

    /// <summary>
    /// Remove the block at this position.
    /// </summary>
    /// <param name="position">The position to remove the block from.</param>
    /// <returns>The block that was removed.</returns>
    public BlockTypeWithPosition RemoveBlockAtPosition(Vector3Int position)
    {
        if (!IsBlockAtPosition(position))
        {
            return new BlockTypeWithPosition(BlockType.Air, position);
        }

        ChunkData chunkDataToBeChanged = ResourceReferenceKeeper.GetResource<IChunkLoader>().GetChunkData(position);
        int x = position.x - chunkDataToBeChanged.position.x;
        int y = position.y - chunkDataToBeChanged.position.y;
        int z = position.z - chunkDataToBeChanged.position.z;
        var chunkData = chunkDataToBeChanged.Data;

        var blockDrop = chunkData[x, y, z];
        chunkData[x, y, z] = BlockType.Air;

        chunkDataToBeChanged.OverwriteBlockTypeData(chunkData, false);

        ResourceReferenceKeeper.GetResource<ISaveData>().SaveChunkData(chunkDataToBeChanged);

        return new BlockTypeWithPosition(blockDrop, position);
    }
}
