using System.Diagnostics;
namespace OMetaSharp
{
    /// <summary>
    /// A stream that breaks an input string into its character parts.
    /// </summary>
    public class StringStream : OMetaStream<char>
    {
        [DebuggerStepThrough]
        public StringStream(string s)
            : base(new OMetaStringList(s))
        {
        }
    }
}
