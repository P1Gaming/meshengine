using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshEngine.SaveSystem
{
    public class MockSaver : MonoBehaviour
    {
        [SerializeField] Bounds bounds;
        [SerializeField] List<ChunkData> chunkData = new List<ChunkData>();
        MeshEngineSaveSystem meshEngineSaveSystem;
        ChunkData[,,] worldChunk = new ChunkData[20, 20, 20];

        private void Awake()
        {
            chunkData.Add(new ChunkData(new Vector3Int(0, 0, 0), new Vector3Int(16, 128, 16)));
            chunkData.Add(new ChunkData(new Vector3Int(0, 0, 1), new Vector3Int(16, 128, 16)));
            chunkData.Add(new ChunkData(new Vector3Int(0, 1, 0), new Vector3Int(16, 128, 16)));
            chunkData.Add(new ChunkData(new Vector3Int(10, 10, 10), new Vector3Int(16, 128, 16)));

            meshEngineSaveSystem = new MeshEngineSaveSystem("C:/temp/k", "testSave", new Vector3Int(20, 20, 20));

            SetupChunk(3);

            Save();
            byte[,,] sizearray = new byte[2, 2, 2];
        }
        public void LoadIndex(int index)
        {
            Bounds bounds = new Bounds();
            bounds.min = new Vector3(0, 0, 0);
            bounds.max = new Vector3(1, 1, 1);
            bounds.size = new Vector3(1, 1, 1);
            ChunkData[,,] data = meshEngineSaveSystem.GetChunkData(bounds);
            foreach (var item in data)
            {
                if (item != null)
                {
                    foreach (var block in item.Data)
                    {
                        if (block != BlockType.Air)
                        {
                            Debug.Log(block);

                        }
                    }
                    Debug.Log(item.position);
                    Debug.Log(item.Data);
                }
            }
        }

        private void SetupChunk(int i)
        {
            chunkData[i].AddBlockAtIndex(new Vector3Int(0, 0, 0), BlockType.debug1);
            chunkData[i].AddBlockAtIndex(new Vector3Int(0, 0, 1), BlockType.debug2);
            chunkData[i].AddBlockAtIndex(new Vector3Int(0, 1, 0), BlockType.debug3);
            chunkData[i].AddBlockAtIndex(new Vector3Int(0, 1, 1), BlockType.debug4);
            chunkData[i].AddBlockAtIndex(new Vector3Int(1, 0, 0), BlockType.debug5);
            chunkData[i].AddBlockAtIndex(new Vector3Int(1, 0, 1), BlockType.debug6);
            chunkData[i].AddBlockAtIndex(new Vector3Int(1, 1, 0), BlockType.debug7);
            chunkData[i].AddBlockAtIndex(new Vector3Int(1, 1, 1), BlockType.debug8);
        }

        public void Save()
        {
            meshEngineSaveSystem.SaveChunkData(chunkData[3]);
        }


    }
}