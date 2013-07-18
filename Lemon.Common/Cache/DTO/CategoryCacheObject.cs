using System.Data.Linq.Mapping;
using Lemon.Base;
using ProtoBuf;

namespace Lemon.Common
{
    [Table(Name = "Category")]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class CategoryCacheObject : IDisplayObject, IObjectWithTimestamp
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int CategoryId { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public byte[] Timestamp { get; set; }

        int IObjectWithId.Id
        {
            get { return CategoryId; }
        }
    }
}