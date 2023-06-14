using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace MeshEngine.SaveSystem.Testing
{
    public class MockSaver : MonoBehaviour
    {
        [SerializeField] Bounds bounds;
        [SerializeField] List<ChunkData> chunkData = new List<ChunkData>();

        [SerializeField] string path = "C:/temp/meshengine";

        [SerializeField] bool testThreeDimensions;

        MeshEngineSaveSystem meshEngineSaveSystemTwoDimensions;

        string twoDFileName = "test2D";
        string threeDFileName = "test3D";


        private void Awake()
        {

            if (testThreeDimensions)
            {
                chunkData.Add(new ChunkData(new Vector3Int(0, 0, 0), new Vector3Int(16, 256, 16)));
                chunkData.Add(new ChunkData(new Vector3Int(0, 0, 1), new Vector3Int(16, 256, 16)));
                chunkData.Add(new ChunkData(new Vector3Int(2, 0, 0), new Vector3Int(16, 256, 16)));
                chunkData.Add(new ChunkData(new Vector3Int(9, 0, 9), new Vector3Int(16, 256, 16)));
                meshEngineSaveSystemTwoDimensions = new MeshEngineSaveSystem(path, threeDFileName, new Vector3Int(20, 20, 20));
            }
            else
            {
                chunkData.Add(new ChunkData(new Vector2Int(0, 0), new Vector3Int(16, 256, 16)));
                chunkData.Add(new ChunkData(new Vector2Int(0, 1), new Vector3Int(16, 256, 16)));
                chunkData.Add(new ChunkData(new Vector2Int(2, 0), new Vector3Int(16, 256, 16)));
                chunkData.Add(new ChunkData(new Vector2Int(9, 9), new Vector3Int(16, 256, 16)));
                meshEngineSaveSystemTwoDimensions = new MeshEngineSaveSystem(path, twoDFileName, new Vector2Int(20, 20));
            }

            SetupChunk(0);

            Save();
        }

        public void loadData()
        {
            if (testThreeDimensions)
            {
                LoadThreeDimensions();
            }
            else
            {
                Loadtwodimension();
            }
        }
        private void Loadtwodimension()
        {
            SquareBoundXZ bounds = new SquareBoundXZ();
            bounds.Center = new Vector2(0.5f, 0.5f);
            bounds.Size = 3;
            ChunkData[,] data = meshEngineSaveSystemTwoDimensions.GetChunkData(bounds);
            foreach (var item in data)
            {
                if (item != null)
                {
                    foreach (var block in item.Data)
                    {
                        if (block != BlockType.Air)
                        {
                            Debug.Log(block + " in " + item.position);
                        }
                    }
                    Debug.Log(item.position);
                    Debug.Log(item.Data);
                }
            }
        }
        private void LoadThreeDimensions()
        {
            Bounds bounds = new Bounds();
            bounds.min = new Vector3(0, 0, 0);
            bounds.max = new Vector3(1, 1, 1);
            bounds.size = new Vector3(1, 1, 1);
            ChunkData[,,] data = meshEngineSaveSystemTwoDimensions.GetChunkData(bounds);
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
            meshEngineSaveSystemTwoDimensions.SaveChunkData(chunkData[0]);
        }


    }
}