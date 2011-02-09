using System.Collections.Generic;

namespace OMetaSharp.Examples.Prolog
{
    public class Var : IPrologItem, ISupportUnify
    {
        public Var(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        public IPrologItem Rename(string nm)
        {
            return new Var(Name + nm);
        }

        public IPrologItem Rewrite(IDictionary<string, IPrologItem> env)
        {
            IPrologItem item;

            // Does the environment have a definition for this variable?
            if (env.TryGetValue(this.Name, out item))
            {
                // use it
                return item;
            }

            // Keep the Variable-ness
            return this;
        }

        public void Unify(IPrologItem that, IDictionary<string, IPrologItem> env)
        {
            IPrologItem value;

            if (env.TryGetValue(Name, out value))
            {
                ISupportUnify valueUnified = value as ISupportUnify;
                PrologLibrary.Assert(valueUnified != null);

                valueUnified.Unify(that, env);
            }
            else
            {
                PrologLibrary.AddBinding(env, Name, that.Rewrite(env));
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
