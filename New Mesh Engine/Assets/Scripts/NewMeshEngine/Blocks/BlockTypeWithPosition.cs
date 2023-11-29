using UnityEngine;

/// <summary>
/// Stores a block type with its associated position. 
/// </summary>
public class BlockTypeWithPosition
{
    private S_BlockType _blockType;
    public int BlockID { get { return _blockType.Id; } }

    private Vector3Int _position;
    public Vector3Int Position { get { return _position; } }

    public BlockTypeWithPosition(S_BlockType blockType, Vector3Int pos)
    {
        _blockType = blockType;

        _position = pos;
    }
}
