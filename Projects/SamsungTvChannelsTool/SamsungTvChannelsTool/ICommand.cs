namespace SamsungTvChannelsTool
{
    public interface ICommand
    {
        string CommandName { get; }
        string CommandUsage { get; }
        string CommandDescription { get; }

        int Execute(string[] args);
    }
}