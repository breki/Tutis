/*
  Copyright (c) 2008 Jeff Moser <jeff@moserware.com>
  
  Derived from OMeta/JS project file "Prolog_Library.txt" which was
  
  Copyright (c) 2007, 2008 Alessandro Warth <awarth@cs.ucla.edu> and Stephen Murrell <stephen@rabbit.eng.miami.edu>

  Permission is hereby granted, free of charge, to any person
  obtaining a copy of this software and associated documentation
  files (the "Software"), to deal in the Software without
  restriction, including without limitation the rights to use,
  copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the
  Software is furnished to do so, subject to the following
  conditions:

  The above copyright notice and this permission notice shall be
  included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
  OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
  HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
  OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace OMetaSharp.Examples.Prolog
{
    public static class PrologLibrary
    {
        public static void AddBinding(IDictionary<string, IPrologItem> env, string name, IPrologItem value)
        {
            var subst = new Dictionary<string, IPrologItem>();
            subst[name] = value;

            foreach (var n in env.Keys.ToArray())
            {
                env[n] = env[n].Rewrite(subst);
            }

            env[name] = value;
        }

        public static void Assert(bool cond)
        {
            // TODO: Replace with boolean approach
            if (!cond)
            {
                throw new Exception("unification failed");
            }
        }

        public static IEnumerable<IPrologItem> GetSolutions(Clause outerQuery, PrologItems rules)
        {
            int nameMangler = 0;
            var stateStack = new Stack<State>();
            stateStack.Push(new State(outerQuery, new PrologItems(outerQuery)));

            while (true)
            {
                if (stateStack.Count == 0)
                {
                    yield break;
                }

                var state = stateStack.Pop();
                var query = state.Query;
                var goals = state.Goals;

                if (goals.Count == 0)
                {
                    yield return query;
                    nameMangler++;
                    continue;
                }

                var goal = goals.Pop();

                for (int i = rules.Count - 1; i >= 0; i--)
                {
                    var rule = rules[i].Rename(nameMangler.ToString()) as Rule;
                    var env = new Dictionary<string, IPrologItem>();

                    try
                    {
                        (rule.Head as ISupportUnify).Unify(goal, env);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    var newQuery = query.Rewrite(env);
                    var newGoals = goals.Rewrite(env) as PrologItems;
                    var newBody = rule.Clauses.Rewrite(env) as PrologItems;

                    for (int ixBody = newBody.Count - 1; ixBody >= 0; ixBody--)
                    {
                        newGoals.Push(newBody[ixBody]);
                    }

                    stateStack.Push(new State(newQuery, newGoals));
                }               
            }
        }

        public static void Solve(Clause query, PrologItems rules)
        {
            foreach (IPrologItem item in GetSolutions(query, rules))
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("No more solutions.");
        }
    }
}
