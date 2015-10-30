using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

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

        public bool EndOfText
        {
            get { return line > lines.Length; }
        }

        public string CurrentLine
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                if (EndOfText)
                    throw new InvalidOperationException();

                Contract.Assume(lines[line - 1] != null);
                return lines[line - 1];
            }
        }

        public void SetTextLines (IEnumerable<string> textLines)
        {
            Contract.Requires(textLines != null);
            Contract.Requires (Contract.ForAll (textLines, x => x != null));

            this.lines = textLines.ToArray();
            line = 1;

            Contract.Assume (Contract.ForAll (lines, x => x != null));
        }

        public void IncrementLineCounter()
        {
            line++;

            if (line > lines.Length + 1)
                throw new InvalidOperationException("Incremented line beyond the text");
        }

        public void ReportError(string errorMessage)
        {
            errors.Add(new Tuple<string, int, int?>(errorMessage, line, null));
        }

        public void ReportError(string errorMessage, int column)
        {
            errors.Add(new Tuple<string, int, int?>(errorMessage, line, column));
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(line >= 1);
            Contract.Invariant(line <= lines.Length + 1);
            Contract.Invariant(EndOfText || CurrentLine.Length >= 0);
            Contract.Invariant(lines == null || Contract.ForAll(lines, x => x != null));
            Contract.Invariant(errors != null);
        }

        private string[] lines;
        private int line = 1;
        private readonly List<Tuple<string, int, int?>> errors = new List<Tuple<string, int, int?>>();
    }
}