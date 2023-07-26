using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Communication between the mesh generator, save system and the mesh engine
/// </summary>
internal interface IRequestHandler
{
    public bool IsBlockAtPosition(Vector3Int position);

    public BlockType GetBlockAtPosition(Vector3Int position);

    public bool AddBlockAtPosition(BlockTypeWithPosition blockToBeAdded);

    public BlockTypeWithPosition RemoveBlockAtPosition(Vector3Int position);
}
