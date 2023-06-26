using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
internal class MeshGenerator : MonoBehaviour, IMeshGenerator
{
    /* #region  UnitCubeNormalizedVertices */
    /*convention:
          each block has 8 unique vertices.
          The normalized vertices are stored at indices shown below (with cube centre as origin):

          4+++++++++++5
          +           +  
          +    0+++++++++++++1       
          +    +      +      +        
          7+++++++++++6      +
               +             +
               +             +
               3+++++++++++++2
        */
    public static readonly Vector3[] UnitCubeNormalizedVertices =
    {
        new Vector3(-1, 1, -1), new Vector3(1, 1, -1), new Vector3(1, -1, -1), new Vector3(-1, -1, -1),
        new Vector3(-1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, -1, 1), new Vector3(-1, -1, 1)
    };
    /* #endregion */
    public event Action<ChunkData, Mesh> OnMeshGenerated;
    public event Action<ChunkData, Mesh> OnMeshModified;

    Queue<ChunkData> chunksToGenerateMeshFor = new();
    Queue<ChunkDataAndMeshData> chunksGenerated = new();
    bool generatingMesh;
    private void Update()
    {
        if(chunksGenerated.Count > 0)
        {
            var chunkDataAndMeshData = chunksGenerated.Dequeue();
            var mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = chunkDataAndMeshData.meshData.vertices.ToArray();
            mesh.triangles = chunkDataAndMeshData.meshData.triangles.ToArray();
            mesh.RecalculateNormals();
            OnMeshGenerated?.Invoke(chunkDataAndMeshData.chunkData, mesh);
        }

        if(generatingMesh || chunksToGenerateMeshFor.Count == 0)
        {
            return;
        }

        GenerateMesh(chunksToGenerateMeshFor.Dequeue());        
    }

    //Not doing this on a separate thread since a modification probably should happen then and there.
    //Also, changes might be needed in the interface as we get closer to implementation.
    //Will get back to this one later.
    public void ModifyMesh(ChunkData chunkData)
    {
        if(chunkData.isEmpty)
        {
            OnMeshModified?.Invoke(chunkData, new Mesh());
            return;
        }

        var meshData = CalculateMeshData(chunkData);
        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = meshData.vertices.ToArray();
        mesh.triangles = meshData.triangles.ToArray();
        mesh.RecalculateNormals();
        OnMeshModified?.Invoke(chunkData, mesh);
    }

    public void StartGeneratingMesh(ChunkData chunkData)
    {
        if(chunkData.isEmpty)
        {
            return;
        }

        chunksToGenerateMeshFor.Enqueue(chunkData);      
    }

    void GenerateMesh(ChunkData chunkData)
    {
        generatingMesh = true;
        _ = Task.Run(() =>
        {
            var meshData = CalculateMeshData(chunkData);
            chunksGenerated.Enqueue(new ChunkDataAndMeshData(chunkData, meshData));
            generatingMesh = false;
        });
    }

    MeshData CalculateMeshData(ChunkData chunkData)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        //List<Vector2> uvs = new List<Vector2>();

        Vector3 firstBlockCentre = -WorldInfo.ChunkDimensions / 2 + (WorldInfo.BlockSize / 2) * Vector3.one;

        for (int i = 0; i < WorldInfo.ChunkDimensions.x; i++)
        {
            for (int j = 0; j < WorldInfo.ChunkDimensions.y; j++)
            {
                for (int k = 0; k < WorldInfo.ChunkDimensions.z; k++)
                {
                    if (chunkData.Data[i, j, k] == BlockType.Air)
                        continue;

                    Vector3 blockPosition = firstBlockCentre + WorldInfo.BlockSize * new Vector3(i, j, k);

                    if ((i > 0 && chunkData.Data[i - 1, j, k] == BlockType.Air) || i == 0)
                    {
                        //render left face
                        AddFaceVertices(ref vertices, blockPosition, 4, 0, 3, 7);
                        AddFaceTriangles(ref triangles, vertices.Count);
                    }
                    if ((i < WorldInfo.ChunkDimensions.x - 1 && chunkData.Data[i + 1, j, k] == BlockType.Air) || i == WorldInfo.ChunkDimensions.x - 1)
                    {
                        //render right face
                        AddFaceVertices(ref vertices, blockPosition, 1, 5, 6, 2);
                        AddFaceTriangles(ref triangles, vertices.Count);
                    }
                    if ((j > 0 && chunkData.Data[i, j - 1, k] == BlockType.Air) || j == 0)
                    {
                        //render bottom face
                        AddFaceVertices(ref vertices, blockPosition, 3, 2, 6, 7);
                        AddFaceTriangles(ref triangles, vertices.Count);
                    }
                    if ((j < WorldInfo.ChunkDimensions.y - 1 && chunkData.Data[i, j + 1, k] == BlockType.Air) || j == WorldInfo.ChunkDimensions.y - 1)
                    {
                        //render top face
                        AddFaceVertices(ref vertices, blockPosition, 4, 5, 1, 0);
                        AddFaceTriangles(ref triangles, vertices.Count);
                    }
                    if ((k > 0 && chunkData.Data[i, j, k - 1] == BlockType.Air) || k == 0)
                    {
                        //render front face
                        AddFaceVertices(ref vertices, blockPosition, 0, 1, 2, 3);
                        AddFaceTriangles(ref triangles, vertices.Count);
                    }
                    if ((k < WorldInfo.ChunkDimensions.z - 1 && chunkData.Data[i, j, k + 1] == BlockType.Air) || k == WorldInfo.ChunkDimensions.z - 1)
                    {
                        //render back face
                        AddFaceVertices(ref vertices, blockPosition, 5, 4, 7, 6);
                        AddFaceTriangles(ref triangles, vertices.Count);
                    }
                }
            }
        }

        return new MeshData(vertices, triangles);
    }

    static void AddFaceVertices(ref List<Vector3> vertices, Vector3 blockPosition, int vertex0, int vertex1, int vertex2, int vertex3)
    {
        vertices.Add(blockPosition + UnitCubeNormalizedVertices[vertex0]);
        vertices.Add(blockPosition + UnitCubeNormalizedVertices[vertex1]);
        vertices.Add(blockPosition + UnitCubeNormalizedVertices[vertex2]);
        vertices.Add(blockPosition + UnitCubeNormalizedVertices[vertex3]);
    }
    static void AddFaceTriangles(ref List<int> triangles, int verticesCount)
    {
        triangles.Add(verticesCount - 4 + 0);
        triangles.Add(verticesCount - 4 + 1);
        triangles.Add(verticesCount - 4 + 2);
        triangles.Add(verticesCount - 4 + 0);
        triangles.Add(verticesCount - 4 + 2);
        triangles.Add(verticesCount - 4 + 3);
    }

    struct ChunkDataAndMeshData
    {
        public ChunkData chunkData;
        public MeshData meshData;
        public ChunkDataAndMeshData(ChunkData chunkData, MeshData meshData)
        {
            this.chunkData = chunkData;
            this.meshData = meshData;
        }
    }

    struct MeshData
    {
        public List<Vector3> vertices;
        public List<int> triangles;
        public MeshData(List<Vector3> vertices, List<int> triangles)
        {
            this.vertices = vertices;
            this.triangles = triangles;
        }
    }
}