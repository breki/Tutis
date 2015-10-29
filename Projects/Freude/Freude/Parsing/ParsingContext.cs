using System;
using System.Collections.Generic;

namespace Freude.Parsing
{
    public class ParsingContext
    {
        public IList<Tuple<string, int, int?>> Errors
        {
            get { return errors; }
        }

        public int Line
        {
            get { return line; }
        }

        public bool HasAnyErrors
        {
            get { return errors.Count > 0; }
        }

        public void IncrementLineCounter()
        {
            line++;
        }

        public void ReportError(string errorMessage)
        {
            errors.Add(new Tuple<string, int, int?>(errorMessage, line, null));
        }

        public void ReportError(string errorMessage, int column)
        {
            errors.Add(new Tuple<string, int, int?>(errorMessage, line, column));
        }

        private int line = 1;
        private readonly List<Tuple<string, int, int?>> errors = new List<Tuple<string, int, int?>>();
    }
}