using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MeshEngine.SaveSystem;

//For non-monobehvaiour classes that need to be instantiated and their references are required by other classes.
internal static class ResourceReferenceKeeper
{
    static Dictionary<Type, object> resourceReferences = new();
    static string twoDFileName = "test2D";
    public static string SAVEPATH = "C:/temp/meshengine";
    static ResourceReferenceKeeper()
    {
        //This is where we instantiate the classes and store them in the disctionary.

        //Example:
        /*resourceReferences[typeof(IMeshGenerator)] = new MeshGenerator();
        resourceReferences[typeof(ISaveDate)] = new ReadData();
        resourceReferences[typeof(IReadData)] = new SaveData();*/
        resourceReferences[typeof(IMeshGenerator)] = UnityEngine.Object.FindObjectOfType<MeshGenerator>();
        resourceReferences[typeof(IChunkLoader)] = UnityEngine.Object.FindObjectOfType<ChunkLoader>();
        //Add MeshEngineHandler to resources
        resourceReferences[typeof(IMeshEngine)] = new MeshEngineHandler();

        SaveSystem save = new SaveSystem(SAVEPATH, twoDFileName);

        resourceReferences[typeof(IReadData)] = save;
        resourceReferences[typeof(ISaveData)] = save;

        //resourceReferences[typeof(IReadData)] = new ChunkDataReaderDummy();
        resourceReferences[typeof(IRequestHandler)] = new RequestHandler();

    }
    public static T GetResource<T>() where T : class
    {
        if(!resourceReferences.TryGetValue(typeof(T), out object resource))
        {
            Debug.LogError("Couldn't find the requested resource.");
            return null;
        }

        return resource as T;
    }
}
