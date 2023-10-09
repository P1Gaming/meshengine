using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoundedVoxels
{
    internal class VoxelFaceProvider
    {
        public VoxelSettings VoxelSettings { get; private set; }
        Dictionary<FaceConfig, MeshData> meshDataDictionary;

        static Dictionary<Face, Quaternion> inverseRotationsDictionary = new();
        static Dictionary<Face, Vector3Int[]> neighboursToCheck = new()
        {
            { Face.Front, new Vector3Int[9]
                {
                    new Vector3Int(0, 0, -1),
                    new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(1, 0, 0),
                    new Vector3Int(-1, 1, 0), new Vector3Int(1, 1, 0), new Vector3Int(1, -1, 0), new Vector3Int(-1, -1, 0),
                }
            }
        };
        static Dictionary<Face, List<Rotation>> rotationsToApplyDictionary = new()
        {
            { Face.Top, new List<Rotation>() { Rotation.NegativeX } },
            { Face.Bottom, new List<Rotation>() { Rotation.PositiveX } },
            { Face.Left, new List<Rotation>() { Rotation.NegativeY } },
            { Face.Right, new List<Rotation>() { Rotation.PositiveY } },
            { Face.Front, new List<Rotation>() { } },
            { Face.Back, new List<Rotation>() { Rotation.PositiveY, Rotation.PositiveY } }
        };

        public VoxelFaceProvider(VoxelSettings voxelSettings)
        {
            VoxelSettings = voxelSettings;
            PreCalculateMeshes();
        }
        static VoxelFaceProvider()
        {
            PreCalculateRotationsAndNeighboursToCheck();
        }

        public void AddFaceToMeshData(MeshData meshData, Face face, VoxelNeighbours voxelNeighbours, Vector3 voxelCentre)
        {
            if (voxelNeighbours.GetNeighbourStatus(neighboursToCheck[face][0]))
            {
                return;
            }

            var faceMeshData = meshDataDictionary[GenerateFaceConfig(voxelNeighbours, face)];
            var rotation = inverseRotationsDictionary[face];
            int previousCount = meshData.vertices.Count;
            for (int i = 0; i < faceMeshData.vertices.Count; i++)
            {
                meshData.vertices.Add(rotation * faceMeshData.vertices[i] + voxelCentre);
            }

            for (int i = 0; i < faceMeshData.triangles.Count; i++)
            {
                meshData.triangles.Add(previousCount + faceMeshData.triangles[i]);
            }
        }
        void PreCalculateMeshes()
        {
            meshDataDictionary = new();
            for(int i=0;i<256;i++)
            {
                var config = (FaceConfig)i;
                meshDataDictionary.Add(config, CalculateMeshData(VoxelSettings.Bevel, config));
            }
        }
        static MeshData CalculateMeshData(float bevel, FaceConfig faceConfig)
        {
            List<Vector3> vertices = new();
            Vector3 topLeft = new Vector3(-0.5f, 0.5f, -0.5f);
            Vector3 topRight = new Vector3(0.5f, 0.5f, -0.5f);
            Vector3 bottomLeft = new Vector3(-0.5f, -0.5f, -0.5f);
            Vector3 bottomRight = new Vector3(0.5f, -0.5f, -0.5f);

            Vector3 bevelX = new Vector3(0.5f * bevel, 0, 0);
            Vector3 bevelY = new Vector3(0, 0.5f * bevel, 0);

            if (faceConfig.HasFlag(FaceConfig.BevelUp))
            {
                topLeft -= bevelY;
                topRight -= bevelY;
            }
            if (faceConfig.HasFlag(FaceConfig.BevelDown))
            {
                bottomLeft += bevelY;
                bottomRight += bevelY;
            }
            if (faceConfig.HasFlag(FaceConfig.BevelLeft))
            {
                topLeft += bevelX;
                bottomLeft += bevelX;
            }
            if (faceConfig.HasFlag(FaceConfig.BevelRight))
            {
                topRight -= bevelX;
                bottomRight -= bevelX;
            }

            if (faceConfig.HasFlag(FaceConfig.BevelTopLeft))
            {
                vertices.Add(topLeft + new Vector3(0, -bevel / 2, 0));
                vertices.Add(topLeft + new Vector3(bevel / 2, 0, 0));
            }
            else
            {
                vertices.Add(topLeft);
            }

            if (faceConfig.HasFlag(FaceConfig.BevelTopRight))
            {
                vertices.Add(topRight + new Vector3(-bevel / 2, 0, 0));
                vertices.Add(topRight + new Vector3(0, -bevel / 2, 0));
            }
            else
            {
                vertices.Add(topRight);
            }

            if (faceConfig.HasFlag(FaceConfig.BevelBottomRight))
            {
                vertices.Add(bottomRight + new Vector3(0, bevel / 2, 0));
                vertices.Add(bottomRight + new Vector3(-bevel / 2, 0, 0));
            }
            else
            {
                vertices.Add(bottomRight);
            }

            if (faceConfig.HasFlag(FaceConfig.BevelBottomLeft))
            {
                vertices.Add(bottomLeft + new Vector3(bevel / 2, 0, 0));
                vertices.Add(bottomLeft + new Vector3(0, bevel / 2, 0));
            }
            else
            {
                vertices.Add(bottomLeft);
            }

            List<int> triangles = new();
            for (int i = 1; i < vertices.Count - 1; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }

            return new MeshData(vertices, triangles);
        }
        static void PreCalculateRotationsAndNeighboursToCheck()
        {
            var frontFaceNeighboursToCheck = neighboursToCheck[Face.Front];
            for(int i=0;i<6;i++)
            {
                var face = (Face)i;
                var rotationsToApply = rotationsToApplyDictionary[face];
                inverseRotationsDictionary.Add(face, VoxelNeighboursOrientationSolver.GetInverseRotation(rotationsToApply));
                if (face == Face.Front)
                {
                    continue;
                }
                var neighboursToCheckEntry = new Vector3Int[9];
                for (int j = 0; j < 9; j++)
                {
                    neighboursToCheckEntry[j] = VoxelNeighboursOrientationSolver.RotateIndex(frontFaceNeighboursToCheck[j], rotationsToApply);
                }
                neighboursToCheck.Add(face, neighboursToCheckEntry);
            }
        }
        static FaceConfig GenerateFaceConfig(VoxelNeighbours voxelNeighbours, Face face)
        {
            var neighboursToCheckEntry = neighboursToCheck[face];
            bool bevelUp = !voxelNeighbours.GetNeighbourStatus(neighboursToCheckEntry[1]);
            bool bevelDown = !voxelNeighbours.GetNeighbourStatus(neighboursToCheckEntry[2]);
            bool bevelLeft = !voxelNeighbours.GetNeighbourStatus(neighboursToCheckEntry[3]);
            bool bevelRight = !voxelNeighbours.GetNeighbourStatus(neighboursToCheckEntry[4]);
            bool bevelTopLeft = !bevelUp && !bevelLeft && !voxelNeighbours.GetNeighbourStatus(neighboursToCheckEntry[5]);
            bool bevelTopRight = !bevelUp && !bevelRight && !voxelNeighbours.GetNeighbourStatus(neighboursToCheckEntry[6]);
            bool bevelBottomRight = !bevelRight && !bevelDown && !voxelNeighbours.GetNeighbourStatus(neighboursToCheckEntry[7]);
            bool bevelBottomLeft = !bevelDown && !bevelLeft && !voxelNeighbours.GetNeighbourStatus(neighboursToCheckEntry[8]);

            var faceConfig = new FaceConfig();
            if (bevelUp)
            {
                faceConfig |= FaceConfig.BevelUp;
            }
            if (bevelDown)
            {
                faceConfig |= FaceConfig.BevelDown;
            }
            if (bevelLeft)
            {
                faceConfig |= FaceConfig.BevelLeft;
            }
            if (bevelRight)
            {
                faceConfig |= FaceConfig.BevelRight;
            }
            if (bevelTopLeft)
            {
                faceConfig |= FaceConfig.BevelTopLeft;
            }
            if (bevelTopRight)
            {
                faceConfig |= FaceConfig.BevelTopRight;
            }
            if (bevelBottomRight)
            {
                faceConfig |= FaceConfig.BevelBottomRight;
            }
            if (bevelBottomLeft)
            {
                faceConfig |= FaceConfig.BevelBottomLeft;
            }

            return faceConfig;
        }
        enum FaceConfig
        {
            BevelUp = 1 << 0,
            BevelDown = 1 << 1,
            BevelLeft = 1 << 2,
            BevelRight = 1 << 3,
            BevelTopLeft = 1 << 4,
            BevelTopRight = 1 << 5,
            BevelBottomRight = 1 << 6,
            BevelBottomLeft = 1 << 7
        }
    }
}
