using System;
using UnityEngine;

public class BoxAddBlockTool : SelectionTool
{
    private Func<BlockType> GetBlockTypeAction;
    private bool selectedFirst = false;
    private Vector3 selectedFirstPosition;
    private Vector3 selectedSecondPosition;
    private Bounds fillVolume;
    private Transform cube;

    public BoxAddBlockTool(Func<BlockType> GetBlockType)
    {
        GetBlockTypeAction = GetBlockType;
    }

    public override void Tick(IInput input)
    {
        if (input.PointerClick())
        {
            if (!selectedFirst)
            {
                selectedFirstPosition = input.GetPointerPosition();
                selectedFirst = true;
                fillVolume.min = selectedFirstPosition;
                cube = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                cube.GetComponent<Collider>().enabled = false;
            }
            else
            {
                selectedSecondPosition = input.GetPointerPosition();
                Vector3Int minPos = new Vector3Int(
                    (int) Mathf.Min(selectedFirstPosition.x, selectedSecondPosition.x),
                    (int) Mathf.Min(selectedFirstPosition.y, selectedSecondPosition.y),
                    (int) Mathf.Min(selectedFirstPosition.z, selectedSecondPosition.z));
                Vector3Int maxPos = new Vector3Int(
                    (int) Mathf.Max(selectedFirstPosition.x, selectedSecondPosition.x),
                    (int) Mathf.Max(selectedFirstPosition.y, selectedSecondPosition.y),
                    (int) Mathf.Max(selectedFirstPosition.z, selectedSecondPosition.z));
                var command = new AddBlockBoxCommand(
                    minPos,
                    maxPos,
                    GetBlockTypeAction());
                InvokeFinnished(command);
                OnCancel();
            }
        }

        if (selectedFirst)
        {
            fillVolume.max = input.GetPointerPosition();
            cube.position = fillVolume.center-(Vector3.one/2*WorldInfo.BlockSize);
            cube.localScale = fillVolume.size;
        }
    }

    public override void OnCancel()
    {
        if (cube != null)
        {
            GameObject.Destroy(cube.gameObject);
        }

        selectedFirst = false;
    }
}