using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class ChunkObjectPool
{
    Queue<MeshFilter> meshFilters = new();
    bool hasInstantiatedPool;

    public void InstantiatePool(int count, Material chunkMaterial)
    {
        if(hasInstantiatedPool)
        {
            Debug.LogError("Pool has been already instantiated");
            return;
        }

        hasInstantiatedPool = true;
        var parent = new GameObject("Chunk Object Pool");
        for(int i=0;i<count;i++)
        {
            var chunkObject = new GameObject();
            chunkObject.SetActive(false);
            chunkObject.transform.parent = parent.transform;
            chunkObject.AddComponent<MeshRenderer>().sharedMaterial = chunkMaterial;
            chunkObject.AddComponent<MeshCollider>().convex = true;
            var meshFilter = chunkObject.AddComponent<MeshFilter>();
            meshFilters.Enqueue(meshFilter);
        }
    }

    public MeshFilter GetInstance()
    {
        if(!hasInstantiatedPool)
        {
            Debug.LogError("Couldn't get instance. Pool has not been instantiated yet.");
            return null;
        }

        if(meshFilters.Count > 0)
        {
            return meshFilters.Dequeue();
        }

        Debug.LogError("Pool is empty");
        return null;
    }

    public void ReturnInstance(MeshFilter meshFilter)
    {
        if (!hasInstantiatedPool)
        {
            Debug.LogError("Couldn't return instance. Pool has not been instantiated yet.");
            return;
        }

        meshFilter.gameObject.SetActive(false);
        meshFilters.Enqueue(meshFilter);
    }
}