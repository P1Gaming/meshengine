﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class CommandManager : MonoBehaviour
{
    private FloodingStack<ICommand> undoStack = new FloodingStack<ICommand>(10);
    private Stack<ICommand> redoStack = new Stack<ICommand>();

    public void ExecuteCommand(ICommand command)
    {
        if (command.TryExecute())
        {
            undoStack.Push(command);
            redoStack.Clear();
            Debug.Log(undoStack.Count);
        }
    }

    public void UndoLastChange()
    {
        if (undoStack.Count > 0)
        {
            ICommand command = undoStack.Pop();

            command.Undo();
            redoStack.Push(command);
        }
    }
    public void RedoLastChange()
    {
        if (redoStack.Count > 0)
        {
            ICommand command = redoStack.Pop();
            command.TryExecute();
            undoStack.Push(command);
        }
    }

    
}


