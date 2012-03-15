namespace CleanCode.Step7
{
    public interface IWeatherDataReader
    {
        void SetText(string text);
        void MoveToNextRow();
        string ExtractNextFieldId();
        string ExtractNextFieldValue();
    }
}