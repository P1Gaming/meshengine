using System;
using UnityEngine;
using static SelectionTool;

public class UIOptions : MonoBehaviour
{
    [SerializeField] BlockType debugBlocktype;
    [SerializeField] WorldPositionSelection worldPositionSelection;

    SelectionTool selectionTool;
    private IInput input;

    private void Awake()
    {
        input = new MouseAndKeyboardInput(worldPositionSelection);
    }


    public void SelectAddBlock()
    {
        AddBlockTool addBlockTool = new AddBlockTool(GetBlockType);
        ChangeTool(addBlockTool);
    }

    public void SelectBoxFill()
    {
        BoxAddBlockTool boxTool = new BoxAddBlockTool(GetBlockType);
        ChangeTool(boxTool);
    }

    BlockType GetBlockType()
    {
        return debugBlocktype;
    }

    void ChangeTool(SelectionTool newTool)
    {
        if (selectionTool != null)
        {
            selectionTool.OnCancel();
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
        
        input.GetPointerPosition();
        selectionTool.Tick(input);
        if (input.Cancel())
        {
            ChangeTool(null);
        }
    }
}