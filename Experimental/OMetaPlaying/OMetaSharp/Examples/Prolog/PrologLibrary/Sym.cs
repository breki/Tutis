using System.Collections.Generic;

namespace OMetaSharp.Examples.Prolog
{
    public class Sym : IPrologItem, ISupportUnify
    {
        public Sym(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        public IPrologItem Rename(string nm)
        {
            return this;
        }

        public IPrologItem Rewrite(IDictionary<string, IPrologItem> env)
        {
            return this;
        }

        public void Unify(IPrologItem that, IDictionary<string, IPrologItem> env)
        {
            var asSym = that as Sym;

            if (asSym != null)
            {
                // If we're unifying with another symbol, the symbol names must match.
                PrologLibrary.Assert(this.Name == asSym.Name);
            }
            else
            {
                // The only other thing to unify with is a variable
                var asVar = that as Var;
                PrologLibrary.Assert(asVar != null);

                IPrologItem value;
                if (env.TryGetValue(asVar.Name, out value))
                {
                    this.Unify(value, env);
                }
                else
                {
                    PrologLibrary.AddBinding(env, asVar.Name, this.Rewrite(env));
                }
            }

        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
