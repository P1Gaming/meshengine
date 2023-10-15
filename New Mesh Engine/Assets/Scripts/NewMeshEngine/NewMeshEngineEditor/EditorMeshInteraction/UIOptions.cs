using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SelectionTool;

public class UIOptions : MonoBehaviour
{
    [SerializeField] BlockType debugBlocktype;
    [SerializeField] WorldPositionSelection worldPositionSelection;
    [SerializeField] Transform indicator;
    
    SelectionTool selectionTool;
    private bool isSelectionActive = false;
    CommandManager commandManager;
    IInput input = new MouseAndKeyboardInput();


    public void SelectAddBlock()
    {
        AddBlockTool addBlockTool = new AddBlockTool(worldPositionSelection, GetBlockType,indicator);
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

    private void OnToolUsed(object sender, EventArgs e)
    {
        if (e is SelectionToolsEventArgs args)
        {
            if (args.Result == SelectionToolsEventArgs.SelectionResult.Completed)
            {
                var command = selectionTool.GetResults();
                FindObjectOfType<CommandManager>().ExecuteCommand(command);
            }
            else
            {
                ChangeTool(null);
            }
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

    private void Awake()
    {
        commandManager = FindObjectOfType<CommandManager>();
    }
}
public interface IInput
{
    bool PointerClick();
    bool Cancel();
    float LowerHigherInput();
}
public class MouseAndKeyboardInput : IInput
{
    public bool Cancel()
    {
        return Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape);
    }

    public bool PointerClick()
    {
        return Input.GetMouseButtonDown(0);
    }

    public float LowerHigherInput()
    {
        // return scrollwheel input
        return Input.mouseScrollDelta.y;
    }
}
