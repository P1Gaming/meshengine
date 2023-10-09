using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public static class WorldInfo
{
    public static readonly Vector3Int WorldDimensions = new(640, 256, 640);
    public static readonly Vector3Int ChunkDimensions = new(16, 256, 16);
    public static readonly float BlockSize = 1f;


    


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

}
