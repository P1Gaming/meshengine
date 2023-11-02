using MeshEngine.SaveSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class ChunkLoader : MonoBehaviour, IChunkLoader
{
    [SerializeField] Transform player;
    [SerializeField] Material chunkTestMaterial;
    readonly float chunkLoadRadius = WorldInfo.ChunkDimensions.x * 8;
    readonly float chunkUnloadRadius = WorldInfo.ChunkDimensions.x * 11;
    readonly int boundsSize = 25; //Size of the bounding square as number of chunks. Only the data of chunks within this square will be loaded in the memory.
    ChunkData[,] chunkDatas;
    IReadData chunkDataReader;
    IMeshGenerator meshGenerator;
    SquareBoundXZ currentBounds;
    readonly Dictionary<Vector3Int, MeshFilter> loadedChunks = new();
    ChunkObjectPool chunkObjectPool;
    Vector2Int currentChunkPosition;
    private void Awake()
    {
        chunkDataReader = ResourceReferenceKeeper.GetResource<IReadData>();
        meshGenerator = ResourceReferenceKeeper.GetResource<IMeshGenerator>();

        chunkObjectPool = new ChunkObjectPool();
        var poolSize = Mathf.CeilToInt((4 * (chunkUnloadRadius + 1) * (chunkUnloadRadius + 1)) / 
            (WorldInfo.ChunkDimensions.x * WorldInfo.ChunkDimensions.z));
        chunkObjectPool.InstantiatePool(poolSize, chunkTestMaterial);
    }
    private void OnEnable()
    {
        meshGenerator.OnMeshGenerated += OnMeshGenerated;
        meshGenerator.OnMeshModified += OnMeshModified;
    }
    private void OnDisable()
    {
        meshGenerator.OnMeshGenerated -= OnMeshGenerated;
        meshGenerator.OnMeshModified -= OnMeshModified;
    }
    // Start is called before the first frame update
    void Start()
    {
        RecalculateBounds();
        LoadChunksInBound();
        RefreshActiveChunks();
    }

    // Update is called once per frame
    void Update()
    {
        var worldBottomLeftPoint = (-Vector3.one * WorldInfo.BlockSize)/2;

        var position = player.position;
        var numberOfChunksX = Mathf.FloorToInt((position.x - worldBottomLeftPoint.x) / WorldInfo.ChunkDimensions.x);
        var numberOfChunksY = Mathf.FloorToInt((position.z - worldBottomLeftPoint.z) / WorldInfo.ChunkDimensions.z);
        if(numberOfChunksX - 1 == currentChunkPosition.x && numberOfChunksY - 1 == currentChunkPosition.y)
        {
            return;
        }

        RefreshActiveChunks();
        ResetBoundsIfNeeded();
        currentChunkPosition = new Vector2Int(numberOfChunksX - 1, numberOfChunksY - 1);
    }

    private void LoadChunksInBound()
    {
        chunkDatas = chunkDataReader.GetChunkData(currentBounds);
    }
    private void OnMeshGenerated(ChunkData chunkData, Mesh mesh)
    {
        if(!loadedChunks.ContainsKey(chunkData.position))
        {
            return;
        }
        loadedChunks[chunkData.position].mesh = mesh;
        loadedChunks[chunkData.position].gameObject.SetActive(true);
        loadedChunks[chunkData.position].GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void OnMeshModified(ChunkData chunkData, Mesh mesh)
    {
        if(!loadedChunks.ContainsKey(chunkData.position))
        {
            return;
        }
        loadedChunks[chunkData.position].mesh = mesh;
        loadedChunks[chunkData.position].GetComponent<MeshCollider>().sharedMesh = mesh;
        RefreshActiveChunks();
        
    }

    public ChunkData GetChunkData(Vector3 worldPosition)
    {
        if(chunkDatas == null)
        {
            Debug.LogError("ChunkData Array hasn't initialized yet.");
            return null;
        }

        if(!currentBounds.Contains(worldPosition) || !WorldInfo.IsPositionInsideWorld(worldPosition))
        {
            Debug.LogError("Position out of bounds. Chunk data at this position isn't loaded in memory.");
            return null;
        }

        // Get the world index of the chunk
        Vector2Int chunkWorldIndex = WorldInfo.WorldPositionToChunkXZIndex(worldPosition);

        // calculate the index in the currently loaded chunks array
        Vector2Int firstLoadedChunkWorldIndex = WorldInfo.WorldPositionToChunkXZIndex(new Vector3(Mathf.Max( currentBounds.Min.x,0), 1,Mathf.Max(currentBounds.Min.y,0)));
        Vector2Int indexInLoadedChunks = chunkWorldIndex - firstLoadedChunkWorldIndex;
        
        int xIndex = indexInLoadedChunks.x;
        int zIndex = indexInLoadedChunks.y;

        if (chunkDatas[xIndex, zIndex] == null)
        {
            chunkDatas[xIndex, zIndex] = new ChunkData(chunkWorldIndex, WorldInfo.ChunkDimensions);
        }

        return chunkDatas[xIndex, zIndex];
    }
    Vector3Int FindBlock(Vector3 worldPosition,ChunkData chunk)
    {
        Vector3 globalPos = worldPosition;
        Vector3 localChunkPos = transform.InverseTransformPoint(globalPos);
        Vector3Int localBlockPos=Vector3Int.zero;
        localBlockPos.x = (int)localChunkPos.x + WorldInfo.ChunkDimensions.x/2;
        localBlockPos.y = (int)localChunkPos.y + WorldInfo.ChunkDimensions.y/2;
        localBlockPos.z = (int)localChunkPos.z + WorldInfo.ChunkDimensions.z/2;
        //this is the index within the chunk
        Vector3 translatedPosition = localBlockPos+new Vector3Int(chunk.position.x,chunk.position.y,chunk.position.z);
        Vector3Int index = new Vector3Int((int)translatedPosition.x, (int)translatedPosition.y, (int)translatedPosition.z);
        return index;
    }
    public BlockType GetBlockTypeInPosition(Vector3 worldPosition)
    {
        ChunkData chunk=GetChunkData(worldPosition);
        Vector3Int index=FindBlock(worldPosition, chunk);
        BlockType type = chunk.Data[index.x, index.y, index.z];
        return type;
    }
    
    void ResetBoundsIfNeeded()
    {
        var position = player.position;
        var vectorDiff = new Vector2(position.x, position.z) - currentBounds.Center;
        bool resetRequired = Mathf.Abs(vectorDiff.x) + chunkLoadRadius >= currentBounds.Extent ||
            Mathf.Abs(vectorDiff.y) + chunkLoadRadius >= currentBounds.Extent;

        if(!resetRequired)
        {
            return;
        }

        RecalculateBounds();
        LoadChunksInBound();

        Vector3 firstChunkCentre = WorldInfo.ChunkDimensions / 2;
        for (int i=loadedChunks.Count - 1; i >= 0; i--)
        {
            var chunkIndex = loadedChunks.ElementAt(i).Key;            
            Vector3 chunkPosition = firstChunkCentre +
                new Vector3(chunkIndex.x * WorldInfo.ChunkDimensions.x,
                chunkIndex.y * WorldInfo.ChunkDimensions.y,
                chunkIndex.z * WorldInfo.ChunkDimensions.z);

            if(!currentBounds.Contains(chunkPosition))
            {
                var meshFilter = loadedChunks.ElementAt(i).Value;
                chunkObjectPool.ReturnInstance(meshFilter);
                loadedChunks.Remove(chunkIndex);
            }
        }

        RefreshActiveChunks();
    }

    void RefreshActiveChunks()
    {
        Vector3 firstChunkCentre = (WorldInfo.ChunkDimensions / 2) - (Vector3.one*(WorldInfo.BlockSize/2));
        foreach (var chunkData in chunkDatas)
        {
            if(chunkData == null)
            {
                continue;
            }

            Vector3 chunkPosition = firstChunkCentre + 
                new Vector3(chunkData.position.x * WorldInfo.ChunkDimensions.x, 
                chunkData.position.y * WorldInfo.ChunkDimensions.y, 
                chunkData.position.z * WorldInfo.ChunkDimensions.z);

            Vector3 position = player.position;
            float sqrDistance = (new Vector2(position.x, position.z) - new Vector2(chunkPosition.x, chunkPosition.z)).sqrMagnitude;
            bool isChunkWithinLoadRange = sqrDistance <= chunkLoadRadius * chunkLoadRadius;
            bool isChunkAlreadyLoaded = loadedChunks.ContainsKey(chunkData.position);
            if (isChunkWithinLoadRange && !isChunkAlreadyLoaded)
            {
                var chunkObjectInstance = chunkObjectPool.GetInstance();
                chunkObjectInstance.transform.position = chunkPosition;
                chunkObjectInstance.gameObject.SetActive(false);
                loadedChunks.Add(chunkData.position, chunkObjectInstance);
                meshGenerator.StartGeneratingMesh(chunkData);
                continue;
            }

            bool isChunkWithinUnloadRange = sqrDistance <= chunkUnloadRadius * chunkUnloadRadius;

            if (!isChunkWithinUnloadRange && isChunkAlreadyLoaded)
            {
                var meshFilter = loadedChunks[chunkData.position];
                chunkObjectPool.ReturnInstance(meshFilter);
                loadedChunks.Remove(chunkData.position);
            }
        }
    }

    void RecalculateBounds()
    {
        var position = player.position;
        var centreX = (int)(position.x / WorldInfo.ChunkDimensions.x) * WorldInfo.ChunkDimensions.x;
        var centreZ = (int)(position.z / WorldInfo.ChunkDimensions.z) * WorldInfo.ChunkDimensions.z;

        var size = boundsSize * WorldInfo.ChunkDimensions.x;
        var centre = new Vector2(centreX, centreZ);
        currentBounds = new SquareBoundXZ(centre, size);
    }

    private void OnDrawGizmos()
    {
        if(currentBounds == null)
        {
            return;
        }
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(currentBounds.Center.x, 0, currentBounds.Center.y), new Vector3(currentBounds.Size, WorldInfo.ChunkDimensions.y, currentBounds.Size));
        Gizmos.color = Color.green;
        var position = player.position;
        Gizmos.DrawWireSphere(position, chunkLoadRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(position, chunkUnloadRadius);
    }
}


