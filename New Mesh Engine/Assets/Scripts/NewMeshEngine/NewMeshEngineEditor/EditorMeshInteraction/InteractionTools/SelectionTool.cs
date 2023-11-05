using System;

public abstract class SelectionTool
{
    public event Action<ICommand> SelectionToolEnded;
    
    protected void InvokeFinnished(ICommand command)
    {
        SelectionToolEnded?.Invoke(command);
    }

    public virtual void OnCancel()
    {
        
    }
    public abstract void Tick(IInput input);
    
}
