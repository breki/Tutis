namespace OMetaSharp.Examples.Prolog
{
    public class State
    {
        public State(IPrologItem query, PrologItems goals)
        {
            this.Query = query;
            this.Goals = goals;
        }

        public IPrologItem Query { get; private set; }
        public PrologItems Goals { get; private set; }
    }
}
