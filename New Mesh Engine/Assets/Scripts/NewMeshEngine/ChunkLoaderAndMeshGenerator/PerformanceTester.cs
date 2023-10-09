using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
public class PerformanceTester : MonoBehaviour
{
    public void Test()
    {
        /*float bevel = 0.2f;
        int smoothness = 3;
        var provider = new RoundedVoxels.VoxelProvider(new RoundedVoxels.VoxelSettings(bevel, smoothness));
        var testData = new BlockType[WorldInfo.WorldDimensions.x, WorldInfo.WorldDimensions.y, WorldInfo.WorldDimensions.z];
        for (int i = 0; i < WorldInfo.WorldDimensions.x; i++)
        {
            for (int j = 0; j < WorldInfo.WorldDimensions.y; j++)
            {
                for (int k = 0; k < WorldInfo.WorldDimensions.z; k++)
                {
                    testData[i, j, k] = BlockType.Grass;
                }
            }
        }
        var chunkData = new ChunkData(Vector2Int.zero, WorldInfo.ChunkDimensions);
        chunkData.OverwriteBlockTypeData(testData, false);

        var method = typeof(MeshGenerator)
            .GetMethod("CalculateMeshData_NotRounded", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        method.Invoke(null, new object[1] { chunkData });
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Not Rounded -> {stopwatch.ElapsedMilliseconds}");

        method = typeof(MeshGenerator)
            .GetMethod("CalculateMeshData_RoundedSlow", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

        stopwatch.Reset();
        stopwatch.Start();
        method.Invoke(null, new object[3] { chunkData, bevel, smoothness });
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Rounded Slow -> {stopwatch.ElapsedMilliseconds}");

        method = typeof(MeshGenerator)
            .GetMethod("CalculateMeshData_RoundedFast", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

        stopwatch.Reset();
        stopwatch.Start();
        method.Invoke(null, new object[2] { chunkData, provider });
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Rounded Fast -> {stopwatch.ElapsedMilliseconds}");*/
    }
}
