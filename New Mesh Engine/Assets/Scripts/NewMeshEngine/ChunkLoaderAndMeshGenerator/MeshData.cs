using System.Collections.Generic;
using UnityEngine;
public class MeshData
{
    public List<Vector3> vertices;
    public List<int> triangles;
    public MeshData(List<Vector3> vertices, List<int> triangles)
    {
        this.vertices = vertices;
        this.triangles = triangles;
    }
    public MeshData()
    {
        vertices = new();
        triangles = new();
    }

    public Mesh GetMesh()
    {
        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
