using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RoundedVoxels
{
    internal class VoxelPatchProvider
    {
        public VoxelSettings VoxelSettings { get; private set; }
        Dictionary<PatchType, MeshData> meshDataDictionary;

        static Dictionary<Corner, Vector3Int[]> neighboursToCheck;
        static Dictionary<Corner, Quaternion> inverseRotationsDictionary;
        public VoxelPatchProvider(VoxelSettings voxelSettings)
        {
            VoxelSettings = voxelSettings;
            PreCalculateMeshes();
        }

        static VoxelPatchProvider()
        {
            PreCalculateRotationsAndNeighboursToCheck();
        }

        public void AddPatchIfRequiredToMeshData(MeshData meshData, Vector3 voxelCentre, VoxelNeighbours voxelNeighbours)
        {
            for(int i=0;i<8;i++)
            {
                var corner = (Corner)i;
                var neighboursToCheckEntry = neighboursToCheck[corner];
                for (int j=0;j<7;j++)
                {
                    var patchType = (PatchType)j;
                    if(CheckConditionsForPatch(patchType, voxelNeighbours, neighboursToCheckEntry))
                    {
                        AddPatchToMeshData(meshData, corner, patchType, voxelCentre);
                        continue;
                    }
                }
            }
        }

        void AddPatchToMeshData(MeshData meshData, Corner corner, PatchType patchType, Vector3 voxelCentre)
        {
            var rotation = inverseRotationsDictionary[corner];
            int previousCount = meshData.vertices.Count;
            var patchMeshData = meshDataDictionary[patchType];
            for (int k = 0; k < patchMeshData.vertices.Count; k++)
            {
                meshData.vertices.Add(rotation * patchMeshData.vertices[k] + voxelCentre);
            }

            for (int k = 0; k < patchMeshData.triangles.Count; k++)
            {
                meshData.triangles.Add(previousCount + patchMeshData.triangles[k]);
            }
        }

        void PreCalculateMeshes()
        {
            meshDataDictionary = new();
            for(int i=0;i<7;i++)
            {
                var patchType = (PatchType)i;
                meshDataDictionary[patchType] = CalculateMesh(patchType, VoxelSettings.Bevel, VoxelSettings.Smoothness);
            }
        }
        static void PreCalculateRotationsAndNeighboursToCheck()
        {
            inverseRotationsDictionary = VoxelCornerProvider.GetInverseRotationsDictionaryCopy();
            neighboursToCheck = VoxelCornerProvider.GetNeighboursToCheckCopy();


        }
        static MeshData CalculateMesh(PatchType patchType, float bevel, int smoothness)
        {
            Quaternion rotation;
            Vector3 cornerPoint = new Vector3(0.5f, 0.5f, -0.5f);
            switch (patchType)
            {
                case PatchType.Type1Case1:
                    return CalculateMeshDataForPatchType1(bevel, smoothness, Quaternion.identity);
                case PatchType.Type1Case2:
                    rotation = Quaternion.AngleAxis(120, new Vector3(0.5f, 0.5f, -0.5f).normalized);
                    return CalculateMeshDataForPatchType1(bevel, smoothness,rotation);
                case PatchType.Type1Case3:
                    rotation = Quaternion.AngleAxis(-120, new Vector3(0.5f, 0.5f, -0.5f).normalized);
                    return CalculateMeshDataForPatchType1(bevel, smoothness, rotation);
                case PatchType.Type2:
                    return CalculateMeshDataForPatchType2(bevel, smoothness);
                case PatchType.Type3Case1:
                    return CalculateMeshDataForPatchType3(bevel, smoothness, Quaternion.identity, Vector3.zero);
                case PatchType.Type3Case2:
                    rotation = Quaternion.AngleAxis(-90, Vector3.up) * Quaternion.AngleAxis(90, Vector3.forward);
                    return CalculateMeshDataForPatchType3(bevel, smoothness, rotation, cornerPoint);
                case PatchType.Type3Case3:
                    rotation = Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(90, Vector3.right);
                    return CalculateMeshDataForPatchType3(bevel, smoothness, rotation, cornerPoint);
                default:
                    return new MeshData();
            }
        }
        static MeshData CalculateMeshDataForPatchType1(float bevel, int smoothness, Quaternion rotation)
        {
            List<Vector3> vertices = new();
            List<int> triangles = new();

            var point1 = new Vector3(0.5f, 0.5f, -0.5f + bevel / 2);
            var point2 = point1 + new Vector3(0, -bevel / 2, -bevel / 2);
            var point3 = new Vector3(0.5f - bevel / 2, 0.5f, -0.5f);
            var rotatingVectorForPoint1 = point1 + new Vector3(0, -bevel / 2, 0);
            var rotatingVectorForPoint3 = point3 + new Vector3(0, -bevel / 2, 0);

            point1 = rotation * point1;
            point2 = rotation * point2;
            point3 = rotation * point3;
            rotatingVectorForPoint1 = rotation * rotatingVectorForPoint1;
            rotatingVectorForPoint3 = rotation * rotatingVectorForPoint3;

            var line1 = GetLine(point1, point2, smoothness + 2, rotatingVectorForPoint1, bevel / 2);
            var line2 = GetLine(point3, point2, smoothness + 2, rotatingVectorForPoint3, bevel / 2);

            for (int i = 0; i < smoothness; i++)
            {
                AddTriangle(line2[i], line1[i], line1[i + 1], vertices, triangles);
                AddTriangle(line2[i], line1[i + 1], line2[i + 1], vertices, triangles);
            }

            AddTriangle(line2[smoothness], line1[smoothness], point2, vertices, triangles);
            return new MeshData(vertices, triangles);
        }
        static MeshData CalculateMeshDataForPatchType2(float bevel, int smoothness)
        {
            List<Vector3> vertices = new();
            List<int> triangles = new();

            var point1 = new Vector3(0.5f, 0.5f, -0.5f + bevel / 2);
            var point2 = point1 + new Vector3(0, -bevel / 2, -bevel / 2);
            var point3 = new Vector3(0.5f - bevel / 2, 0.5f, -0.5f);

            var rotatingVectorForPoint1 = point1 + new Vector3(0, -bevel / 2, 0);
            var rotatingVectorForPoint3 = point3 + new Vector3(0, -bevel / 2, 0);
            var rotatingVectorForBase = point1 + new Vector3(-bevel / 2, 0, 0);

            var line1 = GetLine(point1, point2, smoothness + 2, rotatingVectorForPoint1, bevel / 2);
            var line2 = GetLine(point3, point2, smoothness + 2, rotatingVectorForPoint3, bevel / 2);
            var line3 = GetLine(point1, point3, smoothness + 2, rotatingVectorForBase, bevel / 2);

            int quadCountPerJunction = (smoothness + smoothness % 2) / 2 - 1;
            for (int i = 0; i < quadCountPerJunction; i++)
            {
                AddTriangle(line1[i + 1], line1[i + 2], line3[i + 2], vertices, triangles);
                AddTriangle(line1[i + 1], line3[i + 2], line3[i + 1], vertices, triangles);
            }

            AddTriangle(point1, line1[1], line3[1], vertices, triangles);

            for (int i = 0; i < quadCountPerJunction; i++)
            {
                AddTriangle(line1[smoothness + 1 - (i + 2)], line1[smoothness + 1 - (i + 1)], line2[smoothness + 1 - (i + 1)], vertices, triangles);
                AddTriangle(line1[smoothness + 1 - (i + 2)], line2[smoothness + 1 - (i + 1)], line2[smoothness + 1 - (i + 2)], vertices, triangles);
            }

            AddTriangle(line1[smoothness], point2, line2[smoothness], vertices, triangles);

            for (int i = 0; i < quadCountPerJunction; i++)
            {
                AddTriangle(line2[i + 1], line3[smoothness + 1 - (i + 1)], line3[smoothness + 1 - (i + 2)], vertices, triangles);
                AddTriangle(line2[i + 1], line3[smoothness + 1 - (i + 2)], line2[i + 2], vertices, triangles);
            }

            AddTriangle(line2[1], point3, line3[smoothness], vertices, triangles);
            if (smoothness % 2 == 1)
            {
                AddTriangle(line1[quadCountPerJunction + 1], line2[quadCountPerJunction + 1], line3[quadCountPerJunction + 1], vertices, triangles);
                return new MeshData(vertices, triangles);
            }

            int half = smoothness / 2;
            AddTriangle(line3[half], line1[half], line3[half + 1], vertices, triangles);
            AddTriangle(line1[half], line1[half + 1], line2[half + 1], vertices, triangles);
            AddTriangle(line2[half + 1], line2[half], line3[half + 1], vertices, triangles);
            AddTriangle(line1[half], line2[half + 1], line3[half + 1], vertices, triangles);
            return new MeshData(vertices, triangles);
        }
        static MeshData CalculateMeshDataForPatchType3(float bevel, int smoothness, Quaternion rotation, Vector3 pivot)
        {
            List<Vector3> vertices = new();
            List<int> triangles = new();

            var point1 = new Vector3(0.5f, 0.5f, -0.5f + bevel / 2);
            var point2 = new Vector3(0.5f, 0.5f, -0.5f);
            var point3 = new Vector3(0.5f - bevel / 2, 0.5f, -0.5f);

            var rotatingVectorPointBase = new Vector3(0.5f - bevel / 2, 0.5f, -0.5f + bevel / 2);
            point1 = rotation * (point1 - pivot) + pivot;
            point2 = rotation * (point2 - pivot) + pivot;
            point3 = rotation * (point3 - pivot) + pivot;
            rotatingVectorPointBase = rotation * (rotatingVectorPointBase - pivot) + pivot;
            var line = GetLine(point3, point1, smoothness + 2, rotatingVectorPointBase, bevel / 2);

            for (int i = 0; i < smoothness + 1; i++)
            {
                AddTriangle(point2, line[i], line[i + 1], vertices, triangles);
            }

            return new MeshData(vertices, triangles);
        }
        static List<Vector3> GetLine(Vector3 first, Vector3 second, int numberOfPoints, Vector3 centre, float radius)
        {
            var line = new List<Vector3>();
            var direction1 = (first - centre).normalized;
            var direction2 = (second - centre).normalized;
            for (int i = 0; i < numberOfPoints; i++)
            {
                float t = (float)i / (numberOfPoints - 1);
                line.Add(centre + radius * Vector3.Slerp(direction1, direction2, t));
            }

            return line;
        }
        static void AddTriangle(Vector3 first, Vector3 second, Vector3 third, List<Vector3> vertices, List<int> triangles)
        {
            vertices.Add(first);
            vertices.Add(second);
            vertices.Add(third);
            triangles.Add(vertices.Count - 3);
            triangles.Add(vertices.Count - 3 + 1);
            triangles.Add(vertices.Count - 3 + 2);
        }
        static bool CheckConditionsForPatch(PatchType patchType, VoxelNeighbours voxelNeighbours, Vector3Int[] neighboursToCheck)
        {
            return patchType switch
            {
                PatchType.Type1Case1 => voxelNeighbours.GetNeighbourStatus(neighboursToCheck[2]) &&
                        voxelNeighbours.GetNeighbourStatus(neighboursToCheck[1]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[3]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[0]),
                PatchType.Type1Case2 => voxelNeighbours.GetNeighbourStatus(neighboursToCheck[2]) &&
                        voxelNeighbours.GetNeighbourStatus(neighboursToCheck[0]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[1]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[5]),
                PatchType.Type1Case3 => voxelNeighbours.GetNeighbourStatus(neighboursToCheck[1]) &&
                        voxelNeighbours.GetNeighbourStatus(neighboursToCheck[0]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[2]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[4]),
                PatchType.Type2 => voxelNeighbours.GetNeighbourStatus(neighboursToCheck[2]) &&
                        voxelNeighbours.GetNeighbourStatus(neighboursToCheck[1]) &&
                        voxelNeighbours.GetNeighbourStatus(neighboursToCheck[0]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[3]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[4]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[5]),
                PatchType.Type3Case1 => voxelNeighbours.GetNeighbourStatus(neighboursToCheck[2]) &&
                        voxelNeighbours.GetNeighbourStatus(neighboursToCheck[1]) &&
                        voxelNeighbours.GetNeighbourStatus(neighboursToCheck[0]) &&
                        voxelNeighbours.GetNeighbourStatus(neighboursToCheck[3]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[4]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[5]),
                PatchType.Type3Case2 => voxelNeighbours.GetNeighbourStatus(neighboursToCheck[2]) &&
                        voxelNeighbours.GetNeighbourStatus(neighboursToCheck[1]) &&
                        voxelNeighbours.GetNeighbourStatus(neighboursToCheck[0]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[3]) &&
                        voxelNeighbours.GetNeighbourStatus(neighboursToCheck[4]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[5]),
                PatchType.Type3Case3 => voxelNeighbours.GetNeighbourStatus(neighboursToCheck[2]) &&
                        voxelNeighbours.GetNeighbourStatus(neighboursToCheck[1]) &&
                        voxelNeighbours.GetNeighbourStatus(neighboursToCheck[0]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[3]) &&
                        !voxelNeighbours.GetNeighbourStatus(neighboursToCheck[4]) &&
                        voxelNeighbours.GetNeighbourStatus(neighboursToCheck[5]),
                _ => false
            };
        }
        enum PatchType
        {
            Type1Case1,
            Type1Case2,
            Type1Case3,
            Type2,
            Type3Case1,
            Type3Case2,
            Type3Case3
        }
    }
}
