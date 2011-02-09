using System;
using System.Collections.Generic;

namespace OMetaSharp.Examples.Prolog
{
    public class Rule : IPrologItem
    {
        public Rule(IPrologItem head, PrologItems clauses)
        {
            this.Head = head;
            this.Clauses = clauses;
        }

        public IPrologItem Head { get; private set; }
        public PrologItems Clauses { get; private set; }

        public IPrologItem Rename(string n)
        {
            return new Rule(this.Head.Rename(n), this.Clauses.Rename(n) as PrologItems);
        }

        public IPrologItem Rewrite(IDictionary<string, IPrologItem> env)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Rule[Head=" + Head.ToString() + ", Clauses=" + Clauses.ToString() + "]";
        }

    }
}
