using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal interface IChunkLoader
{
    public ChunkData GetChunkData(Vector3 worldPosition);
}
