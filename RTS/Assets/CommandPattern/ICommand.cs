namespace KK.CommandPattern
{
    /// <summary>
    /// Command interface. Each command must implement it
    /// </summary>
    public interface ICommand
    {
        void Execute();
    }
}