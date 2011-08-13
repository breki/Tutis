using System;
using System.Collections.Generic;

namespace TreasureChest.Tests.SampleModule
{
    public class ComponentWithProperties : IServiceX
    {
        public IServiceY ServiceY { get; set; }
        public IServiceY ServiceY2
        {
            get { return null; }
        }

        public string Whatever { get; set; }
        public IDictionary<string, IServiceY> Whatever2 { get; set; }
        public string this[string propertyId]
        {
            get { return null; }
            set { }
        }

        public bool BooleanWithError
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}