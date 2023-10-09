using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoundedVoxels
{
    public class RoundedVoxelVisualizer : MonoBehaviour
    {
        [SerializeField] MeshFilter meshFilter;
        [Range(0f, 1f)]
        [SerializeField] float bevelness;
        [Range(0, 10)]
        [SerializeField] int smoothness;

        public static bool weldedEdges = false;

        public bool[,,] activeCubes = new bool[3, 3, 3];
        public void GenerateCubes()
        {
            var voxelProvider = new VoxelProvider(new VoxelSettings(bevelness, smoothness));
            var meshData = new MeshData();
            activeCubes[1, 1, 1] = true;
            for (int i = 0; i < activeCubes.GetLength(0); i++)
            {
                for (int j = 0; j < activeCubes.GetLength(1); j++)
                {
                    for (int k = 0; k < activeCubes.GetLength(2); k++)
                    {
                        if (!activeCubes[i, j, k])
                        {
                            continue;
                        }

                        var neighbours = new VoxelNeighbours(activeCubes, new Vector3Int(i, j, k));
                        var voxelCentre = -1 * Vector3.one + new Vector3(i, j, k);
                        voxelProvider.AddVoxelToMeshData(meshData, voxelCentre, neighbours);
                    }
                }
            }

            meshFilter.sharedMesh = meshData.GetMesh();
        }
    }
}
