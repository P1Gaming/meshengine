using MeshEngine.SaveSystem;
using System;
using UnityEngine;

public class MeshEngineManager : MonoBehaviour
{
    private void OnDestroy()
    {
        var sd = ResourceReferenceKeeper.GetResource<ISaveData>();
        if (sd is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
