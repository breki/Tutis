namespace CleanCode.Step9
{
    public interface IWeatherDataReader
    {
        void SetText(string text);
        bool MoveToNextRow();
        string ExtractNextFieldId();
        string ExtractNextFieldValue();
    }
}