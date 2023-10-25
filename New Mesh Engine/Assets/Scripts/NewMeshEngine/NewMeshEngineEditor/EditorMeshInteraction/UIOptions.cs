using System;
using UnityEngine;
using static SelectionTool;

public class UIOptions : MonoBehaviour
{
    [SerializeField] BlockType debugBlocktype;
    [SerializeField] WorldPositionSelection worldPositionSelection;
    [SerializeField] Transform indicator;

    SelectionTool selectionTool;
    IInput input = new MouseAndKeyboardInput();


    public void SelectAddBlock()
    {
        AddBlockTool addBlockTool = new AddBlockTool(worldPositionSelection, GetBlockType, indicator);
        ChangeTool(addBlockTool);
    }

    BlockType GetBlockType()
    {
        return debugBlocktype;
    }

    void ChangeTool(SelectionTool newTool)
    {
        if (selectionTool != null)
        {
            selectionTool.SelectionToolEnded -= OnToolUsed;
        }

        selectionTool = newTool;
        if (newTool != null)
        {
            selectionTool.SelectionToolEnded += OnToolUsed;
        }
    }

    private void OnToolUsed(ICommand command)
    {
        if (command != null)
        {
            FindObjectOfType<CommandManager>().ExecuteCommand(command);
        }
        else
        {
            ChangeTool(null);
        }
    }


    private void Update()
    {
        if (selectionTool == null) return;

        selectionTool.Tick(input);
        if (input.Cancel())
        {
            ChangeTool(null);
        }
    }
}