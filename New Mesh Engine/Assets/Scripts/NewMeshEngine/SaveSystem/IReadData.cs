using UnityEngine;

namespace MeshEngine.SaveSystem
{

    internal interface IReadData
    {
        public ChunkData[,] GetChunkData(SquareBoundXZ bounds);
    }
}