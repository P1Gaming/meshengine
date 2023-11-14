using UnityEngine;

namespace RoundedVoxels
{
    internal struct VoxelSettings
    {
        public float Bevel { get; private set; }
        public int Smoothness { get; private set; }
        public VoxelSettings(float bevel, int smoothness)
        {
            Bevel = Mathf.Clamp01(bevel);
            Smoothness = Mathf.Clamp(smoothness, 0, 10);
        }
    }
}
