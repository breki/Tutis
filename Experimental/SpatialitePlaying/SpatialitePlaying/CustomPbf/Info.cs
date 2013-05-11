using System.Diagnostics.CodeAnalysis;
using ProtoBuf;

namespace SpatialitePlaying.CustomPbf
{
    [ProtoContract]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class Info
    {
        [ProtoMember (1, Name = "version", IsRequired = false)]
        public int? Version
        {
            get { return version; }
            set { version = value; }
        }

        [ProtoMember (2, Name = "timestamp", IsRequired = false)]
        public long? Timestamp { get; set; }

        [ProtoMember (3, Name = "changeset", IsRequired = false)]
        public long? ChangeSet { get; set; }

        [ProtoMember (4, Name = "uid", IsRequired = false)]
        public int? UserId { get; set; }

        [ProtoMember (5, Name = "user_sid", IsRequired = false)]
        public int? UserSid { get; set; }

        private int? version = -1;
    }
}