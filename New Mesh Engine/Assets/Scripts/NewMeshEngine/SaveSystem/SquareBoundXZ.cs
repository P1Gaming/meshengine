using UnityEngine;

internal class SquareBoundXZ
{
    public Vector2 Center;
    public float Size;
    public float Extent => Size /2;
    public Vector2 Max => Center + new Vector2(Extent, Extent);
    public Vector2 Min => Center - new Vector2(Extent, Extent);
    
}
