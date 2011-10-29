using System;
using System.Globalization;
using System.IO;
using System.Text;
using PyDocs.Descriptions;

namespace PyDocs.Parsing
{
    public class DescParser : IDescParser
    {
        public PackageDesc Parse(string descDir)
        {
            PackageDesc packageDesc = ParsePackageDesc(descDir);
            return packageDesc;
        }

        private PackageDesc ParsePackageDesc(string descDir)
        {
            ParsedBlock containerBlock = blockParser.ParseFileIntoBlocks(Path.Combine(descDir, "__package.txt"));

            Assert(containerBlock.Children.Count != 0, "Invalid package description file, there are no blocks inside.");
            Assert(containerBlock.Children.Count == 1, "Only one top block is allowed in the package description file.");

            ParsedBlock packageBlock = containerBlock.Children[0];
            Assert(packageBlock.BlockType == "package", "Invalid package description file, wrong block type.");

            return ParseModules(packageBlock);
        }

        private PackageDesc ParseModules(ParsedBlock packageBlock)
        {
            PackageDesc packageDesc = new PackageDesc(packageBlock.BlockName);
            packageDesc.Description = packageBlock.BlockName;

            foreach (ParsedBlock childBlock in packageBlock.Children)
            {
                Assert(childBlock.BlockType == "module", "Invalid package description, child is of wrong block type ({0}).", packageBlock.BlockType);
                ModuleDesc moduleDesc = ParseModule(childBlock);
                packageDesc.Modules.Add(moduleDesc);
            }

            return packageDesc;
        }

        private ModuleDesc ParseModule(ParsedBlock moduleBlock)
        {
            ModuleDesc moduleDesc = new ModuleDesc(moduleBlock.BlockName);
            moduleDesc.Description = moduleBlock.BlockName;
            return moduleDesc;
        }

        private static void Assert(bool condition, string messageFormat, params object[] args)
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

        private IBlockParser blockParser = new BlockParser();
    }
}