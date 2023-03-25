using System;
using UnityEngine;

public static class GameEvents
{
    public static Action<float, bool> UpdateAction;
    public static Action<Vector3> UpdatePlayerPositionEvent;
    public static Action<ChunkCoord, Vector3Int> UpdatePlayerCoordsInWorldEvent;
    public static Action<string> ChangeSelectedItemEvent;
}
