namespace CleanCode.Step9
{
    public interface ITableDataReader
    {
        void SetText(string text);
        bool MoveToNextRow();
        string ExtractNextFieldId();
        string ExtractNextFieldValue();
    }
}