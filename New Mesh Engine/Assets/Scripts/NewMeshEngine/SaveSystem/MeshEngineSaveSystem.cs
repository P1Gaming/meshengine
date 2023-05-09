using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace MeshEngine.SaveSystem
{


    public class MeshEngineSaveSystem : ISaveData
    {
        static int xMultiplier = 3;
        static int yMultiplier = 9;
        static int zMultiplier = 27;

        string fullPath;

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        public MeshEngineSaveSystem(string savePath, string filename, int worldSize)
        {
            fullPath = $"{savePath}/{filename}.meshchunks";

            Directory.CreateDirectory(savePath);
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            bool createNewFile = !File.Exists(fullPath);


            if (createNewFile)
            {
                using (Stream stream = new FileStream(fullPath, FileMode.Create))
                {
                    ChunkSaveData[] saveData = new ChunkSaveData[worldSize];
                    for (int i = 0; i < worldSize; i++)
                    {
                        saveData[i] = new ChunkSaveData()
                        {
                            isEmpty = true
                        };
                    }
                    binaryFormatter.Serialize(stream, saveData);
                    stream.Close();
                    //byte[] datas = new byte[worldSize * 2];

                    //for (int i = 0; i < datas.Length; i++)
                    //{
                    //    datas[i] = i % 2 == 0 ? (byte)0 : (byte)255;
                    //    stream.WriteByte(datas[i]);
                    //}
                }

            }


        }
        public void SaveChunkData(ChunkData chunkData)
        {
            string[] lines = File.ReadAllLines(fullPath);
            Vector3Int pos = chunkData.position;
            int lineIndex = pos.x * xMultiplier + pos.y * yMultiplier + pos.z * zMultiplier;

            byte[] binDatas = new byte[chunkData.Data.Length + 1];

            using (Stream stream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                BinaryWriter writer = new BinaryWriter(stream);

                string line = lines[lineIndex];
                StringBuilder newLine = new StringBuilder();

                if (chunkData.isEmpty)
                {
                    // empty line
                    writer.Write(0);
                }
                else
                {
                    writer.Write(1);
                    for (int z = 0; z < chunkData.Data.GetLength(2); z++)
                    {
                        for (int y = 0; y < chunkData.Data.GetLength(1); y++)
                        {
                            for (int x = 0; x >= chunkData.Data.GetLength(0); x++)
                            {
                                writer.Write((byte)chunkData.Data[x, y, z]);
                            }
                        }
                    }
                }
            }

        }
        void readSingelChunkFromFile()
        {
            // The path to the binary file.
            string filePath = "example.bin";

            // The index of the struct to retrieve.
            int index = 3;
            var tempChunk = new ChunkSaveData();
            // The size of each struct in bytes.
            int structSize = 32769;

            // Open the binary file using a FileStream.
            using (FileStream stream = File.Open(filePath, FileMode.Open))
            {

                // Position the stream at the appropriate location for the desired struct.
                stream.Seek(index * structSize, SeekOrigin.Begin);

                // Read the bytes representing the struct using a BinaryReader.
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    byte[] bytes = reader.ReadBytes(structSize);

                    // Create a new MemoryStream from the bytes and deserialize the struct.
                    MemoryStream memoryStream = new MemoryStream(bytes);
                    BinaryFormatter formatter = new BinaryFormatter();
                    ChunkSaveData myStruct = (ChunkSaveData)formatter.Deserialize(memoryStream);

                    // Use the struct as needed.
                    Console.WriteLine("The value of MyProperty in the retrieved struct is: " + myStruct.blockType);
                }
            }

            Console.WriteLine("Done!");
        }

    }


    //[StructLayout(LayoutKind.Sequential,Pack =1)]
    internal struct ChunkSaveData
    {
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public bool isEmpty;

        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32768)]
        public Byte[,,] blockType; // Blocktype as byte
    }
}
