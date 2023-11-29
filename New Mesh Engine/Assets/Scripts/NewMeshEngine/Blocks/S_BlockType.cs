using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Basic Block", menuName = "ScriptableObjects/BlockScriptableObject", order = 1)]
public class S_BlockType : ScriptableObject
{
    [SerializeField]
    public Material Material { get; private set; }

    [SerializeField]
    public string Name { get; private set; }

    [SerializeField]
    public int Id { get; private set; } = 0;

    [SerializeField]
    public float BreakTime { get; private set; }

    [SerializeField]
    public GameObject DropItem { get; private set; }

    public static int TotalDifferentBlocks { get { return Blocks.Count; } }

    private static List<S_BlockType> Blocks = new List<S_BlockType>();

    private void OnEnable()
    {
        Blocks.Add(this);
    }

    public static S_BlockType GetBlockWitId(int id)
    {
        // That is air
        if (id == 0)
        {
            return null;
        }

        CleanList();

        for (int i = 0; i < Blocks.Count; i++)
        {
            if (Blocks[i] == null)
            {
                Blocks.RemoveAt(i);
                if (i == Blocks.Count)
                {
                    break;
                }
            }
            if(id == Blocks[i].Id)
            {
                return Blocks[i];
            }
        }

        Debug.Log("No Block With That Id Exists!");
        return null;
    }

    private static void CleanList()
    {
        for (int i = 0; i < Blocks.Count; i++)
        {
            if (i >= Blocks.Count)
            {
                break;
            }
            if (Blocks[i] == null)
            {
                Blocks.RemoveAt(i);
                i--;
            }
        }
    }
}


