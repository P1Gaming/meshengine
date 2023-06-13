using UnityEngine;

internal interface IReadData
{
    public ChunkData[,,] GetChunkData(Bounds bounds);
    public ChunkData[,] GetChunkData(SquareBoundXZ bounds);
}
