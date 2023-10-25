using UnityEngine;

/// <summary>
/// Stores a block type with its associated position. 
/// </summary>
public class BlockTypeWithPosition
{
    private BlockType _blockType;
    public BlockType BlockType { get { return _blockType; } }

    private Vector3Int _position;
    public Vector3Int Position { get { return _position; } }

    public BlockTypeWithPosition(BlockType blockType, Vector3Int pos)
    {
        _blockType = blockType;

        _position = pos;
    }
}
