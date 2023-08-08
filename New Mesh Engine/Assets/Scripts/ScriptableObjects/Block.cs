using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolType
{
    NONE,
    TEST1,
    TEST2
}

/// <summary>
/// Used to create new blocks for the mesh engine.
/// </summary>
public class Block : ScriptableObject
{
    [SerializeField]
    public string BlockName { get; private set; }

    [SerializeField]
    public string Description { get; private set; }

    [SerializeField]
    public short Id { get; private set; }

    [SerializeField]
    public Texture Texture { get; private set; }

    [SerializeField]
    public float BreakSpeed { get; private set; }

    [SerializeField]
    public ToolType BreakTool { get; private set; }
}
