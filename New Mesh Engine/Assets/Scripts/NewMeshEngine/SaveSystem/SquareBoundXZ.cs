using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBoundXZ : MonoBehaviour
{
    public Vector2 Center;
    public float Size;
    public float Extent => Size /2;
    public Vector2 Max => Center + new Vector2(Extent, Extent);
    public Vector2 Min => Center - new Vector2(Extent, Extent);
    
}
