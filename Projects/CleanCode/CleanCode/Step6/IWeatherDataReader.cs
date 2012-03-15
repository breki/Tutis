namespace CleanCode.Step6
{
    public interface IWeatherDataReader
    {
        void SetText(string text);
        void MoveToTableStart();
        void MoveToNextRow();
        string ExtractNextFieldId();
        string ExtractNextFieldValue();
    }
}