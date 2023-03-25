using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
    public ChunkCoord coord;

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uvs = new List<Vector2>();
    public int vertexIndex = 0;

    public byte[] voxelMap;

    public Vector3Int worldPosition;
    public WorldMesh world;

    public ChunkData(WorldMesh world, ChunkCoord chunkCoord)
    {
        worldPosition = new Vector3Int(chunkCoord.x * Chunk.CHUNK_WIDTH, 0, chunkCoord.z * Chunk.CHUNK_WIDTH);
        coord = chunkCoord;
        this.world = world;
        voxelMap = new byte[Chunk.CHUNK_VOLUME];
        PopulateVoxelMap();
        CreateMeshData();
    }

    void PopulateVoxelMap()
    {
        for (int y = 0; y < Chunk.CHUNK_HEIGHT; y++)
            for (int z = 0; z < Chunk.CHUNK_WIDTH; z++)
                for (int x = 0; x < Chunk.CHUNK_WIDTH; x++)
                    voxelMap[To3DArrayIndex(x, y, z)] = world.GenerateVoxel(new Vector3Int(x, y, z) + worldPosition);
    }

    void CreateMeshData()
    {
        for (int y = 0; y < Chunk.CHUNK_HEIGHT; y++)
            for (int z = 0; z < Chunk.CHUNK_WIDTH; z++)
                for (int x = 0; x < Chunk.CHUNK_WIDTH; x++)
                    if (world.blockTypes[voxelMap[To3DArrayIndex(x, y, z)]].isSolid)
                        AddVoxelDataToChunk(new Vector3Int(x, y, z));
    }

    void AddVoxelDataToChunk(Vector3Int position)
    {
        // 0 = Back face, 1 = front face, 2 = top face, 3 = bottom face, 4 = left face, 5 = right face
        for (int p = 0; p < 6; p++)
        {
            // if no voxel or voxel in direction of face is not solid, then need to show this face
            if (!CheckVoxel(position + VoxelData.faceChecks[p]))
            {
                byte blockID = voxelMap[To3DArrayIndex(position.x, position.y, position.z)];

                vertices.Add(position + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                vertices.Add(position + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                vertices.Add(position + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                vertices.Add(position + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                AddTexture(world.blockTypes[blockID].GetTextureID(p));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);

                vertexIndex += 4;
            }
        }
    }

    // Subtextures are indexed from bottom-left corner
    void AddTexture(int textureID)
    {
        if (textureID >= (VoxelData.TextureAtlasSizeInBlocks * VoxelData.TextureAtlasSizeInBlocks))
            Debug.LogError("Attempting to get subtexture outside texture atlas bounds");

        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);

        uvs.Add(new Vector2(x * VoxelData.NormalizedBlockTextureSize, y * VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x * VoxelData.NormalizedBlockTextureSize, (y + 1) * VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2((x + 1) * VoxelData.NormalizedBlockTextureSize, y * VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2((x + 1) * VoxelData.NormalizedBlockTextureSize, (y + 1) * VoxelData.NormalizedBlockTextureSize));
    }
    bool CheckVoxel(Vector3Int position)
    {
        if (!IsVoxelInChunk(position.x, position.y, position.z))
            return world.blockTypes[world.GetVoxelFromWorld(worldPosition + position)].isSolid;
        else
            return world.blockTypes[voxelMap[To3DArrayIndex(position.x, position.y, position.z)]].isSolid;
    }

    bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > Chunk.CHUNK_WIDTH - 1 || y < 0 || y > Chunk.CHUNK_HEIGHT - 1 || z < 0 || z > Chunk.CHUNK_WIDTH - 1)
            return false;
        else
            return true;
    }
    public static int To3DArrayIndex(int x, int y, int z)
    {
        return x + (z * Chunk.CHUNK_WIDTH) + (y * Chunk.CHUNK_AREA);
    }
}
