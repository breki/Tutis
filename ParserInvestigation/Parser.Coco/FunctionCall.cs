using System.Collections.Generic;

namespace Parser.Coco
{
    public class FunctionCall : ILabelPart
    {
        public string FunctionName
        {
            get { return functionName; }
            set { functionName = value; }
        }

        public List<FunctionArgument> Arguments
        {
            get { return arguments; }
        }

        public void AddArgument (FunctionArgument argument)
        {
            arguments.Add(argument);
        }

        private string functionName;
        private List<FunctionArgument> arguments = new List<FunctionArgument>();
    }
}