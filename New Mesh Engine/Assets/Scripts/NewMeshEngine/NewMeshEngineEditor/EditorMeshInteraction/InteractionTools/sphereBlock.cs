using System;
using System.Collections.Generic;
using UnityEngine;

public class SphereBlock : SelectionTool
{
    List<Vector3Int> blocks;
    Func<BlockType> GetBlockType;
    Func<float> GetRadius;
    Transform sphere;
    public SphereBlock(Func<BlockType> getBlockType, Func<float> getRadius)
    {
        blocks = new List<Vector3Int>();
        GetBlockType = getBlockType;
        GetRadius = getRadius;
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        sphere.GetComponent<Collider>().enabled = false;
    }

    public override void Tick(IInput input)
    {
        float radius = GetRadius();
        sphere.localScale = Vector3.one * radius * 2;

        Vector3Int pos = input.GetPointerGridPositionPosition(false);
        sphere.position = pos;
        if (input.PointerClick())
        {
            FindWholeUnitPositions(radius, pos);
            var command = new AddBlocksCommand(blocks, GetBlockType());
            InvokeFinnished(command);
        }

    }
    public override void OnCancel()
    {
        GameObject.Destroy(sphere.gameObject);
    }
    void FindWholeUnitPositions(float radius, Vector3 initialPosition)
    {
        blocks.Clear();
        Vector3Int roundedPosition = Vector3Int.RoundToInt(initialPosition);

        for (int x = (int)(roundedPosition.x - radius); x <= (int)(roundedPosition.x + radius); x++)
        {
            for (int y = (int)(roundedPosition.y - radius); y <= (int)(roundedPosition.y + radius); y++)
            {
                for (int z = (int)(roundedPosition.z - radius); z <= (int)(roundedPosition.z + radius); z++)
                {
                    Vector3Int gridPosition = new Vector3Int(x, y, z);

                    // Calculate the distance from the initial position to the gridPosition.
                    float distance = Vector3.Distance(initialPosition, gridPosition);

                    if (distance <= radius)
                    {
                        blocks.Add(gridPosition);
                        // 'gridPosition' is within the desired radius from 'initialPosition'.
                        // You can use 'gridPosition' for your purposes here.
                        Debug.Log("Found whole unit position: " + gridPosition);
                    }
                }
            }
        }
    }
}