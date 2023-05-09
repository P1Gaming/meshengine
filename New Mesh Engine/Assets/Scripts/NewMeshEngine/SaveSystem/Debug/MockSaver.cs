using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshEngine.SaveSystem
{
    public class MockSaver : MonoBehaviour
    {
        [SerializeField] List<ChunkData> chunkData = new List<ChunkData>();
        [SerializeField]int saveIndex = 2;
        MeshEngineSaveSystem meshEngineSaveSystem;
        
        private void Awake()
        {
            
            chunkData.Add(new ChunkData(new Vector3Int(0, 0, 0), new Vector3Int(2, 2, 2)));
            chunkData.Add(new ChunkData(new Vector3Int(1, 0, 0), new Vector3Int(2, 2, 2)));
            chunkData.Add(new ChunkData(new Vector3Int(0, 1, 0), new Vector3Int(2, 2, 2)));
            chunkData.Add(new ChunkData(new Vector3Int(0, 0, 1), new Vector3Int(2, 2, 2)));
            chunkData.Add(new ChunkData(new Vector3Int(1, 1, 0), new Vector3Int(2, 2, 2)));
            chunkData.Add(new ChunkData(new Vector3Int(1, 0, 1), new Vector3Int(2, 2, 2)));
            chunkData.Add(new ChunkData(new Vector3Int(0, 1, 1), new Vector3Int(2, 2, 2)));
            chunkData.Add(new ChunkData(new Vector3Int(1, 1, 1), new Vector3Int(2, 2, 2)));

            meshEngineSaveSystem = new MeshEngineSaveSystem("C:/temp/k", "testSave", chunkData.Count);

            SetupChunk(saveIndex);

            Save();

        }

        private void SetupChunk(int i)
        {
            chunkData[i].AddBlockAtIndex(new Vector3Int(0,0,1),BlockType.Grass);
            chunkData[i].AddBlockAtIndex(new Vector3Int(1, 0, 0), BlockType.Grass);
        }

        public void Save()
        {
            meshEngineSaveSystem.SaveChunkData(chunkData[saveIndex]);
        }


    }
}