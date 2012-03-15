namespace CleanCode.Step7
{
    public class ArsoHtmlWeatherDataReader : IWeatherDataReader
    {
        public void SetText(string text)
        {
            htmlData = text;
            MoveToTableStart();
        }

        public void MoveToNextRow()
        {
            MoveToNext(TableRowMarker);
        }

        public string ExtractNextFieldId()
        {
            MoveToNext(ClassMarker);
            int classEnd = FindNext(EndTagMarker);
            string className = htmlData.Substring(
                htmlCursor + classMarkerLen, classEnd - htmlCursor - classMarkerLen);
            htmlCursor = classEnd;

            return className;
        }

        public string ExtractNextFieldValue()
        {
            int valueEnd = FindNext(StartTagMarker);
            string classValue = htmlData.Substring(
                htmlCursor + endTagMarkerLen,
                valueEnd - htmlCursor - endTagMarkerLen);
            htmlCursor = valueEnd;
            return classValue;
        }

        private void MoveToTableStart()
        {
            MoveToNext(TableMarker);
            MoveToNext(TBodyMarker);

            classMarkerLen = ClassMarker.Length;
            endTagMarkerLen = EndTagMarker.Length;
        }

        private int FindNext(string text)
        {
            return htmlData.IndexOf(text, htmlCursor);
        }

        private void MoveToNext(string text)
        {
            htmlCursor = htmlData.IndexOf(text, htmlCursor);
        }

        private string htmlData;
        private int classMarkerLen;
        private int endTagMarkerLen;
        private int htmlCursor;
        private const string TableMarker = "<table class=\"meteoSI-table\"";
        private const string TBodyMarker = "<tbody";
        private const string TableRowMarker = "<tr>";
        private const string ClassMarker = "<td class=\"";
        private const string EndTagMarker = "\">";
        private const string StartTagMarker = "<";
    }
}