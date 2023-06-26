using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldInfo
{
    public static readonly Vector3Int WorldDimensions = new(640, 256, 640);
    public static readonly Vector3Int ChunkDimensions = new(16, 256, 16);
    public static readonly float BlockSize = 1f;
}
