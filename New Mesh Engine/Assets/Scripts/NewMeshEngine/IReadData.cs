using UnityEngine;

internal interface IReadData
{
    public ChunkData[,,] GetChunkData(Bounds bounds);
}
