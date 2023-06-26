using MeshEngine.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

internal class ChunkLoader : MonoBehaviour, IChunkLoader
{
    [SerializeField] Transform player;
    [SerializeField] Material chunkTestMaterial;
    readonly float chunkLoadRadius = WorldInfo.ChunkDimensions.x * 8;
    readonly float chunkUnloadRadius = WorldInfo.ChunkDimensions.x * 11;
    readonly int boundsSize = 25; //Size of the bounding square as number of chunks. Only the data of chunks within this square will be loaded in the memory.
    ChunkData[,] chunkData;
    IReadData chunkDataReader;
    IMeshGenerator meshGenerator;
    SquareBoundXZ currentBounds;
    readonly Dictionary<Vector3Int, MeshFilter> loadedChunks = new();
    ChunkObjectPool chunkObjectPool;
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
    }
    private void OnDisable()
    {
        meshGenerator.OnMeshGenerated -= OnMeshGenerated;
    }
    // Start is called before the first frame update
    void Start()
    {
        RecalculateBounds();
        chunkData = chunkDataReader.GetChunkData(currentBounds);
        RefreshActiveChunks();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshActiveChunks();
        ResetBoundsIfNeeded();
    }
    private void OnMeshGenerated(ChunkData chunkData, Mesh mesh)
    {
        if(!loadedChunks.ContainsKey(chunkData.position))
        {
            return;
        }
        loadedChunks[chunkData.position].mesh = mesh;
        loadedChunks[chunkData.position].gameObject.SetActive(true);
    }

    public ChunkData GetChunkData(Vector3 worldPosition)
    {
        if(chunkData == null)
        {
            Debug.LogError("ChunkData Array hasn't initialized yet.");
            return null;
        }

        if(!currentBounds.Contains(worldPosition))
        {
            Debug.LogError("Position out of bounds. Chunk data at this position isn't loaded in memory.");
            return null;
        }

        int xIndex = Clamp0(Mathf.CeilToInt(Mathf.Abs(currentBounds.Min.x - worldPosition.x) / WorldInfo.ChunkDimensions.x) - 1);
        int zIndex = Clamp0(Mathf.CeilToInt(Mathf.Abs(currentBounds.Min.y - worldPosition.z) / WorldInfo.ChunkDimensions.z) - 1);

        return chunkData[xIndex, zIndex];
    }

    static int Clamp0(int value)
    {
        if(value < 0)
        {
            return 0;
        }

        return value;
    }

    void ResetBoundsIfNeeded()
    {
        var vectorDiff = new Vector2(player.position.x, player.position.z) - currentBounds.Center;
        bool resetRequired = Mathf.Abs(vectorDiff.x) + chunkLoadRadius >= currentBounds.Extent ||
            Mathf.Abs(vectorDiff.y) + chunkLoadRadius >= currentBounds.Extent;

        if(!resetRequired)
        {
            return;
        }

        RecalculateBounds();
        chunkData = chunkDataReader.GetChunkData(currentBounds);

        Vector3 firstChunkCentre = -WorldInfo.WorldDimensions / 2 + WorldInfo.ChunkDimensions / 2;
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
        Vector3 firstChunkCentre = -WorldInfo.WorldDimensions / 2 + WorldInfo.ChunkDimensions / 2;
        foreach (var chunkData in chunkData)
        {
            if(chunkData == null)
            {
                continue;
            }

            Vector3 chunkPosition = firstChunkCentre + 
                new Vector3(chunkData.position.x * WorldInfo.ChunkDimensions.x, 
                chunkData.position.y * WorldInfo.ChunkDimensions.y, 
                chunkData.position.z * WorldInfo.ChunkDimensions.z);

            float sqrDistance = (new Vector2(player.position.x, player.position.z) - new Vector2(chunkPosition.x, chunkPosition.z)).sqrMagnitude;
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
        var centreX = (int)(player.position.x / WorldInfo.ChunkDimensions.x) * WorldInfo.ChunkDimensions.x;
        var centreZ = (int)(player.position.z / WorldInfo.ChunkDimensions.z) * WorldInfo.ChunkDimensions.z;

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
        Gizmos.DrawWireSphere(player.position, chunkLoadRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(player.position, chunkUnloadRadius);
    }
}


