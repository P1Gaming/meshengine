using MeshEngine.SaveSystem;
using System;
using System.Drawing;
using UnityEngine;

[Serializable]
internal class ChunkData
{
    public bool isEmpty { get; private set; }
    public BlockType[,,] Data { get; private set; }

    public readonly Vector3Int position;

    public readonly Vector3Int size;

    /// <summary>
    /// Create a Chunkdata with a position in a three dimensional world.
    /// </summary>
    /// <param name="position"> The position of the chunk</param>
    /// <param name="size">how many blocks fits in the chunk's space (all shunks in the world should have the same size).</param>
    public ChunkData(Vector3Int position, Vector3Int size)
    {
        this.position = position;
        this.size = size;
        isEmpty = true;
    }

    /// <summary>
    /// Create a Chunkdata with a position in a two dimensional world. using x & z
    /// </summary>
    /// <param name="position"> The world position of the chunk</param>
    /// <param name="size">how many blocks fits in the chunk's space (all shunks in the world should have the same size).</param>
    public ChunkData(Vector2Int position,Vector3Int size)
    {
        this.position = new Vector3Int(position.x, 0, position.y);
        this.size = size;
        isEmpty = true;
    }
    
    /// <summary>
    /// Replace the existing data in the chunk. 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="emptyData"></param>
    public void OverwriteBlockTypeData(BlockType[,,] data, bool emptyData)
    {
        isEmpty = emptyData;
        Data = data;
    }

    /// <summary>
    /// Add a blocktype and the specified position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="blockType"></param>
    public void AddBlockAtIndex(Vector3Int position, BlockType blockType)
    {
        // if index is within Chunk size
        if (!(
            position.x < 0 || position.x > size.x - 1 ||
            position.y < 0 || position.y > size.y - 1 ||
            position.z < 0 || position.z > size.z - 1
            ))
        {
            if (isEmpty)
            {
                Data = new BlockType[size.x, size.y, size.z];
                isEmpty = false;
            }
            Data[position.x, position.y, position.z] = blockType;
        }
    }
    
    //TESTING
    /// <summary>
    /// Add a blocktype and the specified position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="blockType"></param>
    public void OverwriteBlockDataAtPosition(Vector3Int pos, BlockType newBlockType)
    {
        
    }

}
