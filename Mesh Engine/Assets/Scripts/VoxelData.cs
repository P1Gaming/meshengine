using UnityEngine;

/* Contains static data for defining voxel vertices, triangles, faces and uv coors. */

public static class VoxelData
{
    public static readonly int TextureAtlasSizeInBlocks = 4;
    public static float NormalizedBlockTextureSize
    {
        get { return 1f / TextureAtlasSizeInBlocks; }
    }

    public static readonly int ViewDistanceInChunks = 10;

    // Define the points of a voxel
    public static readonly Vector3Int[] voxelVerts = new Vector3Int[8]
    {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(1, 0, 1),
        new Vector3Int(1, 1, 1),
        new Vector3Int(0, 1, 1)
    };

    public static readonly Vector3Int[] faceChecks = new Vector3Int[6]
    {
        new Vector3Int(0, 0, -1), // Check Back face
        new Vector3Int(0, 0, 1), // Check Front face
        new Vector3Int(0, 1, 0), // Check Top Face
        new Vector3Int(0, -1, 0), // Check Bottom Face
        new Vector3Int(-1, 0, 0), // Check Left Face
        new Vector3Int(1, 0, 0), // Check Right Face

    };

    public static readonly int[,] voxelTris = new int[6, 4]
    {
        { 0, 3, 1, 2 }, // Back face
        { 5, 6, 4, 7 }, // Front face
        { 3, 7, 2, 6 }, // Top face
        { 1, 5, 0, 4 }, // Bottom face
        { 4, 7, 0, 3 }, // Left face
        { 1, 2, 5, 6 } // Right face
    };

    public static readonly Vector2[] voxelUvs = new Vector2[4]
    {
        new Vector2 (0, 0),
        new Vector2 (0, 1),
        new Vector2 (1, 0),
        new Vector2 (1, 1)
    };
}
