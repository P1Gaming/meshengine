using MeshEngine.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class ChunkDataReaderDummy : IReadData
{
    BlockType[,,] testData;
    BlockType[,,] TestData
    {
        get
        {
            if(testData == null)
            {
                testData = new BlockType[WorldInfo.WorldDimensions.x, WorldInfo.WorldDimensions.y, WorldInfo.WorldDimensions.z];
                for (int i = 0;i<WorldInfo.WorldDimensions.x;i++)
                {
                    for(int j=0;j<WorldInfo.WorldDimensions.y;j++)
                    {
                        for(int k=0;k<WorldInfo.WorldDimensions.z;k++)
                        {
                            testData[i, j, k] = BlockType.Grass;
                        }
                    }
                }
            }
            return testData;
        }
    } 
    public ChunkData[,,] GetChunkData(Bounds bounds)
    {
        Vector3Int arraySize = new((int)bounds.size.x/WorldInfo.ChunkDimensions.x, (int)bounds.size.y/WorldInfo.ChunkDimensions.y, (int)bounds.size.z / WorldInfo.ChunkDimensions.z);
        var chunkDataArray = new ChunkData[arraySize.x, arraySize.y, arraySize.z];
        int firstPositionX = Mathf.Abs(-WorldInfo.WorldDimensions.x / 2 - (int)bounds.min.x) / WorldInfo.ChunkDimensions.x;
        int firstPositionY = Mathf.Abs(-WorldInfo.WorldDimensions.y / 2 - (int)bounds.min.y) / WorldInfo.ChunkDimensions.y;
        int firstPositionZ = Mathf.Abs(-WorldInfo.WorldDimensions.z / 2 - (int)bounds.min.z) / WorldInfo.ChunkDimensions.z;

        var maxChunksX = WorldInfo.WorldDimensions.x / WorldInfo.ChunkDimensions.x;
        var maxChunksY = WorldInfo.WorldDimensions.y / WorldInfo.ChunkDimensions.y;
        var maxChunksZ = WorldInfo.WorldDimensions.z / WorldInfo.ChunkDimensions.z;

        for (int i = 0;i<arraySize.x;i++)
        {
            for(int j = 0;j<arraySize.y;j++)
            {
                for(int k = 0;k<arraySize.z;k++)
                {
                    if(firstPositionX + i > maxChunksX - 1 || firstPositionY + j > maxChunksY - 1 || firstPositionZ + k > maxChunksZ - 1)
                    {
                        chunkDataArray[i, j, k] = null;
                        continue;
                    }

                    chunkDataArray[i, j, k] = new ChunkData(new Vector3Int(firstPositionX + i, firstPositionY + j, firstPositionZ + k), WorldInfo.ChunkDimensions);
                    chunkDataArray[i, j, k].OverwriteBlockTypeData(TestData, false);
                }
            }
        }

        return chunkDataArray;
    }

    public ChunkData[,] GetChunkData(SquareBoundXZ squareBounds)
    {
        Bounds bounds = new Bounds(new Vector3(squareBounds.Center.x, 0, squareBounds.Center.y), new Vector3(squareBounds.Size, WorldInfo.ChunkDimensions.y, squareBounds.Size));
        ChunkData[,,] chunkDatas = GetChunkData(bounds);

        ChunkData[,] results = new ChunkData[chunkDatas.GetLength(0), chunkDatas.GetLength(2)];
        for (int x = 0; x < results.GetLength(0); x++)
        {
            for (int z = 0; z < results.GetLength(1); z++)
            {
                results[x, z] = chunkDatas[x, 0, z];
            }
        }
        return results;
    }
}