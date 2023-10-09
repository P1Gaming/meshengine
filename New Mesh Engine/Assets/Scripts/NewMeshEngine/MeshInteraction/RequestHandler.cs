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

        ChunkData chunkDataToBeChanged =
            ResourceReferenceKeeper.GetResource<IChunkLoader>().GetChunkData(blockToBeAdded.Position);
        int x = blockToBeAdded.Position.x - (chunkDataToBeChanged.position.x * WorldInfo.ChunkDimensions.x);
        int y = blockToBeAdded.Position.y - chunkDataToBeChanged.position.y * WorldInfo.ChunkDimensions.y;
        int z = blockToBeAdded.Position.z - chunkDataToBeChanged.position.z * WorldInfo.ChunkDimensions.z;
        var chunkData = chunkDataToBeChanged.Data;

        Debug.Log("Placed Block at: " + blockToBeAdded.Position);

        chunkData[x, y, z] = blockToBeAdded.BlockType;

        chunkDataToBeChanged.AddBlockAtIndex(new Vector3Int(x, y, z), blockToBeAdded.BlockType);

        ResourceReferenceKeeper.GetResource<ISaveData>().SaveChunkData(chunkDataToBeChanged);

        Debug.Log("Added " + blockToBeAdded.BlockType.ToString() + " at position " + blockToBeAdded.Position);

        return true;
    }

    /// <summary>
    /// Get the block type at the given position.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>The block type of the block at teh position.</returns>
    public BlockType GetBlockAtPosition(Vector3Int position)
    {
        BlockType result = BlockType.Air;
        ChunkData chunkDataToBeChanged = ResourceReferenceKeeper.GetResource<IChunkLoader>().GetChunkData(position);
        if (chunkDataToBeChanged != null)
        {
            int x = position.x - (chunkDataToBeChanged.position.x * WorldInfo.ChunkDimensions.x);
            int y = position.y - (chunkDataToBeChanged.position.y * WorldInfo.ChunkDimensions.y);
            int z = position.z - (chunkDataToBeChanged.position.z * WorldInfo.ChunkDimensions.z);
            var chunkData = chunkDataToBeChanged.Data;

            result = chunkData[x, y, z];
            Debug.Log("Block at position: " + position +" is " +result);
        }

        return result;
        //return chunkData[x, y, z];
    }

    /// <summary>
    /// Returns if there is a block at this position (Not Air)
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>If there was a bloc at this position.</returns>
    public bool IsBlockAtPosition(Vector3Int position)
    {
        ChunkData chunkDataToBeChanged = ResourceReferenceKeeper.GetResource<IChunkLoader>().GetChunkData(position);
        bool result = false;
        if (chunkDataToBeChanged != null)
        {
            int x = position.x - (chunkDataToBeChanged.position.x * WorldInfo.ChunkDimensions.x);
            int y = position.y - (chunkDataToBeChanged.position.y * WorldInfo.ChunkDimensions.y);
            int z = position.z - (chunkDataToBeChanged.position.z * WorldInfo.ChunkDimensions.z);
            var chunkData = chunkDataToBeChanged.Data;
            result = chunkData[x, y, z] == BlockType.Air ? false : true;
        }
        Debug.Log("There is a block at " + position + "?");

        return result;
        //return chunkData[x, y, z] != BlockType.Air;
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

        Debug.Log("Remove Block at position: " + position);

        return new BlockTypeWithPosition(BlockType.Air, position);
        //return new BlockTypeWithPosition(blockDrop, position);
    }
}