using UnityEngine;

internal interface IChunkLoader
{
    public ChunkData GetChunkData(Vector3 worldPosition);
    public BlockType GetBlockTypeInPosition(Vector3 worldPosition);
}
