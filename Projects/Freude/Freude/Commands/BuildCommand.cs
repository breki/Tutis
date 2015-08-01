using Brejc.Common.Console;

namespace Freude.Commands
{
    public class BuildCommand : StandardConsoleCommandBase
    {
        public BuildCommand ()
        {
            AddArg ("template ID", "map template to use").Value ((x, env) => templateId = x);
            AddArg ("theme ID", "map theme to use. If not specified, all themes will be used").Value ((x, env) => themeId = x).IsOptional ();
            AddSwitch ("downloadOsm", "forces re-downloading of OSM data", (x, env) => forceOsmRedownload = x).Alias ("dlosm");
            AddSwitch ("recreateDB", "forces recreation of geo DB", (x, env) => forceRecreateDb = x).Alias ("rcrdb");
            AddSwitch ("debug", "switches on the debug rendering mode", (x, env) => debugMode = x);
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

        private string templateId;
        private string themeId;
        private bool forceOsmRedownload;
        private bool forceRecreateDb;
        private bool debugMode;
    }
}