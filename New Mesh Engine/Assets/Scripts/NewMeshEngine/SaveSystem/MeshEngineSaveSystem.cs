using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace MeshEngine.SaveSystem
{


    public class MeshEngineSaveSystem : ISaveData, IReadData
    {
        static int xMultiplier = 27;
        static int yMultiplier = 9;
        static int zMultiplier = 3;

        string fullPath;

        private readonly Vector3Int chunkSize = new Vector3Int(16, 128, 16);
        private readonly int chunkSizeInBytes;
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        public MeshEngineSaveSystem(string savePath, string filename, int worldSize)
        {
            chunkSizeInBytes = chunkSize.x * chunkSize.y * chunkSize.z + 1;

            fullPath = $"{savePath}/{filename}.meshchunks";

            Directory.CreateDirectory(savePath);
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            bool createNewFile = !File.Exists(fullPath);


            if (createNewFile)
            {
                using (Stream stream = new FileStream(fullPath, FileMode.Create,FileAccess.ReadWrite))
                {
                    ChunkSaveData[] saveData = new ChunkSaveData[worldSize];
                    for (int i = 0; i < worldSize; i++)
                    {
                        saveData[i] = new ChunkSaveData()
                        {
                            isEmpty = true,
                            blockType = new byte[chunkSizeInBytes]
                        };
                    }
                    binaryFormatter.Serialize(stream, saveData);
                    stream.Close();
                }
            }
        }

        public void SaveChunkData(ChunkData chunkData)
        {
            long binOffset = GetChunkFileIndex(chunkData.position)*chunkSizeInBytes;
            ChunkSaveData saveData = chunkData.GetSaveData();

            using (FileStream stream = File.Open(fullPath, FileMode.Open))
            {
                stream.Seek(binOffset, SeekOrigin.Begin);

                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    binaryFormatter.Serialize(stream, saveData);
                }
            }

        }

        
        int GetChunkFileIndex(Vector3Int pos)
        {
            return pos.x * xMultiplier + pos.y * yMultiplier + pos.z * zMultiplier;
        }

        ChunkSaveData ReadSingelChunkFromFile(int chunkFileIndex)
        {
            ChunkSaveData results;
            // The path to the binary file.
            string filePath = fullPath;

            // The index of the struct to retrieve.
            int index = chunkFileIndex;

            // The size of each struct in bytes.
            int structSize = chunkSizeInBytes;
            long byteOffest = structSize * index;

            // Open the binary file using a FileStream.
            using (FileStream stream = File.Open(filePath, FileMode.Open))
            {

                // Position the stream at the appropriate location for the desired struct.
                stream.Seek(byteOffest, SeekOrigin.Begin);


                // Read the bytes representing the struct using a BinaryReader.
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    byte[] bytes = reader.ReadBytes(structSize);

                    binaryFormatter = new BinaryFormatter();

                    // Create a new MemoryStream from the bytes and deserialize the struct.
                    MemoryStream memoryStream = new MemoryStream(bytes);
                    results = (ChunkSaveData)binaryFormatter.Deserialize(memoryStream);
                }

            }
            return results;
        }

        public ChunkData[,,] GetChunkData(Bounds bounds)
        {
            ChunkData[,,] results = new ChunkData[(int)bounds.size.x,(int)bounds.size.y,(int)bounds.size.z];

            for(int x = (int)bounds.min.x; x < (int)bounds.max.y; x++)
            {
                for(int y = (int)bounds.min.y; y < (int)bounds.max.y; y++)
                {
                    for(int z = (int)bounds.min.z; z < (int)bounds.max.z; y++)
                    {
                        Vector3Int pos = new Vector3Int(x, y, z);
                        int ChunkFileIndex = GetChunkFileIndex(pos);
                        ChunkSaveData savedData = ReadSingelChunkFromFile(ChunkFileIndex);
                        ChunkData data = new ChunkData(pos, chunkSize, savedData);
                        results[x, y, z] = data;
                    }
                }
            }
            return results;
        }
    }

    [Serializable]
    public struct ChunkSaveData
    {
        public bool isEmpty;

        public byte[] blockType; // Blocktype as byte
    }
}
