using System;
using System.Collections.Generic;

namespace TreasureChest.Diagnostics
{
    public class DependencyDotGraphGenerator
    {
        public string GenerateDotGraph(IChestMaster chest)
        {
            this.chest = chest;
            dot = new DotBuilder();

            Header ();
            Nodes ();
            Dependencies ();
            Footer ();

            return dot.ToString();
        }

        private void Header()
        {
            dot.StartGraph("chest");
        }

        private void Nodes()
        {
            foreach (KeyValuePair<object, IRegistrationHandler> pair 
                in chest.DependencyGraph.EnumerateObjects ())
            {
                object obj = pair.Key;
                Type type = obj.GetType();
                dot.AddNode(obj, type.Name);
            }
        }

        private void Dependencies()
        {
            foreach (KeyValuePair<object, IRegistrationHandler> pair 
                in chest.DependencyGraph.EnumerateObjects ())
            {
                object obj1 = pair.Key;
                foreach (object obj2 in chest.DependencyGraph.GetObjectsThatAreNecessaryFor (obj1))
                    dot.AddDirectedEdge(obj1, obj2);
            }
        }

        private void Footer()
        {
            dot.EndGraph();
        }

        private IChestMaster chest;
        private DotBuilder dot;
    }
}