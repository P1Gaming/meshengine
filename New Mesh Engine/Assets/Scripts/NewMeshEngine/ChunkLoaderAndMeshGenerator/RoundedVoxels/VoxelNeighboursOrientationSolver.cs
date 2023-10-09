using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoundedVoxels
{
    internal class VoxelNeighboursOrientationSolver
    {
        public VoxelNeighbours Original { get; private set; }
        public VoxelNeighbours Relative { get; private set; }
        public Quaternion CummulativeRotation { get; private set; }
        public Quaternion CummulativeInverseRotation { get; private set; }

        static Dictionary<Rotation, Quaternion> rotationLookup = new()
        {
            { Rotation.PositiveX, Quaternion.AngleAxis(90, Vector3.right) },
            { Rotation.NegativeX, Quaternion.AngleAxis(-90, Vector3.right) },
            { Rotation.PositiveY, Quaternion.AngleAxis(90, Vector3.up) },
            { Rotation.NegativeY, Quaternion.AngleAxis(-90, Vector3.up) },
            { Rotation.PositiveZ, Quaternion.AngleAxis(90, Vector3.forward) },
            { Rotation.NegativeZ, Quaternion.AngleAxis(-90, Vector3.forward) },
        };

        public VoxelNeighboursOrientationSolver(VoxelNeighbours neighbours)
        {
            Original = neighbours;
            Relative = neighbours;
            CummulativeRotation = Quaternion.identity;
            CummulativeInverseRotation = Quaternion.identity;
        }

        public void Rotate(Rotation rotation)
        {
            Rotate(rotationLookup[rotation]);
        } 

        public void Rotate(IEnumerable<Rotation> rotations)
        {
            var quaternion = Quaternion.identity;
            foreach(var rotation in rotations)
            {
                quaternion *= rotationLookup[rotation];
            }

            Rotate(quaternion);
        }

        void Rotate(Quaternion quaternion)
        {
            bool[,,] afterRotation = new bool[3, 3, 3];
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    for (int k = -1; k < 2; k++)
                    {
                        Vector3 newNeghbourCentre = quaternion * new Vector3(i, j, k);
                        Vector3Int newIndex = new Vector3Int(Mathf.RoundToInt(newNeghbourCentre.x), Mathf.RoundToInt(newNeghbourCentre.y), Mathf.RoundToInt(newNeghbourCentre.z));
                        afterRotation[newIndex.x + 1, newIndex.y + 1, newIndex.z + 1] = Relative.GetNeighbourStatus(i, j, k);
                    }
                }
            }

            Relative = new VoxelNeighbours(afterRotation, Vector3Int.one);
            CummulativeRotation *= quaternion;
            CummulativeInverseRotation *= Quaternion.Inverse(quaternion);
        }

        public static Vector3Int RotateIndex(Vector3Int index, IEnumerable<Rotation> rotations)
        {
            var quaternion = GetInverseRotation(rotations);

            var rotatedVector = quaternion * index;
            return new Vector3Int(Mathf.RoundToInt(rotatedVector.x), Mathf.RoundToInt(rotatedVector.y), Mathf.RoundToInt(rotatedVector.z));
        }

        public static Quaternion GetRotation(IEnumerable<Rotation> rotations)
        {
            var quaternion = Quaternion.identity;
            foreach (var rotation in rotations)
            {
                quaternion *= rotationLookup[rotation];
            }

            return quaternion;
        }

        public static Quaternion GetInverseRotation(IEnumerable<Rotation> rotations)
        {
            return Quaternion.Inverse(GetRotation(rotations));
        }
    }
    public enum Rotation
    {
        PositiveX,
        NegativeX,
        PositiveY,
        NegativeY,
        PositiveZ,
        NegativeZ
    }
}
