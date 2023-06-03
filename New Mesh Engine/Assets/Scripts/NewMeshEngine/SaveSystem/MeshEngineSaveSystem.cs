using System;
using System.IO;
using UnityEngine;

namespace MeshEngine.SaveSystem
{


    internal class MeshEngineSaveSystem : ISaveData, IReadData
    {
        readonly int xMultiplier;
        readonly int yMultiplier;
        readonly int zMultiplier;

        string fullPath;

        private readonly Vector3Int chunkSize = new Vector3Int(16, 128, 16);
        private readonly int chunkSizeInBytes;
        private readonly Vector3Int worldSize;

        public MeshEngineSaveSystem(string savePath, string filename, Vector3Int worldSize)
        {
            this.worldSize = worldSize;
            int chunks = worldSize.x*worldSize.y*worldSize.z;
            zMultiplier = worldSize.z > 0 ? 1 : 0;
            yMultiplier = worldSize.x;
            xMultiplier = worldSize.y * worldSize.z;

            chunkSizeInBytes = (chunkSize.x * chunkSize.y * chunkSize.z)*2+1;

            fullPath = $"{savePath}/{filename}.meshchunks";

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            bool createNewFile = !File.Exists(fullPath);


            if (createNewFile)
            {
                using (Stream stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        
                        for (int i = 0; i < chunks; i++)
                        {
                            byte[] saveData = new byte[chunkSizeInBytes];

                            saveData[0] = 1;

                            writer.Write(saveData);
                        }
                    }
                    stream.Close();
                }
            }
        }

        public void SaveChunkData(ChunkData chunkData)
        {
            long binOffset = GetChunkByteIndex(chunkData.position) * chunkSizeInBytes;
            byte[] saveData = ChunkDataByteConverter.GetSaveData(chunkData);

            using (FileStream stream = File.Open(fullPath, FileMode.Open))
            {
                stream.Seek(binOffset, SeekOrigin.Begin);

                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(saveData);
                }
            }

        }


        int GetChunkByteIndex(Vector3Int pos)
        {
            return pos.x * xMultiplier + pos.y * yMultiplier + pos.z * zMultiplier;
        }

        bool ReadSingelChunkFromFile(int chunkFileIndex, out byte[] data)
        {
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
                    if(reader.ReadBoolean()) 
                    {
                        data = null;
                        return false;
                    }
                    stream.Seek(-1, SeekOrigin.Current);
                    data = reader.ReadBytes(structSize);
                }

            }
            
            return true;
        }

        public ChunkData[,,] GetChunkData(Bounds bounds)
        {
            ChunkData[,,] results = new ChunkData[(int)bounds.size.x+1, (int)bounds.size.y+1, (int)bounds.size.z+1];

            for (int x = (int)bounds.min.x; x < (int)bounds.max.x+1; x++)
            {
                for (int y = (int)bounds.min.y; y < (int)bounds.max.y+1; y++)
                {
                    for (int z = (int)bounds.min.z; z < (int)bounds.max.z+1; z++)
                    {
                        Vector3Int pos = new Vector3Int(x, y, z);
                        int ChunkFileIndex = GetChunkByteIndex(pos);
                        ChunkData data = null;
                        if (ReadSingelChunkFromFile(ChunkFileIndex, out byte[] savedData)) 
                        {
                            data = new ChunkData(pos, chunkSize);
                            data.OverwriteBlockTypeData(ChunkDataByteConverter.ConvertByteToChunkBlockTypeData(pos,chunkSize,savedData),false);
                        }

                        //ChunkData chunkData = new ChunkData(pos,chunkSize);
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
