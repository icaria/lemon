using ProtoBuf;

namespace Winterspring.Lemon.DataPortal
{
    [ProtoContract]
    public class DataPortalMessageHeader
    {
        [ProtoMember(1)]
        public SerializationType SerializationType { get; set; }
        [ProtoMember(2)]
        public string Type { get; set; }
        [ProtoMember(3)]
        public string AuthenticationToken { get; set; }
    }
}