using UnityEngine;

public class Chunk
{
    public ChunkCoord coord;

    public const int CHUNK_WIDTH = 16;
    public const int CHUNK_HEIGHT = 128;
    public const int CHUNK_AREA = CHUNK_WIDTH * CHUNK_WIDTH;
    public const int CHUNK_VOLUME = CHUNK_AREA * CHUNK_HEIGHT;

    GameObject chunkObject;
    MeshRenderer meshRend;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    ChunkData chunkData;
    Vector3 chunkTransformPosition;

    public bool voxelsGenerated { get; private set; }
    WorldMesh world;

    public Chunk(ChunkCoord coord, WorldMesh world, Transform chunkContainer, int groundLayer, bool initOnCreate)
    {
        this.coord = coord;
        this.world = world;
        voxelsGenerated = false;

        chunkObject = new GameObject();
        meshRend = chunkObject.AddComponent<MeshRenderer>();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshCollider = chunkObject.AddComponent<MeshCollider>();

        meshRend.material = world.material;
        chunkObject.transform.parent = chunkContainer;
        chunkTransformPosition = new Vector3(coord.x * CHUNK_WIDTH, 0, coord.z * CHUNK_WIDTH);
        chunkObject.transform.position = chunkTransformPosition;
        chunkObject.name = "Chunk " + coord.x + "," + coord.z;
        chunkObject.layer = groundLayer;

        //if (initOnCreate)
        //    InitChunk();
    }

    public void LoadChunkData(ChunkData data)
    {
        chunkData = data;
        voxelsGenerated = true;
    }

    public void GenerateChunkMeshes()
    {
        CreateMesh();
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    public void InitChunk()
    {
        chunkData = new ChunkData(world, coord);
        voxelsGenerated = true;

        CreateMesh();
        meshCollider.sharedMesh = meshFilter.mesh;
    }

    public bool isActive
    {
        get { return chunkObject.activeSelf; }
        set { chunkObject.SetActive(value); }
    }

    public Vector3Int worldPosition
    {
        get { return new Vector3Int(coord.x * CHUNK_WIDTH, 0, coord.z * CHUNK_WIDTH); }
    }

    public byte GetVoxelFromWorldPosition(Vector3 pos)
    {
        int intX = Mathf.FloorToInt(pos.x);
        int intY = Mathf.FloorToInt(pos.y);
        int intZ = Mathf.FloorToInt(pos.z);

        intX -= Mathf.FloorToInt(chunkTransformPosition.x);
        intZ -= Mathf.FloorToInt(chunkTransformPosition.z);

        return chunkData.voxelMap[To3DArrayIndex(intX, intY, intZ)];
    }

    void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = chunkData.vertices.ToArray();
        mesh.triangles = chunkData.triangles.ToArray();
        mesh.uv = chunkData.uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    // Subtextures are indexed from bottom-left corner

    public static int To3DArrayIndex(int x, int y, int z)
    {
        return x + (z * CHUNK_WIDTH) + (y * CHUNK_AREA);
    }
}

public struct ChunkCoord
{
    public int x;
    public int z;

    public ChunkCoord(int _x, int _z)
    {
        x = _x;
        z = _z;
    }

    public ChunkCoord(Vector3 pos)
    {
        int intX = Mathf.FloorToInt(pos.x);
        int intZ = Mathf.FloorToInt(pos.z);
        // for now, only one layer of chunks vertically, so no need for y coord

        x = intX / Chunk.CHUNK_WIDTH;
        z = intZ / Chunk.CHUNK_WIDTH;
    }

    public bool Equals(ChunkCoord other)
    {
        if (other.x == x && other.z == z)
            return true;
        else
            return false;
    }
}
