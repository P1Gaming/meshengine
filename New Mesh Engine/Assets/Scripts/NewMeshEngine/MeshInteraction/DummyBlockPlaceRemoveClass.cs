using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBlockPlaceRemoveClass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(placeRandomBlocks());
    }

    private IEnumerator placeRandomBlocks()
    {
        while (true)
        {
            yield return new WaitForSeconds(.25f);
            int x = Random.Range(1, 40);
            int y = Random.Range(1, 40);
            int z = Random.Range(1, 40);
            MeshEngineHandler.TryAddBlock(new BlockTypeWithPosition(BlockType.Grass, new Vector3Int(x, y, z)));
            MeshEngineHandler.IsBlockAtPosition(new Vector3Int(x, y, z));
            MeshEngineHandler.TryRemoveBlock(new Vector3Int(x, y, z));


        }
    }
}
