using System;
using System.Collections.Generic;
using System.Linq;
using MeshEngine.SaveSystem;
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
        IChunkLoader chunkLoader = ResourceReferenceKeeper.GetResource<IChunkLoader>();
        List<ChunkData> chunks=new List<ChunkData>();
        //assumption: chunks change based on the block position each time we make a modification
        bool foundFlag = false;
        foreach(BlockTypeWithPosition blockWithPosition in blocksToAdd)
        {
            successfullyAdded.Add(TryAddBlock(blockWithPosition));
            ChunkData chunkData = chunkLoader.GetChunkData(blockWithPosition.Position);
            Vector3Int positionInChunk = WorldInfo.WorldPositionToPositionInChunk(blockWithPosition.Position); 
            chunkData.AddBlockAtIndex(positionInChunk, blockWithPosition.BlockType); 

            for (int i=0;i<chunks.Count;i++) 
            { 
                foundFlag = false; 
                if (chunks[i].position == chunkData.position) 
                { 
                    chunks[i]=chunkData; 
                    foundFlag = true; 
                    break;
                }
            } 
            if (!foundFlag) 
            { 
                chunks.Add(chunkData);
            }
        }
        //save and modify mesh here
        foreach (ChunkData chunk in chunks)
        {
            ResourceReferenceKeeper.GetResource<ISaveData>().SaveChunkData(chunk);
            ResourceReferenceKeeper.GetResource<IMeshGenerator>().ModifyMesh(chunk);
        }
        
        return successfullyAdded;
    }

    public BlockType GetBlockType(Vector3Int position)
    {
        if (!WorldInfo.IsPositionInsideWorld(position))
        {
            return  BlockType.Air;
        }
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
        List<ChunkData> chunksChanged = new List<ChunkData>();
        bool foundFlag = false;
        foreach(Vector3Int position in positionOfBlocksToRemove)
        {
            bool wasRemoved = TryRemoveBlock(position, out BlockTypeWithPosition removedBlock);
            
            ChunkData chunkDataToBeChanged = ResourceReferenceKeeper.GetResource<IChunkLoader>().GetChunkData(position);
            Vector3Int posInChunk = WorldInfo.WorldPositionToPositionInChunk(position);
            var chunkData = chunkDataToBeChanged.Data;
            //var blockDrop = chunkData[posInChunk.x, posInChunk.y, posInChunk.z];
            chunkData[posInChunk.x, posInChunk.y, posInChunk.z] = BlockType.Air;
            chunkDataToBeChanged.OverwriteBlockTypeData(chunkData, false);
            for (int i=0;i<chunksChanged.Count;i++) 
            { 
                foundFlag = false; 
                if (chunksChanged[i].position == chunkDataToBeChanged.position) 
                { 
                    chunksChanged[i]=chunkDataToBeChanged; 
                    foundFlag = true; 
                    break;
                }
            } 
            if (!foundFlag) 
            { 
                chunksChanged.Add(chunkDataToBeChanged);
            }
            
            successfulBlockRemovals.Add(wasRemoved);
            
            if(wasRemoved)
                blocksRemoved.Add(removedBlock);
            else
                blocksRemoved.Add(new BlockTypeWithPosition(BlockType.Air, position));
        }
        foreach (ChunkData chunk in chunksChanged)
        {
            ResourceReferenceKeeper.GetResource<ISaveData>().SaveChunkData(chunk);
            ResourceReferenceKeeper.GetResource<IMeshGenerator>().ModifyMesh(chunk);
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

        return WorldInfo.IsPositionInsideWorld(position) && removedType.BlockType != BlockType.Air;
    }
}
