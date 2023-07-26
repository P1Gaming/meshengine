using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Responsible for outside communication with other classes to allow them to interface with the mesh/save data.
/// </summary>
public interface IMeshEngine
{
    public BlockType GetBlockType(Vector3Int position);
    public bool TryAddBlock(BlockTypeWithPosition block);
    public bool TryAddBlock(Vector3Int position, BlockType blockType);
    public bool TryRemoveBlock(Vector3Int position);
    public bool TryRemoveBlock(Vector3Int position, out BlockType removedType);
    public bool IsBlockThere(Vector3Int position);
    public Vector3 GetBlockCentre(Vector3 position);

    public void AddBlocks(List<BlockTypeWithPosition> blocksToAdd);
    List<BlockTypeWithPosition> RemoveBlocks(List<Vector3Int> positionOfBlocksToRemove);
}
