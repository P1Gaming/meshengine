using UnityEngine;

public interface ISaveDate
{
    public void SaveChunkData (ChunkData chunkData);
}
public class ChunkData
{
    bool isEmpty;
    BlockType[][][] Data;
    Vector3Int position;
}
public enum BlockType
{

}
