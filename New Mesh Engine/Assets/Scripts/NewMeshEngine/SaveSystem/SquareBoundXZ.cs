using Codice.Client.BaseCommands;
using UnityEngine;

internal class SquareBoundXZ
{
    public Vector2 Center;
    public float Size;
    public float Extent => Size / 2;
    public Vector2 Max => Center + new Vector2(Extent, Extent);
    public Vector2 Min => Center - new Vector2(Extent, Extent);
    public SquareBoundXZ()
    {

    }

    public SquareBoundXZ(Vector2 center, float size)
    {
        Center = center;
        Size = size;
    }

    public bool Contains(Vector3 point)
    {
        return point.x >= Min.x && point.x <= Max.x && point.z >= Min.y && point.z <= Max.y;
    }

}
