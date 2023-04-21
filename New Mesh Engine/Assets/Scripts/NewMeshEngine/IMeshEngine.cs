using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IMeshEngine
{
    BlockType WhichBlock(Ray ray, Vector3 collisionPoint);
    bool TryAddBlock(Ray ray, BlockType blockType);
    bool TryAddBlock(Vector3 position, BlockType blockType);
    bool TryRemoveBlock(Ray ray);
    bool TryRemoveBlock(Vector3 position);
    bool TryGetBlockCentre(Ray ray, Vector3 collisionPoint, out Vector3 blockCentre);

    bool IsBlockThere(Vector3 position);

    Vector3 GetBlockCentre(Vector3 position);

    UnityAction<Bounds> OnChunkLoaded();

    UnityAction<Bounds> OnChunkUnloaded();

    UnityAction OnWorldLoaded();

    void AddBlocks(List<BlockTypeWithPosition> blocksToAdd);

    void RemoveBlocks(List<Vector3Int> positionOfBlocksToRemove);
    //List<BlockTypeWithPosition> RemoveBlocks(List<Vector3Int> positionOfBlocksToRemove); // Potentially Will need to be implemented to get block drops 

}
