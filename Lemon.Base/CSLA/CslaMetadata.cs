using ProtoBuf;

namespace Lemon.Base
{
    [ProtoContract]
    public class CslaMetadata
    {
        [ProtoMember(1)]
        public bool IsDirty { get; set; }
        [ProtoMember(2)]
        public bool IsNew { get; set; }
    }
}