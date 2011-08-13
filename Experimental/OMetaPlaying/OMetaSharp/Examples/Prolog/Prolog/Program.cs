using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMetaSharp.OMetaCS;
using OMetaSharp.OMetaCS.UnitTests;
using System.Diagnostics;

namespace OMetaSharp.Examples.Prolog
{
    internal enum PrologInterpreterMode
    {
        Query,
        Fact
    }

    public class Program : OMetaCSConsoleProgram<PrologTranslator>
    {
        public static void Main()
        {
            OMetaConsoleProgram.Run<Program>();
        }
                   
        public override void PerformTests()
        {
            AssertSolve("grandfather(X, Y)",
                        @"father(abe, homer).
                          father(homer, lisa).
                          father(homer, bart).
                          grandfather(X, Y) :- father(X, Z), father(Z, Y).",
                         "grandfather(abe, lisa)",
                         "grandfather(abe, bart)");
        }

        public override void AddSamples()
        {
            AddSample("!",
                      "father(abe, homer).",
                      "father(homer, lisa).",
                      "father(homer, bart).",
                      "grandfather(X, Y) :- father(X, Z), father(Z, Y).",
                      "?",
                      "grandfather(X, Y)",
                      "[clear]");
        }
        public override void StartInteractiveSession()
        {
            DisplayWelcomeMessage();

            var mode = PrologInterpreterMode.Fact;

            var rules = new StringBuilder();

            while (true)
            {
                try
                {
                    DisplayPrompt(mode);
                    var input = Input.Next();

                    if (input.Trim().Length == 0)
                    {
                        continue;
                    }

                    if (WantsToSwitchMode(input, ref mode))
                    {
                        continue;
                    }

                    if (WantsToClearEnvironment(input))
                    {
                        Console.WriteLine("-- Clearing Environment --");
                        rules = new StringBuilder();
                        mode = PrologInterpreterMode.Fact;
                        continue;
                    }

                    ExecutePrologCommand(input, rules, mode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred: " + ex.Message);
                }
            }
        }

        private void ExecutePrologCommand(string input, StringBuilder rules, PrologInterpreterMode mode)
        {
            switch (mode)
            {
                case PrologInterpreterMode.Query:
                    ExecuteQuery(input, rules.ToString());
                    break;
                case PrologInterpreterMode.Fact:
                    AddFact(input, rules);
                    break;
                default:
                    throw new NotImplementedException("Unrecognized mode: " + mode.ToString());                    
            }
        }
        
        private void ExecuteQuery(string query, string rules)
        {
            var solutions = GetSolutions(query, rules);
            if (solutions.Count == 0)
            {
                Console.WriteLine("no");
                return;
            }

            foreach (var currentSolution in solutions)
            {
                Console.WriteLine(currentSolution);
            }

            Console.WriteLine("yes");
        }

        private void AddFact(string fact, StringBuilder allFacts)
        {            
            if (!fact.TrimEnd().EndsWith("."))
            {
                // I seem to forget the "." sometimes, so let's compensate for that
                fact = fact + ".";
            }
            allFacts.Append(fact);
        }

        private bool WantsToSwitchMode(string input, ref PrologInterpreterMode mode)
        {
            if (WantsFactMode(input))
            {
                mode = PrologInterpreterMode.Fact;
                return true;
            }
            else if (WantsQueryMode(input))
            {
                mode = PrologInterpreterMode.Query;
                return true;
            }

            return false;
        }

        private bool WantsQueryMode(string input)
        {
            return MatchesAny(input, "?", "[query]");
        }

        private bool WantsFactMode(string input)
        {
            return MatchesAny(input, "!", "[fact]", "[rule]", "[user]", "[clause]");
        }

        private bool WantsToClearEnvironment(string input)
        {
            return MatchesAny(input, "[clear]", "[new]");
        }

        private bool MatchesAny(string input, params string[] items)
        {
            input = input.Trim();

            foreach (string currentItem in items)
            {
                if (currentItem.Equals(input, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void DisplayPrompt(PrologInterpreterMode mode)
        {
            string prompt = (mode == PrologInterpreterMode.Query)
                            ? "?-"
                            : "->";
            Console.Write(prompt + " ");
        }

        private static IList<IPrologItem> GetSolutions(string query, string rules)
        {
            var actualQuery = Grammars.ParseWith<PrologTranslator>(query, t => t.Query).As<Prolog.Clause>();
            var actualRules = new Prolog.PrologItems(Grammars.ParseWith<PrologTranslator>(rules,t => t.Rules).ToIEnumerable<IPrologItem>());

            var result = new List<IPrologItem>(PrologLibrary.GetSolutions(actualQuery, actualRules));

            return result;

        }

        public static void AssertSolve(string query, string rules, params string[] solutions)
        {
            var actualSolutions = GetSolutions(query, rules);

            Debug.Assert(solutions.Length == actualSolutions.Count);
            for (int i = 0; i < solutions.Length; i++)
            {
                Debug.Assert(solutions[i].Equals(actualSolutions[i].ToString(), StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
