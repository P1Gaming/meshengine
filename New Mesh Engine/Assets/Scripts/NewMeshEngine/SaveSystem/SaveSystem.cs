using System;
using System.IO;
using UnityEngine;

namespace MeshEngine.SaveSystem
{
    internal class SaveSystem : ISaveData, IReadData, IDisposable
    {
        // How many bytes will one chunk use up,
        private readonly int chunkSizeInBytes;

        // Size of the world
        private readonly Vector3Int worldSizeInChunks;
        private readonly int numberOfChunks;

        //path to interact with
        private string saveFileFullPath;
        private static string fileExtension = ".meshchunks";

        // Keeps track on what chunk is saved.
        private int[] fileChunksIndexs;
        private int currentChunkFileIndex = 1;
        private readonly long savedChunksByteStorage;

        private FileStream fileStream;

        /// <summary>
        /// Creat a save system for saving chunks, opens a file stream to the save file during the lifetime of the object.
        /// 
        /// 
        /// .meshChunks File structure: if the numberOfChunks is 1600 (1600 ints == 6400 bytes) and a chunk type is defined by to two bytes
        /// 
        /// Byte 0 to 6400 => int[] fileChunksIndexs, for keeping track on where the chunks are saved.
        /// 
        /// byte 6401 to 6402 => first saved chunks type
        /// byte 6403 to 6404 => second saved chunks type
        /// 
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="filename"></param>
        public SaveSystem(string savePath, string filename)
        {
            worldSizeInChunks = WorldInfo.GetWorldDimensionInChunks();

            numberOfChunks = worldSizeInChunks.x * worldSizeInChunks.y * worldSizeInChunks.z;
            savedChunksByteStorage = numberOfChunks * sizeof(int);
            chunkSizeInBytes = (WorldInfo.ChunkDimensions.x * WorldInfo.ChunkDimensions.y * WorldInfo.ChunkDimensions.z) * 2;
            saveFileFullPath = Path.Combine(savePath, $"{filename}{fileExtension}");

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            bool createNewFile = !File.Exists(saveFileFullPath);

            fileStream = new FileStream(saveFileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            disposed = false;

            if (createNewFile)
            {
                CreateNewFile();
            }
            else
            {
                LoadFileChunkIndexes();
            }
        }

        #region Save to File
        /// <summary>
        /// Create a new save file, and initialize it with the FileChunkIndexs Array
        /// </summary>
        private void CreateNewFile()
        {
            fileChunksIndexs = new int[numberOfChunks];

            SaveFileChunkIndexes();
        }
        /// <summary>
        /// Saves where chunks have been saved to the beginning of the file.
        /// </summary>
        private void SaveFileChunkIndexes()
        {
            byte[] buffer = new byte[numberOfChunks * sizeof(int)];
            for (int i = 0; i < numberOfChunks; i++)
            {
                byte[] intBytes = BitConverter.GetBytes(fileChunksIndexs[i]);
                intBytes.CopyTo(buffer, i * sizeof(int));
            }
            using (BinaryWriter writer = new BinaryWriter(fileStream, System.Text.Encoding.Default, true))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                writer.Write(buffer);
            }
        }

        /// <summary>
        /// Save the specified chunkdata to the correct position in the file
        /// </summary>
        /// <param name="chunkData"></param>
        public void SaveChunkData(ChunkData chunkData)
        {
            int index = GetChunkWorldIndex(chunkData.position);

            if(index == -1)
            {
                return;
            }

            bool addToFileChunkIndexs = !IsChunkSaved(index);
            long binOffset = GetFileChunkOffset(index);

            byte[] saveData = ChunkDataByteConverter.GetSaveData(chunkData);

            WriteToFile(binOffset, saveData);
            if (addToFileChunkIndexs)
            {
                fileChunksIndexs[index] = currentChunkFileIndex;
                currentChunkFileIndex++;
                SaveFileChunkIndexes();
            }
        }

        private void WriteToFile(long binOffset, byte[] saveData)
        {
            using (BinaryWriter writer = new BinaryWriter(fileStream, System.Text.Encoding.Default, true))
            {
                fileStream.Seek(binOffset, SeekOrigin.Begin);
                writer.Write(saveData);
            }
        }
        #endregion

        #region Load from File
        /// <summary>
        /// Load the Array that keeps track on where chunks are saved. 
        /// </summary>
        public void LoadFileChunkIndexes()
        {
            using (BinaryReader reader = new BinaryReader(fileStream, System.Text.Encoding.Default, true))
            {
                fileChunksIndexs = new int[numberOfChunks];
                byte[] buffer = new byte[numberOfChunks * sizeof(int)];
                reader.Read(buffer, 0, buffer.Length);

                for (int i = 0; i < numberOfChunks; i++)
                {
                    fileChunksIndexs[i] = BitConverter.ToInt32(buffer, i * sizeof(int));
                }

            }

            UpdateCurrentChunkFileIndex();
        }

