using System;
using System.Collections.Generic;

namespace SpatialitePlaying.NodeIndexBuilding1.Features
{
    public class AreaFeatures
    {
        public AreaFeatures AddCategory(int category, string tagName, string tagValue)
        {
            categories.Add(new Tuple<short, string, string>((short)category, tagName, tagValue));
            return this;
        }

        public IList<Tuple<short, string, string>> Categories
        {
            get { return categories; }
        }

        private List<Tuple<short, string, string>> categories = new List<Tuple<short, string, string>>();
    }
}