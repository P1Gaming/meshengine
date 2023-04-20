using UnityEngine;

public interface IReadData
{
    public ChunkData[,,] GetChunkData(Bounds bounds);
}
