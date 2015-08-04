using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Web;
using Brejc.Common;
using log4net;
using Syborg.Razor;

namespace Freude.Templating
{
    public abstract class FreudeRazorTemplateBase
    {
        public dynamic ViewBag
        {
            get
            {
                if (innerTemplate != null)
                    return innerTemplate.viewBag;
                return viewBag;
            }

            set
            {
                if (innerTemplate != null)
                    innerTemplate.viewBag = value;
                else
                    viewBag = value;
            }
        }

        public IDictionary<string, Action> Sections
        {
            get
            {
                if (innerTemplate != null)
                    return innerTemplate.sections;

                return sections;
            }
        }

        public StringBuilder OutputBuilder
        {
            get
            {
                if (outerTemplate != null)
                    return outerTemplate.outputBuilder;

                return outputBuilder;
            }
        }

        public RazorTemplateBase InnerTemplate
        {
            get { return innerTemplate; }
            set { innerTemplate = value; }
        }

        public string InnerTemplateBody
        {
            get { return innerTemplateBody; }
            set { innerTemplateBody = value; }
        }

        public RazorTemplateBase OuterTemplate
        {
            get { return outerTemplate; }
            set { outerTemplate = value; }
        }

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
            params object[] args)
        {
            Contract.Requires (args != null);

            if (args.Length < 3)
                throw new InvalidOperationException ("parms.Length < 3");

            string prefix = ((Tuple<string, int>)args[0]).Item1;
            string suffix = ((Tuple<string, int>)args[1]).Item1;

            //log.DebugFormat("prefix='{0}' suffix='{1}'", prefix, suffix);

            Write (prefix);
            for (int i = 2; i < args.Length; i++)
            {
                string valuePrefix;
                string value;

                object arg = args[i];

                if (arg is Tuple<Tuple<string, int>, Tuple<string, int>, bool>)
                {
                    Tuple<Tuple<string, int>, Tuple<string, int>, bool> token = (Tuple<Tuple<string, int>, Tuple<string, int>, bool>)arg;

                    valuePrefix = token.Item1.Item1;
                    value = valuePrefix + token.Item2.Item1;

                    //log.DebugFormat ("Write1 ('{0}'), token.Item1.Item1='{1}'", value, token.Item1.Item1);
                }
                else if (arg is Tuple<Tuple<string, int>, Tuple<object, int>, bool>)
                {
                    Tuple<Tuple<string, int>, Tuple<object, int>, bool> token = (Tuple<Tuple<string, int>, Tuple<object, int>, bool>)arg;

                    valuePrefix = token.Item1.Item1;
                    value = valuePrefix + token.Item2.Item1;

                    //log.DebugFormat ("Write ('{0}'), token.Item1.Item1='{1}'", value, token.Item1.Item1);
                }
                else
                    throw new InvalidOperationException ("Unsupported token type: {0}".Fmt (arg.GetType ()));

                Write (value);
            }

            Write (suffix);
        }

        public void WriteSection (string name, Action contents)
        {
            throw new NotImplementedException ("WriteSection");
            //if (name == null || contents == null)
            //    return;

            //Sections[name] = contents;
        }

        public virtual void DefineSection (string sectionName, Action action)
        {
            Contract.Requires (sectionName != null);
            //if (log.IsDebugEnabled)
            //    log.DebugFormat("DefineSection (sectionName='{0}', action={1})", sectionName, action.Method);

            Sections.Add (sectionName, action);
        }

        public HtmlString RenderBody ()
        {
            return new HtmlString (innerTemplateBody);
        }

        public HtmlString RenderSection (string sectionName)
        {
            return RenderSection (sectionName, false);
        }

        public HtmlString RenderSection (string sectionName, bool required)
        {
            //if (log.IsDebugEnabled)
            //    log.DebugFormat("RenderSection (name={0}, required={1})", sectionName, required);

            if (sectionName == null)
                throw new ArgumentNullException ("sectionName");

            Action renderSection;
            if (!Sections.TryGetValue (sectionName, out renderSection))
            {
                if (innerTemplate == null)
                    return null;

                if (required)
                    throw new ApplicationException ("Section not defined: " + sectionName);

                //log.DebugFormat ("Section '{0}' not defined", sectionName);

                return null;
            }

            renderSection ();

            return null;
        }

        private dynamic viewBag = new ExpandoObject ();
        private readonly Dictionary<string, Action> sections = new Dictionary<string, Action> ();
        private FreudeRazorTemplateBase innerTemplate;
        private FreudeRazorTemplateBase outerTemplate;
        private string innerTemplateBody;
        private readonly StringBuilder outputBuilder = new StringBuilder ();
        private static readonly ILog log = LogManager.GetLogger (MethodBase.GetCurrentMethod ().DeclaringType);
    }
}