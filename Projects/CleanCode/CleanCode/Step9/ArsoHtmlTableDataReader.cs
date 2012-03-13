using System;

namespace CleanCode.Step9
{
    public class ArsoHtmlTableDataReader : ITableDataReader
    {
        public void SetText(string text)
        {
            htmlData = text;
            MoveToTableStart();
        }

        public bool MoveToNextRow()
        {
            if (!hasMoreRecords)
                return false;

            hasMoreRecords = MoveToNext(TableRowStartMarker, TBodyEndMarker);
            return hasMoreRecords;
        }

        public string ExtractNextFieldId()
        {
            if (!hasMoreRecords)
                return null;

            bool hasMoreFields = MoveToNext(ClassMarker, TableRowEndMarker);
            if (!hasMoreFields)
                return null;

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
            hasMoreRecords = true;
            if (!MoveToNext(TableStartMarker, HtmlEndMarker))
                hasMoreRecords = false;
            else if (!MoveToNext(TBodyStartMarker, TableEndMarker))
                hasMoreRecords = false;

            classMarkerLen = ClassMarker.Length;
            endTagMarkerLen = EndTagMarker.Length;
        }

        private int FindNext(string text)
        {
            AssertNotEof();

            return htmlData.IndexOf(text, htmlCursor);
        }

        private bool MoveToNext(string text, string altenativeText)
        {
            AssertNotEof();

            int textIndex = htmlData.IndexOf(text, htmlCursor);
            int alternativeTextIndex = htmlData.IndexOf(altenativeText, htmlCursor);

            if (textIndex == -1)
            {
                if (alternativeTextIndex == -1)
                    throw new InvalidOperationException("Invalid input data.");

                htmlCursor = alternativeTextIndex;
                return false;
            }

            if (textIndex < alternativeTextIndex)
            {
                htmlCursor = textIndex;
                return true;
            }

            htmlCursor = alternativeTextIndex;
            return false;
        }

        private void AssertNotEof()
        {
            if (!hasMoreRecords)
                throw new InvalidOperationException("End of data reached.");
        }

        private string htmlData;
        private int classMarkerLen;
        private int endTagMarkerLen;
        private int htmlCursor;
        private const string TableStartMarker = "<table class=\"meteoSI-table\"";
        private const string TableEndMarker = "</table";
        private const string TBodyStartMarker = "<tbody";
        private const string TBodyEndMarker = "</tbody";
        private const string TableRowStartMarker = "<tr>";
        private const string TableRowEndMarker = "</tr>";
        private const string ClassMarker = "<td class=\"";
        private const string EndTagMarker = "\">";
        private const string StartTagMarker = "<";
        private const string HtmlEndMarker = "</html>";
        private bool hasMoreRecords;
    }
}