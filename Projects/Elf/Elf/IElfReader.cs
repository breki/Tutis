namespace Elf
{
    public interface IElfReader
    {
        LogContents ReadLogFile(string fileName);
    }
}