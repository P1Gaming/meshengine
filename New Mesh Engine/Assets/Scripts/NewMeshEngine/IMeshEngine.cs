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
    public List<bool> AddBlocks(List<BlockTypeWithPosition> blocksToAdd);
    public BlockType GetBlockType(Vector3Int position);
    public bool IsBlockAtPosition(Vector3Int position);
    public List<bool> TryRemoveBlocks(List<Vector3Int> positionOfBlocksToRemove, out List<BlockTypeWithPosition> blocks);
    public bool TryAddBlock(Vector3Int position, BlockType blockType);
    public bool TryAddBlock(BlockTypeWithPosition block);
    public bool TryRemoveBlock(Vector3Int position);
    public bool TryRemoveBlock(Vector3Int position, out BlockTypeWithPosition removedType);

}
