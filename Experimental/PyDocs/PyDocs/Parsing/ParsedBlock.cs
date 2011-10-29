using System.Collections.Generic;

namespace PyDocs.Parsing
{
    public class ParsedBlock
    {
        public ParsedBlock(string blockType)
        {
            this.blockType = blockType;
        }

        public string BlockType
        {
            get { return blockType; }
        }

        public string BlockName
        {
            get { return blockName; }
            set { blockName = value; }
        }

        public string BlockContent
        {
            get { return blockContent; }
            set { blockContent = value; }
        }

        public IList<ParsedBlock> Children
        {
            get { return children; }
        }

        private string blockType;
        private string blockName;
        private string blockContent;
        private List<ParsedBlock> children = new List<ParsedBlock>();
    }
}