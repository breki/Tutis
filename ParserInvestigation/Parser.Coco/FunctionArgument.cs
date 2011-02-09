namespace Parser.Coco
{
    public class FunctionArgument : ILabelPart
    {
        public FunctionArgument(LabelDefinition labelDefinition)
        {
            this.labelDefinition = labelDefinition;
        }

        public LabelDefinition LabelDefinition
        {
            get { return labelDefinition; }
        }

        private readonly LabelDefinition labelDefinition;
    }
}