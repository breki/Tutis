using System.Collections.Generic;
using System.Diagnostics;

namespace OMetaSharp.Examples.Prolog.UnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var query = new Clause(new Sym("grandfather"), 
                                  new PrologItems(new Sym("abe"), new Var("X")));

            var rules = new PrologItems(
                                new Rule(
                                       new Clause(new Sym("grandfather"),
                                                  new PrologItems(new Var("X"), new Var("Y"))),
                                       new PrologItems(
                                           new Clause(new Sym("father"),
                                                new PrologItems(new Var("X"), new Var("Z"))),
                                           new Clause(new Sym("father"),
                                                new PrologItems(new Var("Z"), new Var("Y"))))),
                                new Rule(new Clause(new Sym("father"), new PrologItems(new Sym("abe"), new Sym("homer"))), new PrologItems()),
                                new Rule(new Clause(new Sym("father"), new PrologItems(new Sym("homer"), new Sym("lisa"))), new PrologItems()),
                                new Rule(new Clause(new Sym("father"), new PrologItems(new Sym("homer"), new Sym("bart"))), new PrologItems())
                                );

            var solutions = new List<IPrologItem>(PrologLibrary.GetSolutions(query, rules));
            Debug.Assert(solutions.Count == 2);
            Debug.Assert(solutions[0].ToString() == "grandfather(abe, lisa)");
            Debug.Assert(solutions[1].ToString() == "grandfather(abe, bart)");           
        }
    }
}
