using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Communication between the mesh generator, save system and the mesh engine
/// </summary>
internal interface IRequestHandler
{
    public bool IsBlockAtPosition(Vector3Int worldPosition);

    public BlockType GetBlockAtPosition(Vector3Int worldPosition);

    public bool AddBlockAtPosition(BlockTypeWithPosition blockToBeAdded);

    public BlockTypeWithPosition RemoveBlockAtPosition(Vector3Int worldPosition);
}
