using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RoundedVoxels
{
    internal class VoxelEdgeProvider
    {
        public VoxelSettings VoxelSettings { get; private set; }
        Dictionary<EdgeConfig, MeshData> meshDataDictionary;

        static Dictionary<Edge, Quaternion> inverseRotationsDictionary = new();
        static Dictionary<Edge, Vector3Int[]> neighboursToCheck = new()
        {
            { Edge.TopFront, new Vector3Int[4]
                {
                    new Vector3Int(0, 1, 0), new Vector3Int(0, 0, -1),
                    new Vector3Int(-1, 0, 0), new Vector3Int(1, 0, 0)
                }
            }
        };
        static Dictionary<Edge, List<Rotation>> rotationsToApplyDictionary = new()
        {
            { Edge.TopLeft, new List<Rotation>() { Rotation.NegativeY } },
            { Edge.TopRight, new List<Rotation>() { Rotation.PositiveY } },
            { Edge.TopFront, new List<Rotation>() {} },
            { Edge.TopBack, new List<Rotation>() { Rotation.NegativeY, Rotation.NegativeY } },
            { Edge.BottomLeft, new List<Rotation>() { Rotation.NegativeY, Rotation.NegativeZ } },
            { Edge.BottomRight, new List<Rotation>() { Rotation.PositiveY, Rotation.PositiveZ } },
            { Edge.BottomFront, new List<Rotation>() { Rotation.PositiveX } },
            { Edge.BottomBack, new List<Rotation>() { Rotation.PositiveX, Rotation.PositiveX} },
            { Edge.FrontLeft, new List<Rotation>() { Rotation.NegativeZ } },
            { Edge.FrontRight, new List<Rotation>() { Rotation.PositiveZ } },
            { Edge.BackLeft, new List<Rotation>() { Rotation.NegativeY, Rotation.NegativeX } },
            { Edge.BackRight, new List<Rotation>() { Rotation.PositiveY, Rotation.NegativeX } }
        };

        public VoxelEdgeProvider(VoxelSettings voxelSettings)
        {
            VoxelSettings = voxelSettings;
            PreCalculateMeshes();
        }
        static VoxelEdgeProvider()
        {
            PreCalculateRotationsAndNeighboursToCheck();
        }

        public void AddEdgeToMeshData(MeshData meshData, Edge edge, VoxelNeighbours voxelNeighbours, Vector3 voxelCentre)
        {
            var neighboursToCheckEntry = neighboursToCheck[edge];
            if (voxelNeighbours.GetNeighbourStatus(neighboursToCheckEntry[0]) || 
                voxelNeighbours.GetNeighbourStatus(neighboursToCheckEntry[1]))
            {
                return;
            }

            var edgeMeshData = meshDataDictionary[GenerateEdgeConfig(voxelNeighbours, edge)];
            var rotation = inverseRotationsDictionary[edge];
            int previousCount = meshData.vertices.Count;
            for (int i = 0; i < edgeMeshData.vertices.Count; i++)
            {
                meshData.vertices.Add(rotation * edgeMeshData.vertices[i] + voxelCentre);
            }

            for (int i = 0; i < edgeMeshData.triangles.Count; i++)
            {
                meshData.triangles.Add(previousCount + edgeMeshData.triangles[i]);
            }
        }
        void PreCalculateMeshes()
        {
            meshDataDictionary = new();
            for (int i = 0; i < 4; i++)
            {
                var config = (EdgeConfig)i;
                meshDataDictionary.Add(config, CalculateMeshData(VoxelSettings.Bevel, VoxelSettings.Smoothness, config));
            }
        }
        static MeshData CalculateMeshData(float bevel, int smoothness, EdgeConfig edgeConfig)
        {
            List<Vector3> vertices = new();
            List<int> triangles = new();
            var shortWidth = bevel / Mathf.Sqrt(2);
            float lengthTowardsLeft = 0.5f;
            float lengthTowardsRight = 0.5f;

            if (!edgeConfig.HasFlag(EdgeConfig.ExtendedLeft))
            {
                lengthTowardsLeft -= bevel / 2;
            }
            if (!edgeConfig.HasFlag(EdgeConfig.ExtendedRight))
            {
                lengthTowardsRight -= bevel / 2;
            }

            var offset = ((1 - bevel + bevel / 2) / Mathf.Sqrt(2)) * new Vector3(0, 1, -1).normalized;

            var point1 = Quaternion.Euler(-45, 0, 0) * new Vector3(-lengthTowardsLeft, 0, shortWidth / 2) + offset;
            var point2 = Quaternion.Euler(-45, 0, 0) * new Vector3(lengthTowardsRight, 0, shortWidth / 2) + offset;
            var point3 = Quaternion.Euler(-45, 0, 0) * new Vector3(lengthTowardsRight, 0, -shortWidth / 2) + offset;
            var point4 = Quaternion.Euler(-45, 0, 0) * new Vector3(-lengthTowardsLeft, 0, -shortWidth / 2) + offset;

            var normal = Vector3.Cross((point2 - point1), (point3 - point1)).normalized;
            var rotatingVector1StartPoint = Vector3.Lerp(point1, point4, 0.5f) - (shortWidth / 2) * normal;
            var rotatingVector2StartPoint = Vector3.Lerp(point2, point3, 0.5f) - (shortWidth / 2) * normal;

            Vector3 previousPoint1 = point1;
            Vector3 previousPoint2 = point2;

            Vector3 rotatingVector1Start = (point1 - rotatingVector1StartPoint).normalized;
            Vector3 rotatingVector1End = (point4 - rotatingVector1StartPoint).normalized;

            Vector3 rotatingVector2Start = (point2 - rotatingVector2StartPoint).normalized;
            Vector3 rotatingVector2End = (point3 - rotatingVector2StartPoint).normalized;

            var fullAngle = Vector3.Angle(rotatingVector1Start, rotatingVector1End) * Mathf.Deg2Rad;

            float delta = fullAngle / (smoothness + 1);
            float distance = (point1 - rotatingVector1StartPoint).magnitude;

            for (int i = 0; i < smoothness; i++)
            {
                float t = (delta * (i + 1)) / fullAngle;
                var currentPoint3 = rotatingVector2StartPoint + distance * Vector3.Slerp(rotatingVector2Start, rotatingVector2End, t);
                var currentPoint4 = rotatingVector1StartPoint + distance * Vector3.Slerp(rotatingVector1Start, rotatingVector1End, t);
                AddQuad(previousPoint1, previousPoint2, currentPoint3, currentPoint4, vertices, triangles);

                previousPoint1 = currentPoint4;
                previousPoint2 = currentPoint3;
            }

            AddQuad(previousPoint1, previousPoint2, point3, point4, vertices, triangles);
            return new MeshData(vertices, triangles);
        }

        static void AddQuad(Vector3 first, Vector3 second, Vector3 third, Vector3 fourth, List<Vector3> vertices, List<int> triangles)
        {
            vertices.Add(first);
            vertices.Add(second);
            vertices.Add(third);
            vertices.Add(fourth);
            triangles.Add(vertices.Count - 4);
            triangles.Add(vertices.Count - 4 + 1);
            triangles.Add(vertices.Count - 4 + 2);
            triangles.Add(vertices.Count - 4);
            triangles.Add(vertices.Count - 4 + 2);
            triangles.Add(vertices.Count - 4 + 3);
        }
        static void PreCalculateRotationsAndNeighboursToCheck()
        {
            var topFrontEdgeNeighboursToCheck = neighboursToCheck[Edge.TopFront];
            for (int i = 0; i < 12; i++)
            {
                var edge = (Edge)i;
                var rotationsToApply = rotationsToApplyDictionary[edge];
                inverseRotationsDictionary.Add(edge, VoxelNeighboursOrientationSolver.GetInverseRotation(rotationsToApply));
                if (edge == Edge.TopFront)
                {
                    continue;
                }
                var neighboursToCheckEntry = new Vector3Int[4];
                for (int j = 0; j < 4; j++)
                {
                    neighboursToCheckEntry[j] = VoxelNeighboursOrientationSolver.RotateIndex(topFrontEdgeNeighboursToCheck[j], rotationsToApply);
                }
                neighboursToCheck.Add(edge, neighboursToCheckEntry);
            }
        }
        static EdgeConfig GenerateEdgeConfig(VoxelNeighbours voxelNeighbours, Edge edge)
        {
            var neighboursToCheckEntry = neighboursToCheck[edge];
            var edgeConfig = new EdgeConfig();
            if (voxelNeighbours.GetNeighbourStatus(neighboursToCheckEntry[2]))
            {
                edgeConfig |= EdgeConfig.ExtendedLeft;
            }
            if (voxelNeighbours.GetNeighbourStatus(neighboursToCheckEntry[3]))
            {
                edgeConfig |= EdgeConfig.ExtendedRight;
            }

            return edgeConfig;
        }
        enum EdgeConfig
        {
            ExtendedLeft = 1 << 0,
            ExtendedRight = 1 << 1
        }
    }
}
