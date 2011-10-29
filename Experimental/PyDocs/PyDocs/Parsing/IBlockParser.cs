using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace PyDocs.Parsing
{
    public interface IBlockParser
    {
        ParsedBlock ParseFileIntoBlocks(string fileName);
    }

    public class BlockParser : IBlockParser
    {
        public ParsedBlock ParseFileIntoBlocks(string fileName)
        {
            string[] lines = FetchFile(fileName);

            ParsedBlock containerBlock = new ParsedBlock("container");
            ParseBlock(containerBlock, lines, 0);
            return containerBlock;
        }

        private static int ParseBlock(ParsedBlock containerBlock, string[] lines, int lineCounter)
        {
            StringBuilder blockContent = new StringBuilder();

            for (; lineCounter < lines.Length; lineCounter++)
            {
                string line = lines[lineCounter].TrimEnd();

                if (line == "---")
                    break;

                if (line.StartsWith("---"))
                {
                    ParsedBlock block = ParseBlockHeader(line);
                    lineCounter = ParseBlock(block, lines, lineCounter + 1) - 1;
                    containerBlock.Children.Add(block);
                    continue;
                }

                blockContent.AppendLine(line);
            }

            containerBlock.BlockContent = blockContent.ToString();

            return lineCounter;
        }

        private static ParsedBlock ParseBlockHeader(string line)
        {
            int firstSpace = line.IndexOf(' ');
            Assert(firstSpace >= 0, "Invalid line '{0}'", line);

            string blockType = line.Substring(3, firstSpace - 3);

            ParsedBlock block = new ParsedBlock(blockType);
            block.BlockName = line.Substring(firstSpace+1).Trim();
            return block;
        }

        private static string[] FetchFile(string fileName)
        {
            return File.ReadAllLines(fileName);
        }

        private static void Assert (bool condition, string messageFormat, params object[] args)
        {
            if (!condition)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    messageFormat,
                    args);
                throw new InvalidOperationException(message);
            }
        }
    }
}