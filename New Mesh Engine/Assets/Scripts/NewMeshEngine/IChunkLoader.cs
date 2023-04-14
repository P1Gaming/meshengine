using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChunkLoader
{
    public ChunkData GetChunkData(Vector3 worldPosition);
}
