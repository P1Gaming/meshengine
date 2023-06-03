using MeshEngine.SaveSystem;
using System;
using UnityEngine;

[Serializable]
internal class ChunkData
{
    public bool isEmpty { get; private set; }
    public BlockType[,,] Data { get; private set; }

    public readonly Vector3Int position;

    public readonly Vector3Int size;

    public ChunkData(Vector3Int position, Vector3Int size)
    {
        this.position = position;
        this.size = size;
        isEmpty = true;
    }
    
    public void OverwriteBlockTypeData(BlockType[,,] data, bool emptyData)
    {
        isEmpty = emptyData;
        Data = data;
    }

    public void AddBlockAtIndex(Vector3Int index, BlockType blockType)
    {
        // if index is within Chunk size
        if (!(
            index.x < 0 || index.x > size.x - 1 ||
            index.y < 0 || index.y > size.y - 1 ||
            index.z < 0 || index.z > size.z - 1
            ))
        {
            if (isEmpty)
            {
                Data = new BlockType[size.x, size.y, size.z];
                isEmpty = false;
            }
            Data[index.x, index.y, index.z] = blockType;
        }
    }

}
