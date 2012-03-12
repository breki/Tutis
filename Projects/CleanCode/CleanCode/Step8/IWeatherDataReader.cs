namespace CleanCode.Step8
{
    public interface IWeatherDataReader
    {
        void SetText(string text);
        bool MoveToNextRow();
        string ExtractNextFieldId();
        string ExtractNextFieldValue();
    }
}