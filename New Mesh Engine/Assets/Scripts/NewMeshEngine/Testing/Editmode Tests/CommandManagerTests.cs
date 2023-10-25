using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CommandManagerTests
{

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [Test]
    public void ExecuteCommandAddsToUndoStack()
    {
        GameObject commandManagerGO = new GameObject();
        CommandManager commandManager = commandManagerGO.AddComponent<CommandManager>();

        ICommand command = new NoCommand();
        ICommand command2 = new NoCommand();
        commandManager.ExecuteCommand(command);
        commandManager.ExecuteCommand(command2);
        Assert.AreEqual(2, commandManager.UndoStackCount);
        commandManager.UndoLastChange();

        Assert.AreEqual(1, commandManager.UndoStackCount);
    }
    [Test]
    public void UndoCommandRemovesLastAddedCommand()
    {
        GameObject commandManagerGO = new GameObject();
        CommandManager commandManager = commandManagerGO.AddComponent<CommandManager>();

        ICommand command = new NoCommand();
        ICommand command2 = new NoCommand();

        commandManager.ExecuteCommand(command);
        commandManager.ExecuteCommand(command2);

        commandManager.UndoLastChange();
        ICommand nextUndoCommand = commandManager.GetCurrentUndoCommand();

        Assert.AreSame(command, nextUndoCommand);
    }

    [Test]
    public void UndoCommandAddsLastUndoToRedoStack()
    {
        GameObject commandManagerGO = new GameObject();
        CommandManager commandManager = commandManagerGO.AddComponent<CommandManager>();

        ICommand command = new NoCommand();
        ICommand command2 = new NoCommand();
        ICommand command3 = new NoCommand();

        commandManager.ExecuteCommand(command);
        commandManager.ExecuteCommand(command2);
        commandManager.ExecuteCommand(command3);


        commandManager.UndoLastChange();
        commandManager.UndoLastChange();

        ICommand nextRedoCommand = commandManager.GetCurrentRedoCommand();

        Assert.AreSame(command2, nextRedoCommand);
    }
}
