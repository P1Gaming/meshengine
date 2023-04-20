using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMeshEngineRequestHandler
{
    public void ChangeBlockAtChunkPosition(Vector3Int pos, BlockType block);

    public bool IsBlockAtChunkPosition(Vector3Int pos);

    public BlockType WhatIsAtPosition(Vector3Int pos);

    public void BuildMesh();

    public void BuildMeshChunk();
}
