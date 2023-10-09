using UnityEngine;

namespace RoundedVoxels
{
    internal class VoxelProvider
    {
        public VoxelSettings VoxelSettings { get; private set; }
        VoxelFaceProvider voxelFaceProvider;
        VoxelEdgeProvider voxelEdgeProvider;
        VoxelCornerProvider voxelCornerProvider;
        VoxelPatchProvider voxelPatchProvider;
        public VoxelProvider(VoxelSettings voxelSettings)
        {
            VoxelSettings = voxelSettings;
            voxelFaceProvider = new VoxelFaceProvider(voxelSettings);
            voxelEdgeProvider = new VoxelEdgeProvider(voxelSettings);
            voxelCornerProvider = new VoxelCornerProvider(voxelSettings);
            voxelPatchProvider = new VoxelPatchProvider(voxelSettings);
        }

        public void AddVoxelToMeshData(MeshData meshData, Vector3 voxelCentre, VoxelNeighbours voxelNeighbours)
        {
            for (int i = 0; i < 6; i++)
            {
                voxelFaceProvider.AddFaceToMeshData(meshData, (Face)i, voxelNeighbours, voxelCentre);
            }
            for (int i = 0; i < 12; i++)
            {
                voxelEdgeProvider.AddEdgeToMeshData(meshData, (Edge)i, voxelNeighbours, voxelCentre);
            }
            for (int i = 0; i < 8; i++)
            {
                voxelCornerProvider.AddCornerToMeshData(meshData, (Corner)i, voxelNeighbours, voxelCentre);
            }
            voxelPatchProvider.AddPatchIfRequiredToMeshData(meshData, voxelCentre, voxelNeighbours);
        }
    }
    public enum Face
    {
        Top,
        Bottom,
        Left,
        Right,
        Front,
        Back
    }
    public enum Edge
    {
        TopLeft,
        TopRight,
        TopFront,
        TopBack,
        BottomLeft,
        BottomRight,
        BottomFront,
        BottomBack,
        FrontLeft,
        FrontRight,
        BackLeft,
        BackRight
    }
    public enum Corner
    {
        FrontTopLeft,
        FrontTopRight,
        FrontBottomRight,
        FrontBottomLeft,
        BackTopRight,
        BackTopLeft,
        BackBottomLeft,
        BackBottomRight
    }
}
