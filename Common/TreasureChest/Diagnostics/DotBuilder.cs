using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TreasureChest.Diagnostics
{
    public class DotBuilder
    {
        public void StartGraph (string graphName)
        {
            s.AppendFormat("graph {0} {{", graphName);
            s.AppendLine();
            nodesToNames.Clear ();
            usedNodeNames.Clear ();
        }

        public void AddNode(object node, string namePrefix)
        {
            string nodeName = namePrefix;

            int i = 1;
            while (usedNodeNames.Contains(nodeName))
                nodeName = namePrefix + i++ .ToString(CultureInfo.InvariantCulture);

            nodesToNames.Add(node, nodeName);
            usedNodeNames.Add(nodeName);

            s.AppendFormat("\t{0} [label=\"{1}\"];", nodeName, namePrefix);
            s.AppendLine();
        }

        public void AddDirectedEdge (object fromNode, object toNode)
        {
            string fromNodeName = nodesToNames[fromNode];
            string toNodeName = nodesToNames[toNode];
            s.AppendFormat("\t{0} -> {1};", fromNodeName, toNodeName);
            s.AppendLine();
        }

        public void EndGraph ()
        {
            s.AppendLine("}");
        }

        public override string ToString ()
        {
            return s.ToString ();
        }

        private StringBuilder s = new StringBuilder();
        private Dictionary<object, string> nodesToNames = new Dictionary<object, string>();
        private HashSet<string> usedNodeNames = new HashSet<string>();
    }
}