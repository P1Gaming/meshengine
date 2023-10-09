using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public static class WorldInfo
{
    public static readonly Vector3Int WorldDimensions = new(640, 256, 640);
    public static readonly Vector3Int ChunkDimensions = new(16, 256, 16);
    public static readonly float BlockSize = 1f;


    
    internal static Vector3Int WorldPositionToPositionInChunk(Vector3 worldPosition)
    {
        int x = (int)worldPosition.x % ChunkDimensions.x;
        int z = (int)worldPosition.z % ChunkDimensions.z;
        int y = (int)worldPosition.y;
        

        return new Vector3Int(x, y, z);
    }
    internal static Vector2Int WorldPositionToChunkXZIndex(Vector3 worldPosition)
    {
        int x = (int)worldPosition.x / ChunkDimensions.x;
        int z = (int)worldPosition.z / ChunkDimensions.z;
        return new Vector2Int(x, z);
    }

    /// <summary>
    /// Calculates the size of the World in Chunks.
    /// </summary>
    /// <returns></returns>
    internal static Vector3Int GetWorldDimensionInChunks()
    {
        int xSize = Mathf.CeilToInt((float)WorldDimensions.x / (float)ChunkDimensions.x);
        int ySize = Mathf.CeilToInt((float)WorldDimensions.y / (float)ChunkDimensions.y);
        int zSize = Mathf.CeilToInt((float)WorldDimensions.z / (float)ChunkDimensions.z);

        return new Vector3Int(xSize, ySize, zSize);
    }

    internal static Vector3 PositionInChunkToWorldPosition(Vector3Int positionInChunk, ChunkData chunk)
    {
        Vector3 chunkOffset = chunk.position*WorldInfo.ChunkDimensions;
        return chunkOffset + positionInChunk;
    }
}
