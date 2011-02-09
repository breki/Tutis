using System.Text;

namespace OMetaSharp
{
    /// <summary>
    /// An assorted collection of extension methods that might be useful in creating grammars.
    /// </summary>
    public static class ExtensionMethods
    {
        public static int ToDigit(this char digit)
        {
            return digit - '0';
        }

        public static int ToDigit(this OMetaList<HostExpression> list)
        {
            return list.As<string>().ToDigit();
        }

        public static int ToDigit(this object digit)
        {
            return (digit.ToString()[0]).ToDigit();
        }

        public static string ToProgramString(this string rawString)
        {
            return "\"" + rawString.ToCSharpEscapedString() + "\"";
        }

        public static string ToLiteralString(this OMetaList<HostExpression> l)
        {
            if (l == OMetaList<HostExpression>.Nil)
            {
                return "";
            }

            return Sugar.Implode(l).ToString();
        }

        public static OMetaList<HostExpression> ToProgramString(this OMetaList<HostExpression> l)
        {
            if (l == OMetaList<HostExpression>.Nil)
            {
                return "\"\"".AsHostExpressionList();                
            }

            return l.ToString().ToProgramString().AsHostExpressionList();


        }

        public static string ToCSharpEscapedString(this string unescapedString)
        {
            // see http://blogs.msdn.com/csharpfaq/archive/2004/03/12/88415.aspx
            var sb = new StringBuilder();
            for (int ix = 0; ix < unescapedString.Length; ix++)
            {
                char currentChar = unescapedString[ix];
                if (currentChar == '\"')
                {
                    sb.Append(@"\""");
                }
                else if (currentChar == '\\')
                {
                    sb.Append(@"\\");
                }
                else if (currentChar == '\0')
                {
                    sb.Append(@"\0");
                }
                else if (currentChar == '\a')
                {
                    sb.Append(@"\a");
                }
                else if (currentChar == '\b')
                {
                    sb.Append(@"\b");
                }
                else if (currentChar == '\f')
                {
                    sb.Append(@"\f");
                }
                else if (currentChar == '\n')
                {
                    sb.Append(@"\n");
                }
                else if (currentChar == '\r')
                {
                    sb.Append(@"\r");
                }
                else if (currentChar == '\t')
                {
                    sb.Append(@"\t");
                }
                else if (currentChar == '\v')
                {
                    sb.Append(@"\v");
                }
                else
                {
                    sb.Append(currentChar);
                }

                // TODO:

                // \uxxxx - Unicode escape sequence for character with hex value xxxx 
                // \xn[n][n][n] - Unicode escape sequence for character with hex value nnnn (variable length version of \uxxxx) 
                // \Uxxxxxxxx - Unicode escape sequence for character with hex value xxxxxxxx (for generating surrogates)                

            }
            return sb.ToString();
        }
    }
}
