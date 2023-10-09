using System;

public interface ISelectionTool
{
    event EventHandler SelectionToolEnded;

    void OnEnter();
    void OnExit();
    void Tick();
}