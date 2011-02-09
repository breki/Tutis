using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace OMetaSharp
{
    public class CodeGen
    {        
        private StringBuilder m_Buffer = new StringBuilder();
        private static readonly char PushIndentSignal = char.ConvertFromUtf32(2)[0];
        private static readonly char PopIndentSignal = char.ConvertFromUtf32(3)[0];
        private static readonly char PushVariableSignal = char.ConvertFromUtf32(5)[0];
        private static readonly char PopVariableSignal = char.ConvertFromUtf32(6)[0];
        private static readonly char UseVariableSignal = char.ConvertFromUtf32(1)[0];
                
        public CodeGen()        
        {
        }
                
        public CodeGen Write(params object[] objectsToWrite)
        {            
            if (objectsToWrite == null)
            {
                return this;
            }

            foreach (var o in objectsToWrite)
            {
                m_Buffer.Append(o.ToString());
            }

            return this;
        }
                
        public CodeGen WriteLine(params object[] objectsToWrite)
        {
            Write(objectsToWrite);
            
            bool shouldWriteLinefeed = true;

            if (objectsToWrite != null)
            {
                if (objectsToWrite.Length > 0)
                {
                    object lastObject = objectsToWrite[objectsToWrite.Length - 1];

                    if(lastObject != null)
                    {
                        shouldWriteLinefeed = !lastObject.ToString().EndsWith(Environment.NewLine);
                    }                    
                }
            }
            if (shouldWriteLinefeed)
            {
                m_Buffer.AppendLine();
            }
            return this;
        }

        public CodeGen WriteLineEnd(params object[] objectsToWrite)
        {
            Write(objectsToWrite);
            Write(Environment.NewLine);
            return this;
        }

        public CodeGen WriteLines(params object[] linesToWrite)
        {
            foreach (var currentLine in linesToWrite)
            {
                WriteLine(currentLine);
            }

            return this;
        }
                
        public CodeGen WriteLeveledVar(string prefix)
        {
            return Write(LeveledVar(prefix));
        }

        public static string LeveledVar(string prefix)
        {
            return prefix + UseVariableSignal;
        }
                
        public CodeGen PushIndent()
        {
            Write(PushIndentSignal);
            return this;
        }

        public CodeGen PopIndent()
        {
            Write(PopIndentSignal);
            return this;
        }

        public CodeGen PushVariableScope()
        {
            Write(PushVariableSignal);
            return this;
        }

        public CodeGen PopVariableScope()
        {
            Write(PopVariableSignal);
            return this;
        }

        public static CodeGen Create()
        {
            return new CodeGen();
        }

        public CodeGen Do(Action<CodeGen> actionToPerform)
        {
            actionToPerform(this);
            return this;
        }

        public static CodeGen Format(OMetaList<HostExpression> format, params object[] args)
        {            
            return Create().Write(FormatString(format.As<CodeGen>().ToString(), args));            
        }

        private static string FormatString(string format, params object[] args)
        {
            // HACK: Need to fix bugs with using string.Format with special characters.            
            var currentVal = format;

            for (int i = 0; i < args.Length; i++)
            {
                currentVal = currentVal.Replace("{" + i.ToString() + "}", args[i].ToString());
            }
            return currentVal;
        }

        public CodeGen WriteFormattedLine(string format, params object[] args)
        {
            return WriteLine(FormatString(format, args));
        }
                
        public override string ToString()
        {
            return m_Buffer.ToString();
        }
        
        public string ToGeneratedCodeString()
        {
            string indentSpaces = "    ";
            int variableScope = 1;
            int indentLevel = 0;

            // HACK: Won't use Environment.NewLine, since
            //       we really just care about the last char.
            char endOfLine = '\n'; 
            var sb = new StringBuilder();

            bool atStartOfLine = true;

            for(int ixBufferChar = 0; ixBufferChar < m_Buffer.Length; ixBufferChar++)
            {
                char c = m_Buffer[ixBufferChar];

                if (c == PushIndentSignal)
                {
                    indentLevel++;
                }
                else if (c == PopIndentSignal)
                {
                    indentLevel--;
                }
                else if (c == PushVariableSignal)
                {
                    variableScope++;
                }
                else if (c == PopVariableSignal)
                {
                    variableScope--;
                }
                else if (c == UseVariableSignal)
                {
                    if (variableScope > 1)
                    {
                        sb.Append(variableScope.ToString());
                    }
                }
                else if (c == endOfLine)
                {
                    sb.Append(endOfLine);
                    atStartOfLine = true;
                }
                else
                {
                    if (atStartOfLine)
                    {
                        for (int i = 0; i < indentLevel; i++)
                        {
                            sb.Append(indentSpaces);
                        }
                        atStartOfLine = false;
                    }
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
