using System;
using System.IO;
using UnityEngine;

namespace MeshEngine.SaveSystem
{

    internal class OldImplementationMeshEngineSaveSystem : ISaveData, IReadData
    {
        // the chunk size, used to calculate byte size. Prehaps read this from the world at later date
        private readonly Vector3Int chunkSize = new Vector3Int(16, 256, 16);

        //Multipiers used to calulate a chunks index in the world
        private readonly int yMultiplier;
        private readonly int zMultiplier;
        private readonly int xMultiplier;

        // How many bytes will one chunk use up,
        private readonly int chunkSizeInBytes;

        // Size of the world, needed to not read outside of index
        private readonly Vector3Int worldSize;

        
        private string saveFileFullPath;
        

        /// <summary>
        /// Create a save system ment for saving none stacking chunks
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="filename"></param>
        /// <param name="worldSize"></param>
        public OldImplementationMeshEngineSaveSystem(string savePath, string filename, Vector2Int worldSize)
        {
            this.worldSize = new Vector3Int(worldSize.x,1,worldSize.y);
            
            //Vector3Int newWorldSize = new Vector3Int(worldSize.x, 1, worldSize.y);
            int chunks = worldSize.x * worldSize.y;
            zMultiplier = worldSize.x > 0 ? 1 : 0;
            yMultiplier = 1;
            xMultiplier = worldSize.x;

            chunkSizeInBytes = (chunkSize.x * chunkSize.y * chunkSize.z) * 2 + 1;

            saveFileFullPath = $"{savePath}/{filename}.meshchunks";

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            bool createNewFile = !File.Exists(saveFileFullPath);


            if (createNewFile)
            {
                CreateNewFile(chunks);
            }
        }

        /// <summary>
        /// Creat a save system ment for saving stacking chunks
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="filename"></param>
        /// <param name="worldSize"></param>
        public OldImplementationMeshEngineSaveSystem(string savePath, string filename, Vector3Int worldSize)
        {
            this.worldSize = worldSize;

            int chunks = worldSize.x * worldSize.y * worldSize.z;
            zMultiplier = worldSize.z > 0 ? 1 : 0;
            yMultiplier = worldSize.x;
            xMultiplier = worldSize.y * worldSize.z;

            chunkSizeInBytes = (chunkSize.x * chunkSize.y * chunkSize.z) * 2 + 1;

            saveFileFullPath = $"{savePath}/{filename}.meshchunks";

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            bool createNewFile = !File.Exists(saveFileFullPath);


            if (createNewFile)
            {
                CreateNewFile(chunks);
            }
        }

        /// <summary>
        /// Create a new save file, fills it up with the amount of bytes needed to store all chunks
        /// </summary>
        /// <param name="chunks"></param>
        private void CreateNewFile(int chunks)
        {
            using (Stream stream = new FileStream(saveFileFullPath, FileMode.Create, FileAccess.Write))
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

        /// <summary>
        /// Save the specified chunkdata to the correct position in the file
        /// </summary>
        /// <param name="chunkData"></param>
        public bool SaveChunkData(ChunkData chunkData)
        {
            long binOffset = GetChunkByteIndex(chunkData.position) * chunkSizeInBytes;
            byte[] saveData = ChunkDataByteConverter.GetSaveData(chunkData);

            using (FileStream stream = File.Open(saveFileFullPath, FileMode.Open))
            {
                stream.Seek(binOffset, SeekOrigin.Begin);

                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(saveData);
                }
            }

            return true;
        }


        private int GetChunkByteIndex(Vector3Int pos)
        {
            return pos.x * xMultiplier + pos.y * yMultiplier + pos.z * zMultiplier;
        }

        /// <summary>
        /// Reads the saved data if the chunk is not full.
        /// </summary>
        /// <param name="chunkFileIndex"></param>
        /// <param name="data">the returned data, null if no data was read.</param>
        /// <returns>Returns true if the chunk at the position isnt empty.</returns>
        private bool ReadSingelChunkFromFile(int index, out byte[] data)
        {
            long byteOffest = chunkSizeInBytes * index;

            // Open the binary file using a FileStream.
            using (FileStream stream = File.Open(saveFileFullPath, FileMode.Open))
            {

                // Position the stream at the appropriate location for the desired struct.
                stream.Seek(byteOffest, SeekOrigin.Begin);

                // Read the bytes representing the struct using a BinaryReader.
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    if (reader.ReadBoolean())
                    {
                        data = null;
                        return false;
                    }
                    stream.Seek(-1, SeekOrigin.Current);
                    data = reader.ReadBytes(chunkSizeInBytes);
                }

            }

            return true;
        }

        
        public ChunkData[,,] GetChunkData(Bounds bounds)
        {
            ChunkData[,,] results = new ChunkData[(int)bounds.size.x + 1, (int)bounds.size.y + 1, (int)bounds.size.z + 1];
            
            for (int x = (int)bounds.min.x; x < (int)bounds.max.x + 1; x++)
            {
                for (int y = (int)bounds.min.y; y < (int)bounds.max.y + 1; y++)
                {
                    for (int z = (int)bounds.min.z; z < (int)bounds.max.z + 1; z++)
                    {
                        Vector3Int pos = new Vector3Int(x, y, z);
                        if (IsOutsideWorldBounds(pos)) continue;
                        
                        int ChunkFileIndex = GetChunkByteIndex(pos);
                        ChunkData data = null;
                        if (ReadSingelChunkFromFile(ChunkFileIndex, out byte[] savedData))
                        {
                            data = new ChunkData(pos, chunkSize);
                            data.OverwriteBlockTypeData(ChunkDataByteConverter.ConvertByteToChunkBlockTypeData(pos, savedData), false);
                        }

                        //ChunkData chunkData = new ChunkData(pos,chunkSize);
                        results[x, y, z] = data;
                    }
                }
            }
            return results;
        }

        private bool IsOutsideWorldBounds(Vector3Int pos)
        {
            if (pos.x < 0 || pos.y <  0 || pos.z < 0 || pos.x >=worldSize.x || pos.y >=worldSize.y || pos.z >= worldSize.z)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets chunkdate within the specified SquareBounds
        /// </summary>
        /// <param name="squareBounds"></param>
        /// <returns></returns>
        public ChunkData[,] GetChunkData(SquareBoundXZ squareBounds)
        {
            Bounds bounds = new Bounds();
            bounds.center = new Vector3(squareBounds.Center.x, 0, squareBounds.Center.y);
            bounds.extents = new Vector3(squareBounds.Extent, 0, squareBounds.Extent);

            ChunkData[,,] chunkDatas = GetChunkData(bounds);

            ChunkData[,] results = new ChunkData[chunkDatas.GetLength(0), chunkDatas.GetLength(2)];
            for (int x = 0; x < results.GetLength(0); x++)
            {
                for (int z = 0; z < results.GetLength(1); z++)
                {
                    results[x, z] = chunkDatas[x, 0, z];
                }
            }
            return results;
        }

    }

}
