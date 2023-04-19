using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IMeshEngineTwo
{
    BlockType WhichBlock(Ray ray, Vector3 collisionPoint);
    bool TryAddBlock(Ray ray, BlockType blockType);
    bool TryAddBlock(Vector3 position, BlockType blockType);
    bool TryRemoveBlock(Ray ray);
    bool TryRemoveBlock(Vector3 position);
    bool TryGetBlockCentre(Ray ray, Vector3 collisionPoint, out Vector3 blockCentre);
}
