using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(PerformanceTester))]
public class PerformanceTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Test"))
        {
            PerformanceTester tester = (PerformanceTester)target;
            tester.Test();
        }
    }
}
