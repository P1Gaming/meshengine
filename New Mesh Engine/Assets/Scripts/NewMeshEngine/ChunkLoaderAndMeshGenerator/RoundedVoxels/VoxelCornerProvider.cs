using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace RoundedVoxels
{
    internal class VoxelCornerProvider
    {
        public VoxelSettings VoxelSettings { get; private set; }
        MeshData cornerMeshData;

        static Dictionary<Corner, Quaternion> inverseRotationsDictionary = new();
        static Dictionary<Corner, Vector3Int[]> neighboursToCheck = new()
        {
            { Corner.FrontTopRight, new Vector3Int[6]
                {
                    new Vector3Int(0, 1, 0), new Vector3Int(1, 0, 0), new Vector3Int(0, 0, -1),
                    new Vector3Int(1, 0, -1), new Vector3Int(1, 1, 0), new Vector3Int(0, 1, -1)
                }
            }
        };
        static Dictionary<Corner, List<Rotation>> rotationsToApplyDictionary = new()
        {
            { Corner.FrontTopLeft, new List<Rotation>(){ Rotation.NegativeY } },
            { Corner.FrontTopRight, new List<Rotation>(){}},
            { Corner.FrontBottomRight, new List<Rotation>(){ Rotation.PositiveZ }},
            { Corner.FrontBottomLeft, new List<Rotation>(){ Rotation.PositiveZ, Rotation.PositiveZ }},
            { Corner.BackTopLeft, new List<Rotation>(){ Rotation.NegativeY, Rotation.NegativeY }},
            { Corner.BackTopRight, new List<Rotation>(){ Rotation.PositiveY }},
            { Corner.BackBottomRight, new List<Rotation>(){ Rotation.PositiveY, Rotation.NegativeX }},
            { Corner.BackBottomLeft, new List<Rotation>(){ Rotation.NegativeY, Rotation.NegativeX, Rotation.NegativeX }}
        };

        public VoxelCornerProvider(VoxelSettings voxelSettings)
        {
            VoxelSettings = voxelSettings;
            PreCalculateMeshes();
        }
        static VoxelCornerProvider()
        {
            PreCalculateRotationsAndNeighboursToCheck();
        }

        public void AddCornerToMeshData(MeshData meshData, Corner corner, VoxelNeighbours voxelNeighbours, Vector3 voxelCentre)
        {
            if (voxelNeighbours.GetNeighbourStatus(neighboursToCheck[corner][0]) ||
                voxelNeighbours.GetNeighbourStatus(neighboursToCheck[corner][1]) ||
                voxelNeighbours.GetNeighbourStatus(neighboursToCheck[corner][2]))
            {
                return;
            }

            var rotation = inverseRotationsDictionary[corner];
            int previousCount = meshData.vertices.Count;
            for (int i = 0; i < cornerMeshData.vertices.Count; i++)
            {
                meshData.vertices.Add(rotation * cornerMeshData.vertices[i] + voxelCentre);
            }

            for (int i = 0; i < cornerMeshData.triangles.Count; i++)
            {
                meshData.triangles.Add(previousCount + cornerMeshData.triangles[i]);
            }
        }
        void PreCalculateMeshes()
        {
            cornerMeshData = CalculateMeshData(VoxelSettings.Bevel, VoxelSettings.Smoothness);
        }

        public static Dictionary<Corner, Vector3Int[]> GetNeighboursToCheckCopy()
        {
            return new Dictionary<Corner, Vector3Int[]>(neighboursToCheck);
        }
        public static Dictionary<Corner, Quaternion> GetInverseRotationsDictionaryCopy()
        {
            return new Dictionary<Corner, Quaternion>(inverseRotationsDictionary);
        }
        static MeshData CalculateMeshData(float bevel, int smoothness)
        {
            List<Vector3> vertices = new();
            List<int> triangles = new();
            float sideLength = bevel / Mathf.Sqrt(2);
            float heightOfTriangle = (sideLength * Mathf.Sqrt(3)) / 2;
            var offset = ((Mathf.Sqrt(3) / 2) * (1 - bevel) + (bevel / 2) / Mathf.Sqrt(3)) * new Vector3(1, 1, -1).normalized;

            Vector3 first = Quaternion.Euler(-54.7356f, -45, 0) * new Vector3(-sideLength / 2, 0, -heightOfTriangle * (1f / 3)) + offset;
            Vector3 second = Quaternion.Euler(-54.7356f, -45, 0) * new Vector3(0, 0, heightOfTriangle * (2f / 3)) + offset;
            Vector3 third = Quaternion.Euler(-54.7356f, -45, 0) * new Vector3(sideLength / 2, 0, -heightOfTriangle * (1f / 3)) + offset;

            List<List<Vector3>> lines = new();

            var centroid = new Vector3(first.x + second.x + third.x, first.y + second.y + third.y, first.z + second.z + third.z) / 3;
            var normal = Vector3.Cross(second - first, third - first).normalized;
            var centre = centroid - (sideLength / Mathf.Sqrt(6)) * normal;
            var distance = (first - centre).magnitude;

            var startDirection1 = (first - centre).normalized;
            var startDirection2 = (third - centre).normalized;
            var endDirection = (second - centre).normalized;

            for (int i = 0; i < smoothness + 1; i++)
            {
                float t = (float)i / (smoothness + 1);
                var point1 = centre + distance * Vector3.Slerp(startDirection1, endDirection, t);
                var point2 = centre + distance * Vector3.Slerp(startDirection2, endDirection, t);

                var line = GetLine(point1, point2, smoothness + 2 - i, centre, distance);
                lines.Add(line);
            }
            lines.Add(new List<Vector3>() { second });

            for (int i = 0; i < lines.Count - 1; i++)
            {
                var lowerLine = lines[i];
                var upperLine = lines[i + 1];
                for (int j = 0; j < upperLine.Count; j++)
                {
                    AddTriangle(upperLine[j], lowerLine[j + 1], lowerLine[j], vertices, triangles);
                }
                for (int j = 0; j < upperLine.Count - 1; j++)
                {
                    AddTriangle(upperLine[j], upperLine[j + 1], lowerLine[j + 1], vertices, triangles);
                }
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
        static void PreCalculateRotationsAndNeighboursToCheck()
        {
            var frontTopRightCornerNeighboursToCheck = neighboursToCheck[Corner.FrontTopRight];
            for (int i = 0; i < 8; i++)
            {
                var corner = (Corner)i;
                var rotationsToApply = rotationsToApplyDictionary[corner];
                inverseRotationsDictionary.Add(corner, VoxelNeighboursOrientationSolver.GetInverseRotation(rotationsToApply));
                if (corner == Corner.FrontTopRight)
                {
                    continue;
                }
                var neighboursToCheckEntry = new Vector3Int[6];
                for (int j = 0; j < 6; j++)
                {
                    neighboursToCheckEntry[j] = VoxelNeighboursOrientationSolver.RotateIndex(frontTopRightCornerNeighboursToCheck[j], rotationsToApply);
                }
                neighboursToCheck.Add(corner, neighboursToCheckEntry);
            }
        }
    }
}
