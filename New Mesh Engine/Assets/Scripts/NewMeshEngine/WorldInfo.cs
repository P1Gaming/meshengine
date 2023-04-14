using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldInfo
{
    public static readonly Vector3Int WorldDimensions = new(200, 200, 200);
    public static readonly Vector3Int ChunkDimensions = new(16, 128, 16);
    public static readonly float BlockSize = 1f;
}
