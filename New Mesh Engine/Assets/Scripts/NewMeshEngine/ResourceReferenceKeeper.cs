using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

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
    public static T GetResource<T>() where T : class, IReadData, ISaveDate, IMeshGenerator
    {
        if(!resourceReferences.TryGetValue(typeof(T), out object resource))
        {
            return null;
        }

        return resource as T;
    }
}
