using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ProtoBuf;

namespace SpatialitePlaying.CustomPbf
{
    [ProtoContract]
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    internal class DenseInfo
    {
        [ProtoMember (1, Name = "version", Options = MemberSerializationOptions.Packed)]
        public IList<int> Version
        {
            get { return version; }
            set { version = value; }
        }

        [ProtoMember (2, Name = "timestamp", Options = MemberSerializationOptions.Packed, DataFormat = DataFormat.ZigZag)]
        public IList<long> Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        [ProtoMember(3, Name = "changeset", Options = MemberSerializationOptions.Packed, DataFormat = DataFormat.ZigZag)]
        public IList<long> ChangeSet
        {
            get { return changeSet; }
            set { changeSet = value; }
        }

        [ProtoMember(4, Name = "uid", Options = MemberSerializationOptions.Packed, DataFormat = DataFormat.ZigZag)]
        public IList<int> UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        [ProtoMember(5, Name = "user_sid", Options = MemberSerializationOptions.Packed, DataFormat = DataFormat.ZigZag)]
        public IList<int> UserSid
        {
            get { return userSid; }
            set { userSid = value; }
        }

        private IList<int> version = new List<int>();
        private IList<long> timestamp = new List<long>();
        private IList<long> changeSet = new List<long>();
        private IList<int> userId = new List<int>();
        private IList<int> userSid = new List<int>();
    }
}