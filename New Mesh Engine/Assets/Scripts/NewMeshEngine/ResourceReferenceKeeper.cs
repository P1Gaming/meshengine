using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//For non-monobehvaiour classes that need to be instantiated and their references are required by other classes.
internal static class ResourceReferenceKeeper
{
    static Dictionary<Type, object> resourceReferences = new();
    static ResourceReferenceKeeper()
    {
        //This is where we instantiate the classes and store them in the disctionary.

        //Example:
        /*resourceReferences[typeof(IMeshGenerator)] = new MeshGenerator();
        resourceReferences[typeof(ISaveDate)] = new ReadData();
        resourceReferences[typeof(IReadData)] = new SaveData();*/
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
