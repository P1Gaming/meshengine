using UnityEngine;

internal interface IChunkLoader
{
    internal ChunkData GetChunkData(Vector3 worldPosition);

}
