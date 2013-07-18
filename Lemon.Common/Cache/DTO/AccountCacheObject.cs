using System;
using System.Data.Linq.Mapping;
using Lemon.Base;
using ProtoBuf;

namespace Lemon.Common
{
    [Table(Name = "Account")]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class AccountCacheObject : IObjectWithId, IObjectWithTimestamp
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int AccountId { get; set; }
        [Column]
        public int CategoryId { get; set; }
        [Column]
        public int AccountTypeId { get; set; }
        [Column]
        public string AccountNumber { get; set; }
        [Column]
        public string AccountDescription { get; set; }
        [Column]
        public float Balance { get; set; }
        [Column]
        public byte[] Timestamp { get; set; }
        [Column]
        public DateTime LastModDatetime { get; set; }

        int IObjectWithId.Id
        {
            get { return AccountId; }
        }

        public override string ToString()
        {
            return TupleFormatter.Format(CategoryId, AccountNumber);
        }
    }
}
