public interface ICommand
{
    public bool TryExecute();
    public void Undo();
    public string  GetCommandName();
}