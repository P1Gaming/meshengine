using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ToolType
{
    NONE,
    TEST1,
    TEST2
}

/// <summary>
/// Used to create new blocks for the mesh engine.
/// </summary>
[CreateAssetMenu(fileName = "BlockData", menuName = "Blocks/Default", order = 1)]
public class Block : ScriptableObject
{
    public static Dictionary<short, Block> s_BlockList;
    private static Texture s_TextureMatrix;
    private static List<short> s_UsedIds;
    private short _previousId = -1;

    [SerializeField]
    private string BlockName = "[PH]";
    [SerializeField]
    private string Description = "Place Holder Description!";
    [SerializeField]
    private short Id = -1;
    [SerializeField]
    private Texture Texture;

    private void OnValidate()
    {
        if (Id == _previousId)
        {
            return;
        }

        PopulateIds();
    }

    private void OnEnable()
    {
        PopulateIds();
    }

    private void Awake()
    {
        PopulateIds();
    }

    private void PopulateIds()
    {
        if (s_BlockList == null)
        {
            s_BlockList = new Dictionary<short, Block>();
        }

        if (s_UsedIds == null)
        {
            s_UsedIds = new List<short>();
        }

        s_BlockList.Remove(_previousId);
        s_UsedIds.Remove(_previousId);
        _previousId = Id;

        if (Id < 0)
        {
            _previousId = Id;
            Debug.LogError("Id Must Be Positive on: " + BlockName);
            Debug.Log(ExistingBlockIds());
            return;
        }

        if (Id == 0)
        {

            Debug.LogError("Id 0 is reserved for air: " + BlockName);
            Debug.Log("New Id Assigned at position " + Id + " on " + BlockName);
            Debug.Log(ExistingBlockIds());
            return;
        }

        if (s_BlockList.ContainsKey(Id))
        {
            Debug.LogError("Id already exists at: " + Id + " on " + BlockName);
            Id = -1;
            _previousId = -1;
            Debug.Log(ExistingBlockIds());
            return;
        }

        s_BlockList.Add(Id, this);
        s_UsedIds.Add(Id);

        SortUsedIds();

        Debug.Log("New Id Assigned at position " + Id + " on " + BlockName);

        _previousId = Id;

        Debug.Log(ExistingBlockIds());
    }

    /// <summary>
    /// Get this block's Id.
    /// </summary>
    /// <returns>Short Id</returns>
    public short GetId()
    {
        return Id;
    }

    /// <summary>
    /// Get this block's name.
    /// </summary>
    /// <returns>String Name</returns>
    public string GetName()
    {
        return BlockName;
    }

    /// <summary>
    /// Get this block's description.
    /// </summary>
    /// <returns>String Description</returns>
    public string GetDescription()
    {
        return Description;
    }

    public Texture GetTexture()
    {
        return Texture;
    }

    /// <summary>
    /// Try to get block with Id. Returns null if none exist with ID.
    /// </summary>
    /// <param name="id">Block Id</param>
    /// <returns>Block with given Id.</returns>
    public static Block TryGetBlockFromId(short id)
    {
        if(s_BlockList.TryGetValue(id, out Block block)){
            return block;
        }

        return null;
    }

    /// <summary>
    /// Get a string to show all blocks with hteir associated ids.
    /// </summary>
    /// <returns>String of the dictionary.</returns>
    public static string ExistingBlockIds()
    {
        string stringBuilder = string.Empty;

        foreach(short position in s_UsedIds)
        {
            stringBuilder += position + " : " + s_BlockList[position].GetName() + "\t";
        }

        return stringBuilder;
    }

    private static void SortUsedIds()
    {
        if (s_UsedIds == null || s_UsedIds.Count <= 1)
            return;

        s_UsedIds = s_UsedIds.OrderBy(value => value).ToList();
    }
}
