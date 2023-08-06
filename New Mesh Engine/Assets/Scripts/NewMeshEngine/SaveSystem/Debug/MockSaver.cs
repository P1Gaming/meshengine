using System.Collections.Generic;
using UnityEngine;

namespace MeshEngine.SaveSystem.Testing
{
    public class MockSaver : MonoBehaviour
    {
        [SerializeField] Bounds bounds;
        [SerializeField] List<ChunkData> chunkData = new List<ChunkData>();

        [SerializeField] string path = "C:/temp/meshengine";
        public Vector2 pos = new Vector2(2,2);

        IReadData readData;
        ISaveData saveData;

        string threeDFileName = "test3D";


        private void Awake()
        {
            chunkData.Add(new ChunkData(new Vector2Int(0, 0), new Vector3Int(16, 256, 16)));
            chunkData.Add(new ChunkData(new Vector2Int(0, 1), new Vector3Int(16, 256, 16)));
            chunkData.Add(new ChunkData(new Vector2Int(2, 0), new Vector3Int(16, 256, 16)));
            chunkData.Add(new ChunkData(new Vector2Int(9, 9), new Vector3Int(16, 256, 16)));
            chunkData.Add(new ChunkData(new Vector2Int(39, 39), new Vector3Int(16, 256, 16)));
            readData = new SaveSystem(path, threeDFileName);
            saveData = (SaveSystem)readData;

            SetupChunk(4);

            Save(4);
        }

        public void loadData()
        {
                LoadThreeDimensions();
        }

        private void LoadThreeDimensions()
        {
            SquareBoundXZ bounds = new SquareBoundXZ(pos, 4);
            ChunkData[,] data = readData.GetChunkData(bounds);
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

        public void Save(int i)
        {
            saveData.SaveChunkData(chunkData[i]);
        }


    }
}