        public ChunkData[,] GetChunkData(SquareBoundXZ bounds)
        {
            int boundLength = (int)(bounds.Extent * 2);
            ChunkData[,] results = new ChunkData[boundLength + 1, boundLength + 1];

            float xMin = bounds.Center.x - bounds.Extent;
            float zMin = bounds.Center.y - bounds.Extent;
            float xMax = bounds.Center.x + bounds.Extent;
            float zMax = bounds.Center.y + bounds.Extent;

            for (int x = (int)xMin; x < (int)xMax + 1; x++)
            {
                for (int z = (int)zMin; z < (int)zMax; z++)
                {
                    Vector2Int pos = new Vector2Int(x, z);
                    if (IsOutsideWorldBounds(pos)) continue;

                    int ChunkFileIndex = GetChunkWorldIndex(new Vector3Int(pos.x, 0, pos.y));
                    ChunkData data = null;
                    if (ReadSingelChunkFromFile(ChunkFileIndex, out byte[] savedData))
                    {
                        data = new ChunkData(pos, WorldInfo.ChunkDimensions);
                        data.OverwriteBlockTypeData(ChunkDataByteConverter.ConvertByteToChunkBlockTypeData(WorldInfo.ChunkDimensions, savedData), false);
                    }

                    results[x - (int)xMin, z - (int)zMin] = data;
                }

            }
            return results;
        }

        /// <summary>
        /// Reads the saved data if the chunk is not full.
        /// </summary>
        /// <param name="chunkFileIndex"></param>
        /// <param name="data">the returned data, null if no data was read.</param>
        /// <returns>Returns true if the chunk at the position isnt empty.</returns>
        private bool ReadSingelChunkFromFile(int index, out byte[] data)
        {
            bool isSaved = IsChunkSaved(index);

            if (isSaved)
            {
                long byteOffset = GetFileChunkOffset(index);
                // Read the bytes representing the struct using a BinaryReader.
                using (BinaryReader reader = new BinaryReader(fileStream, System.Text.Encoding.Default, true))
                {
                    fileStream.Seek(byteOffset, SeekOrigin.Begin);
                    data = reader.ReadBytes(chunkSizeInBytes);
                }

            }
            else
            {
                data = null;
            }

            return isSaved;
        }
        #endregion

        #region Support methods
        private void UpdateCurrentChunkFileIndex()
        {
            currentChunkFileIndex = 1;
            for (int i = 0; i < fileChunksIndexs.Length; i++)
            {
                if (IsChunkSaved(i))
                {
                    currentChunkFileIndex++;
                }
            }
        }

        private bool IsChunkSaved(int ChunkIndex)
        {
            return fileChunksIndexs[ChunkIndex] > 0;
        }

        /// <summary>
        /// Look up if the chunkID has a defined startposition in savefile
        /// </summary>
        /// <param name="chunkID"></param>
        /// <param name="fileByteOffset">the position that should be used for this chunkID</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Out of bounds Chunk ID</exception>
        private long GetFileChunkOffset(int chunkID)
        {
            if (chunkID < 0 || chunkID >= numberOfChunks)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkID), "Invalid chunk ID.");
            }

            long fileByteOffset = savedChunksByteStorage;

            if (IsChunkSaved(chunkID))
            {
                fileByteOffset += (fileChunksIndexs[chunkID] - 1) * chunkSizeInBytes;
            }
            else
            {
                fileByteOffset += (currentChunkFileIndex - 1) * chunkSizeInBytes;
            }
            fileByteOffset++;
            return fileByteOffset;
        }

        private int GetChunkWorldIndex(Vector3Int position)
        {
            int index = 0;
            Vector3Int worldSize = WorldInfo.GetWorldDimensionInChunks();

            int zMultiplier = worldSize.x > 0 ? 1 : 0;
            int yMultiplier = 1;
            int xMultiplier = worldSize.x;

            index = position.x * xMultiplier + position.y * yMultiplier + position.z * zMultiplier;

            if (index >= numberOfChunks)
            {
                index = -1;
            }

            return index;
        }

        private bool IsOutsideWorldBounds(Vector2Int pos)
        {
            if (pos.x < 0 || pos.y < 0 || pos.x >= worldSizeInChunks.x || pos.y >= worldSizeInChunks.z)
            {
                return true;
            }
            return false;
        }
        #endregion



        #region Dispose handling

        private bool disposed;


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    fileStream?.Close();
                    disposed = true;
                }
            }
        }

        ~SaveSystem()
        {
            Dispose(false);
        }
        #endregion
    }
}
