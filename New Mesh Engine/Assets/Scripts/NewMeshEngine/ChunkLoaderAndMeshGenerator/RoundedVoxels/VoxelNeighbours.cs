using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoundedVoxels
{
    internal class VoxelNeighbours
    {
        bool[,,] neighboursStatus = new bool[3, 3, 3];
        public bool IsCompletelyHidden { get; private set; }
        public VoxelNeighbours(bool[,,] allVoxels, Vector3Int voxelIndex)
        {
            int lengthX = allVoxels.GetLength(0);
            int lengthY = allVoxels.GetLength(1);
            int lengthZ = allVoxels.GetLength(2);
            bool isCompletelyHidden = true;

            for(int i=-1;i<2;i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    for (int k = -1; k < 2; k++)
                    {
                        if (voxelIndex.x + i < 0 || voxelIndex.x + i > lengthX - 1 || 
                            voxelIndex.y + j < 0 || voxelIndex.y + j > lengthY - 1 || 
                            voxelIndex.z + k < 0 || voxelIndex.z + k > lengthZ - 1)
                        {
                            isCompletelyHidden = false;
                            continue;
                        }

                        bool isActive = allVoxels[voxelIndex.x + i, voxelIndex.y + j, voxelIndex.z + k];
                        if(!isActive)
                        {
                            isCompletelyHidden = false;
                        }
                        neighboursStatus[i + 1, j + 1, k + 1] = isActive;
                    }
                }
            }

            IsCompletelyHidden = isCompletelyHidden;
        }
        public VoxelNeighbours(BlockType[,,] allVoxels, Vector3Int voxelIndex)
        {
            int lengthX = allVoxels.GetLength(0);
            int lengthY = allVoxels.GetLength(1);
            int lengthZ = allVoxels.GetLength(2);

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    for (int k = -1; k < 2; k++)
                    {
                        if (voxelIndex.x + i < 0 || voxelIndex.x + i > lengthX - 1 ||
                            voxelIndex.y + j < 0 || voxelIndex.y + j > lengthY - 1 ||
                            voxelIndex.z + k < 0 || voxelIndex.z + k > lengthZ - 1)
                        {
                            continue;
                        }

                        neighboursStatus[i + 1, j + 1, k + 1] = allVoxels[voxelIndex.x + i, voxelIndex.y + j, voxelIndex.z + k] != BlockType.Air;
                    }
                }
            }
        }

        public bool GetNeighbourStatus(int i, int j, int k)
        {
            if(i < -1 || i > 1 || j < -1 || j > 1 || k < -1 || k > 1)
            {
                return false;
            }

            return neighboursStatus[i + 1, j + 1, k + 1];
        }
        public bool GetNeighbourStatus(Vector3Int relativeIndex)
        {
            if (relativeIndex.x < -1 || relativeIndex.x > 1 || relativeIndex.y < -1 || relativeIndex.y > 1 || relativeIndex.z < -1 || relativeIndex.z > 1)
            {
                return false;
            }

            return neighboursStatus[relativeIndex.x + 1, relativeIndex.y + 1, relativeIndex.z + 1];
        }
    }
}
