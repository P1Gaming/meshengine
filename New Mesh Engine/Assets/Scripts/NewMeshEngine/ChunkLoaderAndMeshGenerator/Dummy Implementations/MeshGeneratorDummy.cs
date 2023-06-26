using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class MeshGeneratorDummy : MonoBehaviour, IMeshGenerator
{
    public event Action<ChunkData, Mesh> OnMeshGenerated;
    public event Action<ChunkData, Mesh> OnMeshModified;

    public void ModifyMesh(ChunkData chunkData)
    {
        throw new NotImplementedException();
    }

    public void StartGeneratingMesh(ChunkData chunkData)
    {
        List<Vector3> vertices = new()
        {
            -WorldInfo.ChunkDimensions / 2,
            -WorldInfo.ChunkDimensions / 2 + new Vector3(0, 0, WorldInfo.ChunkDimensions.z),
            -WorldInfo.ChunkDimensions / 2 + new Vector3(WorldInfo.ChunkDimensions.x, 0, WorldInfo.ChunkDimensions.z),
            -WorldInfo.ChunkDimensions / 2 + new Vector3(WorldInfo.ChunkDimensions.x, 0, 0),
            -WorldInfo.ChunkDimensions / 2 + new Vector3(0, WorldInfo.ChunkDimensions.y, 0),
            -WorldInfo.ChunkDimensions / 2 + new Vector3(0, WorldInfo.ChunkDimensions.y, WorldInfo.ChunkDimensions.z),
            -WorldInfo.ChunkDimensions / 2 + new Vector3(WorldInfo.ChunkDimensions.x, WorldInfo.ChunkDimensions.y, WorldInfo.ChunkDimensions.z),
            -WorldInfo.ChunkDimensions / 2 + new Vector3(WorldInfo.ChunkDimensions.x, WorldInfo.ChunkDimensions.y, 0)
        };

        List<int> triangles = new()
        {
            0, 1, 2, 0, 2, 3,
            4, 5, 6, 4, 6, 7,
            0, 4, 7, 0, 7, 3,
            3, 7, 6, 3, 6, 2,
            2, 6, 1, 6, 5, 1,
            1, 5, 4, 1, 4, 0
        };

        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        OnMeshGenerated.Invoke(chunkData, mesh);
    }
}
