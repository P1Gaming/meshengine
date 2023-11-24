using System;
using UnityEngine;
using TMPro;

public class UndoDisplay : MonoBehaviour
{
    private TMP_Text text;
    private CommandManager cm;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        cm = FindObjectOfType<CommandManager>();
        cm.OnUndoStackChanged += OnOnUndoStackChanged;
    }

    private void OnOnUndoStackChanged(string newCommand)
    {
        text.SetText("Undo: "+newCommand);
    }

    private void OnDestroy()
    {
        cm.OnUndoStackChanged -= OnOnUndoStackChanged;
    }
}
