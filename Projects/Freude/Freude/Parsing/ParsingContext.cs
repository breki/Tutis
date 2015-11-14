using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Freude.Parsing
{
    public class ParsingContext
    {
        public ParsingContext (IEnumerable<string> textLines)
        {
            Contract.Requires (textLines != null);
            Contract.Requires (Contract.ForAll (textLines, x => x != null));

            this.lines = textLines.ToArray ();
            line = 1;

            //Contract.Assume (Contract.ForAll (lines, x => x != null));
        }

        public IList<Tuple<string, int, int?>> Errors
        {
            get
            {
                Contract.Ensures (Contract.Result<IList<Tuple<string, int, int?>>> () != null);
                return errors;
            }
        }

        public int Line
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 1);
                return line;
            }
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

                return lines[line - 1];
            }
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
            Contract.Invariant(Contract.ForAll(lines, x => x != null));
            Contract.Invariant(EndOfText || CurrentLine.Length >= 0);
        }

        private readonly string[] lines;
        private int line;
        private readonly List<Tuple<string, int, int?>> errors = new List<Tuple<string, int, int?>>();
    }
}