using UnityEngine;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    [SerializeField] BlockType debugBlocktype;
    [SerializeField] WorldPositionSelection worldPositionSelection;
    [SerializeField] Slider slider;

    SelectionTool selectionTool;
    private IInput input;

    private void Awake()
    {
        input = new MouseAndKeyboardInput(worldPositionSelection);
    }

    #region ToolSelections
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

    public void SelectSpereBlock()
    {
        SphereBlock sphereBlock = new SphereBlock(GetBlockType, GetSize);
        ChangeTool(sphereBlock);
    }
    #endregion
    
    BlockType GetBlockType()
    {
        return debugBlocktype;
    }
    float GetSize()
    {
        return slider.value;
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
        
        input.GetPointerGridPositionPosition(true);
        
        selectionTool.Tick(input);
        if (input.Cancel())
        {
            ChangeTool(null);
        }
    }
}