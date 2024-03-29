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

        if (!WorldInfo.IsPositionInsideWorld(blockToBeAdded.Position))
        {
            return false;
        }

        ChunkData chunkDataToBeChanged = ResourceReferenceKeeper.GetResource<IChunkLoader>().GetChunkData(blockToBeAdded.Position);
        Vector3Int positionInChunk = WorldInfo.WorldPositionToPositionInChunk(blockToBeAdded.Position);

        chunkDataToBeChanged.AddBlockAtIndex(positionInChunk, blockToBeAdded.BlockType);

        ResourceReferenceKeeper.GetResource<ISaveData>().SaveChunkData(chunkDataToBeChanged);
        ResourceReferenceKeeper.GetResource<IMeshGenerator>().ModifyMesh(chunkDataToBeChanged);

        return true;
    }

    public void OverWriteBlockAtPosition(BlockTypeWithPosition blockToBeAdded)
    {
        ChunkData chunkDataToBeChanged = ResourceReferenceKeeper.GetResource<IChunkLoader>().GetChunkData(blockToBeAdded.Position);
        Vector3Int positionInChunk = WorldInfo.WorldPositionToPositionInChunk(blockToBeAdded.Position);
        
        chunkDataToBeChanged.AddBlockAtIndex(positionInChunk, blockToBeAdded.BlockType);
        
        ResourceReferenceKeeper.GetResource<ISaveData>().SaveChunkData(chunkDataToBeChanged);
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
        if (!WorldInfo.IsPositionInsideWorld(position))
        {
            return false;
        }
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

        return result;
    }

    /// <summary>
    /// Remove the block at this position.
    /// </summary>
    /// <param name="position">The position to remove the block from.</param>
    /// <returns>The block that was removed.</returns>
    public BlockTypeWithPosition RemoveBlockAtPosition(Vector3Int position)
    {
        if (!WorldInfo.IsPositionInsideWorld(position))
        {
            return null;
        }
        if (!IsBlockAtPosition(position))
        {
            return new BlockTypeWithPosition(BlockType.Air, position);
        }
        
        
        ChunkData chunkDataToBeChanged = ResourceReferenceKeeper.GetResource<IChunkLoader>().GetChunkData(position);
        Vector3Int posInChunk = WorldInfo.WorldPositionToPositionInChunk(position);
        
        var chunkData = chunkDataToBeChanged.Data;

        var blockDrop = chunkData[posInChunk.x, posInChunk.y, posInChunk.z];
        chunkData[posInChunk.x, posInChunk.y, posInChunk.z] = BlockType.Air;

        chunkDataToBeChanged.OverwriteBlockTypeData(chunkData, false);
        

        ResourceReferenceKeeper.GetResource<ISaveData>().SaveChunkData(chunkDataToBeChanged);

        ResourceReferenceKeeper.GetResource<IMeshGenerator>().ModifyMesh(chunkDataToBeChanged);
        
        return new BlockTypeWithPosition(BlockType.Air, position);
    }

    public BlockTypeWithPosition RemoveBlockAtPositionNoMeshReload(Vector3Int position)
    {
        if (!WorldInfo.IsPositionInsideWorld(position))
        {
            return null;
        }
        if (!IsBlockAtPosition(position))
        {
            return new BlockTypeWithPosition(BlockType.Air, position);
        }

        return new BlockTypeWithPosition(BlockType.Air, position);
    }

    /// <summary>
    /// Add a block into save data but dont reload the mesh.
    /// </summary>
    /// <param name="blockToBeAdded">The block and position to be added.</param>
    /// <returns></returns>
    public bool AddBlockAtPositionNoMeshReload(BlockTypeWithPosition blockToBeAdded)
    {
        if (IsBlockAtPosition(blockToBeAdded.Position))
        {
            return false;
        }

        if (!WorldInfo.IsPositionInsideWorld(blockToBeAdded.Position))
        {
            return false;
        }

        return true;
    }
}