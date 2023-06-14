using System;
using UnityEngine;

namespace MeshEngine.SaveSystem
{


    internal class ChunkDataByteConverter
    {
        public static byte[] GetSaveData(ChunkData chunkData)
        {
            byte[] saveData = new byte[(chunkData.size.x * chunkData.size.y * chunkData.size.z) * 2 + 1];
            saveData[0] = chunkData.isEmpty ? (byte)1 : (byte)0;

            int i = 1;

            for (int x = 0; x < chunkData.size.x; x++)
            {
                for (int y = 0; y < chunkData.size.y; y++)
                {
                    for (int z = 0; z < chunkData.size.z; z++)
                    {
                        byte[] bytes = BitConverter.GetBytes((ushort)chunkData.Data[x, y, z]);
                        Array.Copy(bytes, 0, saveData, i, sizeof(ushort));
                        i += 2;
                    }
                }
            }
            return saveData;
        }

        public static BlockType[,,] ConvertByteToChunkBlockTypeData(Vector3Int position, Vector3Int size, byte[] saveData)
        {
            ushort[] uShortSaveData = new ushort[(saveData.Length - 1) / 2];
            Buffer.BlockCopy(saveData, 1, uShortSaveData, 0, saveData.Length - 1);

            BlockType[,,] results = new BlockType[size.x, size.y, size.z];

            int i = 0;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        results[x, y, z] = (BlockType)uShortSaveData[i];
                        i++;
                    }
                }
            }

            return results;

        }
    }
}