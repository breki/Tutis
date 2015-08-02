using Brejc.Common.Console;

namespace Freude.Commands
{
    public class BuildCommand : StandardConsoleCommandBase
    {
        public BuildCommand ()
        {
            AddArg ("site source dir", "path to the site source directory").Value ((x, env) => siteSourceDirectory = x);
            AddArg ("template file", "path to the template file").Value ((x, env) => templateFileName = x);
            AddArg ("build dir", "path to the destination directory where the site will be built").Value ((x, env) => buildDirectory = x);
        }

        public override string CommandId
        {
            get { return "build"; }
        }

        public override object Description
        {
            get { return "build the site"; }
        }

        public override int Execute (IConsoleEnvironment env)
        {
            return 0;
        }

        private string templateFileName;
        private string siteSourceDirectory;
        private string buildDirectory;
    }
}