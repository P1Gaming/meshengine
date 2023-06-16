using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal interface IMeshGenerator
{
    public void StartGeneratingMesh(ChunkData chunkData);
    public event Action<ChunkData, Mesh> OnMeshGenerated;
    public void ModifyMesh(ChunkData chunkData);
    public event Action<ChunkData, Mesh> OnMeshModified;
}
