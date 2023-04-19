using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WorldMesh : MonoBehaviour, IMeshEngineTwo
{
    [SerializeField] int worldChunksPerSide = 100;
    [SerializeField] Transform chunkContainer;
    [SerializeField] GameObject playerPrefab;

    const int MAX_CHUNKS_PER_FRAME = 1;
    public int worldVoxelsPerSide { get; private set; }

    /* NB: Must disable antialiasing in render pipeline asset and mipmapping on texture in order to eliminate lines between blocks */
    public int seed;
    public BiomeAttributes biome;

    public Vector3Int spawnPosition;

    public Material material;
    public BlockType[] blockTypes;

    public static BlockType[] readableBlockTypes;

    Chunk[,] chunks;
    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();
    List<Task<ChunkData>> generationTasks = new List<Task<ChunkData>>();


    ChunkCoord playerLastChunkCoord;
    static Vector3 playerPosition;

    bool creatingChunks;

    private void OnEnable()
    {
        GameEvents.UpdateAction += OnUpdate;
        GameEvents.UpdatePlayerPositionEvent += UpdatePlayerPosition;
    }

    private void OnDisable()
    {
        GameEvents.UpdateAction -= OnUpdate;
        GameEvents.UpdatePlayerPositionEvent -= UpdatePlayerPosition;
    }

    private void Start()
    {
        Random.InitState(seed);
        creatingChunks = false;


        worldVoxelsPerSide = worldChunksPerSide * Chunk.CHUNK_WIDTH;
        chunks = new Chunk[worldChunksPerSide, worldChunksPerSide];

        spawnPosition = new Vector3Int(worldChunksPerSide / 2 * Chunk.CHUNK_WIDTH, Chunk.CHUNK_HEIGHT - 50, worldChunksPerSide / 2 * Chunk.CHUNK_WIDTH);

        float beginTime = Time.realtimeSinceStartup;
        GenerateWorld();
        Debug.Log(Time.realtimeSinceStartup - beginTime);

        playerLastChunkCoord = GetChunkCoordFromVector3(playerPosition);
    }

    private void OnUpdate(float dT, bool gamePaused)
    {
        if (gamePaused)
            return;

        ChunkCoord currentCoord = GetChunkCoordFromVector3(playerPosition);
        Vector3Int currentVoxel = GetVoxelCoordFromWorldPosition(playerPosition);
        GameEvents.UpdatePlayerCoordsInWorldEvent?.Invoke(currentCoord, currentVoxel);

        if (!playerLastChunkCoord.Equals(GetChunkCoordFromVector3(playerPosition)))
        {
            CheckViewDistance(currentCoord);
            playerLastChunkCoord = currentCoord;
        }

        if (!creatingChunks && chunksToCreate.Count > 0)
            CreateChunks();

        if (creatingChunks)
        {
            // TODO: need to make some sort of time budget for this, so that if it takes a long time to work through the completed tasks, the game doesn't stutter. For now, use max of two chunks
            int chunksCompleted = 0;
            for (int i = 0; i < generationTasks.Count; i++)
            {
                if (generationTasks[i].IsCompleted && chunksCompleted < MAX_CHUNKS_PER_FRAME)
                {
                    chunks[generationTasks[i].Result.coord.x, generationTasks[i].Result.coord.z].LoadChunkData(generationTasks[i].Result);
                    chunks[generationTasks[i].Result.coord.x, generationTasks[i].Result.coord.z].GenerateChunkMeshes();
                    generationTasks.RemoveAt(i);
                    chunksCompleted += 1;
                }
            }

            if (generationTasks.Count == 0)
                creatingChunks = false;
        }
    }

    void CreateChunks()
    {
        creatingChunks = true;

        generationTasks.Clear();
        while (chunksToCreate.Count > 0)
        {
            ChunkCoord coord = new ChunkCoord(chunksToCreate[0].x, chunksToCreate[0].z);
            generationTasks.Add(Task.Factory.StartNew(() => new ChunkData(this, coord)));
            chunksToCreate.RemoveAt(0);
        }
    }

    void UpdatePlayerPosition(Vector3 newPosition)
    {
        playerPosition = newPosition;
    }

    public static ChunkCoord GetCurrentPlayerChunk()
    {
        return new ChunkCoord(playerPosition);
    }

    ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / Chunk.CHUNK_WIDTH);
        int z = Mathf.FloorToInt(pos.z / Chunk.CHUNK_WIDTH);

        return new ChunkCoord(x, z);
    }

    void CheckViewDistance(ChunkCoord coord)
    {
        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);
        activeChunks.Clear();

        for (int x = coord.x - VoxelData.ViewDistanceInChunks; x < coord.x + VoxelData.ViewDistanceInChunks; x++)
            for (int z = coord.z - VoxelData.ViewDistanceInChunks; z < coord.z + VoxelData.ViewDistanceInChunks; z++)
            {
                if (IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    if (chunks[x, z] == null)
                    {
                        // create the chunk object, and add to list to initialize over time
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, chunkContainer, gameObject.layer, false);
                        chunksToCreate.Add(new ChunkCoord(x, z));
                    }

                    chunks[x, z].isActive = true;
                    activeChunks.Add(new ChunkCoord(x, z));

                    for (int i = 0; i < previouslyActiveChunks.Count; i++)
                        if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                            previouslyActiveChunks.RemoveAt(i);
                }
            }

        for (int i = 0; i < previouslyActiveChunks.Count; i++)
            chunks[previouslyActiveChunks[i].x, previouslyActiveChunks[i].z].isActive = false;
    }

    void GenerateWorld()
    {
        List<Task<ChunkData>> tasks = new List<Task<ChunkData>>();
        for (int x = worldChunksPerSide / 2 - VoxelData.ViewDistanceInChunks; x < worldChunksPerSide / 2 + VoxelData.ViewDistanceInChunks; x++)
            for (int z = worldChunksPerSide / 2 - VoxelData.ViewDistanceInChunks; z < worldChunksPerSide / 2 + VoxelData.ViewDistanceInChunks; z++)
            {
                chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, chunkContainer, gameObject.layer, false);
                ChunkCoord coord = new ChunkCoord(x, z);
                tasks.Add(Task.Factory.StartNew(() => new ChunkData(this, coord)));
                activeChunks.Add(new ChunkCoord(x, z));
            }
        Task.WaitAll(tasks.ToArray());

        for (int i = 0; i < tasks.Count; i++)
        {
            chunks[tasks[i].Result.coord.x, tasks[i].Result.coord.z].LoadChunkData(tasks[i].Result);
            chunks[tasks[i].Result.coord.x, tasks[i].Result.coord.z].GenerateChunkMeshes();
        }

        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        Transform player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity, transform).GetComponent<Transform>();
        //player.GetComponent<Player>().InitPlayer(this);
        playerPosition = player.position;
    }

    public byte GetVoxelFromWorld(Vector3 position)
    {
        // Return air if outside 
        if (!IsPositionInWorld(position))
            return 0;

        // return the appropriate voxel, if the chunk has been generated
        ChunkCoord targetChunk = new ChunkCoord(position);
        if (chunks[targetChunk.x, targetChunk.z] != null && chunks[targetChunk.x, targetChunk.z].voxelsGenerated)
            return chunks[targetChunk.x, targetChunk.z].GetVoxelFromWorldPosition(position);

        // generate the voxel using the generation algorithm
        else
            return GenerateVoxel(Vector3Int.FloorToInt(position));
    }

    public byte GenerateVoxel(Vector3Int position)
    {
        /* Immutable Pass */

        // Return air if outside 
        if (!IsPositionInWorld(position))
            return 0;

        // Bottom block is always bedrock
        if (position.y == 0)
            return 1;

        /* BASIC TERRAIN */
        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), 0, biome.terrainScale, Chunk.CHUNK_WIDTH)) + biome.solidGroundHeight;
        byte voxelValue;

        if (position.y == terrainHeight)
            voxelValue = 3;
        else if (position.y < terrainHeight && position.y > terrainHeight - 4)
            voxelValue = 5;
        else if (position.y > terrainHeight)
            return 0;
        else
            voxelValue = 2;

        /* SECOND PASS */
        if (voxelValue == 2)
        {
            for (int i = 0; i < biome.lodes.Length; i++)
            {
                if (position.y > biome.lodes[i].minHeight && position.y < biome.lodes[i].maxHeight)
                    if (Noise.Get3DPerlin(position, biome.lodes[i].noiseOffset, biome.lodes[i].scale, biome.lodes[i].threshold))
                        voxelValue = biome.lodes[i].blockID;
            }
        }
        return voxelValue;
    }

    public bool IsChunkInWorld(ChunkCoord coord)
    {
        if (coord.x > 0 && coord.x < worldChunksPerSide && coord.z > 0 && coord.z < worldChunksPerSide)
            return true;
        else
            return false;
    }

    public bool IsPositionInWorld(Vector3 position)
    {
        if (position.x >= 0 && position.x < worldVoxelsPerSide && position.y >= 0 && position.y < Chunk.CHUNK_HEIGHT && position.z >= 0 && position.z < worldVoxelsPerSide)
            return true;
        else
            return false;
    }

    public Vector3Int GetVoxelCoordFromWorldPosition(Vector3 position)
    {
        int intX = Mathf.FloorToInt(position.x);
        int intY = Mathf.FloorToInt(position.y);
        int intZ = Mathf.FloorToInt(position.z);

        return new Vector3Int
        {
            x = intX,
            y = intY,
            z = intZ
        };
    }

    public BlockType WhichBlock(Ray ray, Vector3 collisionPoint)
    {
        throw new System.NotImplementedException();
    }

    public bool TryAddBlock(Ray ray, BlockType blockType)
    {
        throw new System.NotImplementedException();
    }

    public bool TryAddBlock(Vector3 position, BlockType blockType)
    {
        throw new System.NotImplementedException();
    }

    public bool TryRemoveBlock(Ray ray)
    {
        throw new System.NotImplementedException();
    }

    public bool TryRemoveBlock(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetBlockCentre(Ray ray, Vector3 collisionPoint, out Vector3 blockCentre)
    {
        throw new System.NotImplementedException();
    }
}


/// <summary>
/// We discussed instead of doing what is implemented here, using an enum. This may
/// need to be deleted/replaced.
/// </summary>
[System.Serializable]
public class BlockType
{
    public string blockName;
    public bool isSolid;

    [Header("Texture Values")]
    public int backFaceTextureID;
    public int frontFaceTextureID;
    public int topFaceTextureID;
    public int bottomFaceTextureID;
    public int leftFaceTextureID;
    public int rightFaceTextureID;

    public int GetTextureID(int faceIndex)
    {
        // 0 = Back face, 1 = front face, 2 = top face, 3 = bottom face, 4 = left face, 5 = right face
        switch (faceIndex)
        {
            case 0:
                return backFaceTextureID;
            case 1:
                return frontFaceTextureID;
            case 2:
                return topFaceTextureID;
            case 3:
                return bottomFaceTextureID;
            case 4:
                return leftFaceTextureID;
            case 5:
                return rightFaceTextureID;
            default:
                Debug.LogError("Invalid face index");
                return 0;
        }
    }
}
