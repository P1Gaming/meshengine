using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PositionTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void WorldPositionToPositionInsideChunk()
    {
        Vector3 worldPosition = new Vector3(25, 100, 30);
        Vector3Int expectedPosition = new Vector3Int(9, 100, 14);

        Vector3Int actualPosition = WorldInfo.WorldPositionToPositionInChunk(worldPosition);

        Assert.AreEqual(expectedPosition, actualPosition);
    }

    [Test]
    public void PositionInsideChunkToWorldPosition()
    {
        ChunkData chunk = new ChunkData(new Vector2Int(2, 2), WorldInfo.ChunkDimensions);

        Vector3Int positionInChunk = new Vector3Int(9, 100, 14);
        Vector3 expectedWorldPosition = new Vector3(41, 100, 46);

        Vector3 actualWorldPosition = WorldInfo.PositionInChunkToWorldPosition(positionInChunk,chunk);
    }

    [Test] 
    public void WorldPositionToChunkXZIndex()
    {
        Vector3 worldPosition = new Vector3(25, 100, 30);
        Vector2Int expectedChunkIndex = new Vector2Int(1, 1);
        Vector2Int actualChunkIndex = WorldInfo.WorldPositionToChunkXZIndex(worldPosition);
        Assert.AreEqual(expectedChunkIndex, actualChunkIndex);
    }

    

}
