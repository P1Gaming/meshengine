using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RoundedVoxels
{
    [CustomEditor(typeof(RoundedVoxelVisualizer))]
    public class RoundedVoxelVisualizerEditor : Editor
    {
        static bool editMode;
        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();
            RoundedVoxelVisualizer.weldedEdges = GUILayout.Toggle(RoundedVoxelVisualizer.weldedEdges, "Welded Edged");
            editMode = GUILayout.Toggle(editMode, "Edit Mode");

            if (GUILayout.Button("Generate Cubes"))
            {
                RoundedVoxelVisualizer generator = (RoundedVoxelVisualizer)target;
                generator.GenerateCubes();
            }
        }

        private void OnSceneGUI()
        {
            if (!editMode)
            {
                return;
            }

            RoundedVoxelVisualizer generator = (RoundedVoxelVisualizer)target;
            for (int i = 0; i < generator.activeCubes.GetLength(0); i++)
            {
                for (int j = 0; j < generator.activeCubes.GetLength(1); j++)
                {
                    for (int k = 0; k < generator.activeCubes.GetLength(2); k++)
                    {
                        if (i == 1 && j == 1 && k == 1)
                        {
                            continue;
                        }

                        Handles.color = generator.activeCubes[i, j, k] ? new Color(0, 1, 0, 0.2f) : new Color(1, 0, 0, 0.2f);
                        if (Handles.Button(-1 * Vector3.one + new Vector3(i, j, k), Quaternion.identity, 1, 1, Handles.CubeHandleCap))
                        {
                            generator.activeCubes[i, j, k] = !generator.activeCubes[i, j, k];
                        }
                    }
                }
            }
        }
    }
}
