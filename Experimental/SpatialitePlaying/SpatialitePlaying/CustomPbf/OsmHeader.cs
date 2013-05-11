using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ProtoBuf;

namespace SpatialitePlaying.CustomPbf
{
    [ProtoContract]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class OsmHeader
    {
        [ProtoMember (1, Name = "bbox", IsRequired = false)]
        public HeaderBBox BBox { get; set; }

        [ProtoMember (4, Name = "required_features")]
        public IList<string> RequiredFeatures
        {
            get { return requiredFeatures; }
            set { requiredFeatures = value; }
        }

        [ProtoMember (5, Name = "optional_features")]
        public IList<string> OptionalFeatures
        {
            get { return optionalFeatures; }
            set { optionalFeatures = value; }
        }

        [ProtoMember (16, Name = "writingprogram", IsRequired = false)]
        public string WritingProgram { get; set; }

        [ProtoMember (17, Name = "source", IsRequired = false)]
        public string Source { get; set; }

        private IList<string> optionalFeatures = new List<string>();
        private IList<string> requiredFeatures = new List<string> ();
    }
}