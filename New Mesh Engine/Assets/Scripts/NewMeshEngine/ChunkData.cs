using MeshEngine.SaveSystem;
using System;
using UnityEngine;

[Serializable]
public class ChunkData
{
    public bool isEmpty { get; private set; }
    public BlockType[,,] Data { get; private set; }

    public readonly Vector3Int position;

    Vector3Int size;

    public ChunkData(Vector3Int position, Vector3Int size)
    {
        this.position = position;
        this.size = size;
        isEmpty = true;
    }
    public ChunkData(Vector3Int position, Vector3Int size, byte[] saveData)
    {
        this.position = position;
        this.size = size;
        isEmpty = saveData[0] == 1;
        Data = new BlockType[size.x, size.y, size.z];
        if (!isEmpty)
        {
            int i = 1;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        Data[x, y, z] = (BlockType)saveData[i];
                        i++;
                    }
                }
            }
        }

    }
    public byte[] GetSaveData()
    {
        byte[] saveData = new byte[(size.x * size.y * size.z + 1)*2];
        saveData[0] = isEmpty ? (byte)1 : (byte)0;

        int i = 1;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    byte[] bytes = BitConverter.GetBytes((ushort)Data[x, y, z]);
                    Array.Copy(bytes, 0, saveData, i, sizeof(ushort));
                    i += 2;
                }
            }
        }
        return saveData;
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
