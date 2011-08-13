using System.Collections.Generic;
using System.Linq;

namespace OMetaSharp.Examples.Prolog
{
    public class Clause : IPrologItem, ISupportUnify
    {
        private Sym m_Sym;
        private PrologItems m_Args;

        public Clause(Sym sym, PrologItems args)
        {
            m_Sym = sym;
            m_Args = args;
        }

        public IPrologItem Rename(string name)
        {
            return new Clause(m_Sym, m_Args.Rename(name) as PrologItems);
        }

        public IPrologItem Rewrite(IDictionary<string, IPrologItem> env)
        {
            return new Clause(m_Sym, m_Args.Rewrite(env) as PrologItems);
        }

        public void Unify(IPrologItem that, IDictionary<string, IPrologItem> env)
        {
            var asClause = that as Clause;
            if (asClause != null)
            {
                // Ensure that the -arity is the same.
                PrologLibrary.Assert(asClause.m_Args.Count == m_Args.Count);

                // Symbols need to match
                m_Sym.Unify(asClause.m_Sym, env);

                // Must be able to unify all the arguments
                for (int i = 0; i < m_Args.Count; i++)
                {
                    var currentClauseArgument = m_Args[i] as ISupportUnify;
                    var otherClauseArgument = asClause.m_Args[i];
                    currentClauseArgument.Unify(otherClauseArgument, env);
                }
            }
            else
            {
                (that as ISupportUnify).Unify(this, env);
            }
        }

        public override string ToString()
        {
            return m_Sym.ToString() + "(" + string.Join(", ", m_Args.Select(x => x.ToString()).ToArray()) + ")";
        }
    }
}
