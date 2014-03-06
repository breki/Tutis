using System;
using System.Text;

namespace RankWatch.Razor
{
    public abstract class ReportRazorTemplateBase<TModel>
    {
        protected ReportRazorTemplateBase ()
        {
            OutputBuilder = new StringBuilder ();
        }

        public TModel Model { get; set; }

        public dynamic ViewBag { get; set; }

        public StringBuilder OutputBuilder { get; private set; }

        public abstract void Execute ();

        public virtual void Write (object value)
        {
            OutputBuilder.Append (value);
        }

        public virtual void WriteLiteral (object value)
        {
            OutputBuilder.Append (value);
        }

        public virtual void WriteAttribute (
            string attr,
            Tuple<string, int> token1,
            Tuple<string, int> token2,
            Tuple<Tuple<string, int>, Tuple<object, int>, bool> token3)
        {
            object value;
            if (token3 != null)
                value = token3.Item2.Item1;
            else
                value = string.Empty;

            var output = token1.Item1 + value + token2.Item1;

            Write (output);
        }

        public virtual void WriteAttribute (
            string attr,
            //params object[] parms)
            Tuple<string, int> token1,
            Tuple<string, int> token2,
            Tuple<Tuple<string, int>, Tuple<object, int>, bool> token3,
            Tuple<Tuple<string, int>, Tuple<string, int>, bool> token4)
        {
            //            WriteAttribute("href", 
            //                Tuple.Create(" href=\"", 395), 
            //                Tuple.Create("\"", 452), 
            //                Tuple.Create(Tuple.Create("", 402), Tuple.Create<System.Object, System.Int32>("Value", 402), false),
            //                Tuple.Create(Tuple.Create("", 439), Tuple.Create("?action=login", 439), true)            
            object value;
            object textval;
            if (token3 != null)
                value = token3.Item2.Item1;
            else
                value = string.Empty;

            if (token4 != null)
                textval = token4.Item2.Item1;
            else
                textval = string.Empty;

            string output = token1.Item1 + value + textval + token2.Item1;

            Write (output);
        }
    }
}