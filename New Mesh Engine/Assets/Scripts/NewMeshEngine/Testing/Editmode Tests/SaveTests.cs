using System;
using System.IO;
using MeshEngine.SaveSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SaveTests
{
    private SaveSystem GetNewSaveSystem(out Action closeSystem, out int sizeX, out int sizeY, out int sizeZ)
    {
        
        const string testFileName = "testSave";
        sizeX = WorldInfo.ChunkDimensions.x;
        sizeY = WorldInfo.ChunkDimensions.y;
        sizeZ = WorldInfo.ChunkDimensions.z;

        SaveSystem saveSystem = new SaveSystem(ResourceReferenceKeeper.SAVEPATH, testFileName);
        closeSystem = () =>
        {
            saveSystem.Dispose();
            File.Delete(Path.Combine(ResourceReferenceKeeper.SAVEPATH,$"{testFileName}.meshchunks"));
        };
        return saveSystem;
    }
    
    // A Test behaves as an ordinary method
    [Test]
    public void SavedChunkGetsLoadedCorrectly()
    {
        var saveSystem = GetNewSaveSystem(out Action closeSaveSystem,out var sizeX, out var sizeY, out var sizeZ);
        
        var chunk1 = new ChunkData(new Vector2Int(0, 0), WorldInfo.ChunkDimensions);
        var chunk2 = new ChunkData(new Vector2Int(1, 1), WorldInfo.ChunkDimensions);
        var newBlocks = new BlockType[sizeX, sizeY, sizeZ];
        newBlocks[10, 10, 10] = BlockType.Grass;
        chunk2.OverwriteBlockTypeData(newBlocks,false);
        
        saveSystem.SaveChunkData(chunk1);
        saveSystem.SaveChunkData(chunk2);

        SquareBoundXZ bounds = new SquareBoundXZ(new Vector2(1, 1), 2);
        var loadedChunks = saveSystem.GetChunkData(bounds);
        var chunk = loadedChunks[1, 1];
        
        closeSaveSystem();
        Assert.AreEqual(chunk.Data[10, 10, 10], BlockType.Grass);
    }
    
    [Test]
    public void LoadedEmptyChunk()
    {
        
        var saveSystem = GetNewSaveSystem(out Action closeSaveSystem,out var sizeX, out var sizeY, out var sizeZ);

        var chunk1 = new ChunkData(new Vector2Int(0, 0), WorldInfo.ChunkDimensions);
        var chunk2 = new ChunkData(new Vector2Int(1, 1), WorldInfo.ChunkDimensions);
        var newBlocks = new BlockType[sizeX, sizeY, sizeZ];
        newBlocks[10, 10, 10] = BlockType.Grass;
        chunk2.OverwriteBlockTypeData(newBlocks,false);
        
        saveSystem.SaveChunkData(chunk1);
        saveSystem.SaveChunkData(chunk2);

        SquareBoundXZ bounds = new SquareBoundXZ(new Vector2(1, 1), 2);
        var loadedChunks = saveSystem.GetChunkData(bounds);
        closeSaveSystem();
        
        Assert.IsNull(loadedChunks[0,0]);
    }

   
}
