using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshEngine.SaveSystem
{
    public class MockSaver : MonoBehaviour
    {
        [SerializeField] List<ChunkData> chunkData = new List<ChunkData>();
        [SerializeField]int saveIndex = 0;
        MeshEngineSaveSystem meshEngineSaveSystem;
        
        private void Awake()
        {
            
            chunkData.Add(new ChunkData(new Vector3Int(0, 0, 0), new Vector3Int(2, 2, 2)));
            
            meshEngineSaveSystem = new MeshEngineSaveSystem("C:/temp/k", "testSave", chunkData.Count);

            SetupChunk(saveIndex);

            Save();
            byte[,,] sizearray = new byte[2, 2, 2];
        }
        public void LoadIndex(int index)
        {
            Bounds bounds = new Bounds();
            bounds.min = new Vector3(0, 0, 0);
            bounds.max = new Vector3(2, 2, 2);
            bounds.size = new Vector3(2, 2, 2);
            ChunkData[,,] data = meshEngineSaveSystem.GetChunkData(bounds);
            foreach (var item in data)
            {
                Debug.Log(item.position);
                Debug.Log(item.Data);
            }
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