using System;

public abstract class SelectionTool
{
    public event EventHandler SelectionToolEnded;
    public class SelectionToolsEventArgs : EventArgs
    { public SelectionToolsEventArgs(SelectionResult results)
        {
            Result = results;
        } 
        public enum SelectionResult { Completed, Canceled }
        public SelectionResult Result { get; private set; }
    }
    protected void InvokeFinnished(SelectionToolsEventArgs args)
    {
        SelectionToolEnded?.Invoke(this, args);
    }
    public abstract void Tick(IInput input);
    
    public abstract ICommand GetResults();
}
