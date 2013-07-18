using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lemon.Base;
using Lemon.Common;
using Lemon.Model;
using ProtoBuf;
using Winterspring.DataPortal;

namespace Lemon.DTO
{
    [ProtoContract]
    public partial class Account
    {
		[ProtoMember(1)]
		public CslaMetadata CslaMetadata { get; set; }
		[ProtoMember(2)]
		public System.Int32 AccountId { get; set; }
        [ProtoMember(3)]
        public System.Int32 CategoryId { get; set; }
        [ProtoMember(4)]
        public AccountType AccountType { get; set; }
        [ProtoMember(5)]
        public System.String AccountNumber { get; set; }
        [ProtoMember(6)]
        public System.String AccountDescription { get; set; }
        [ProtoMember(7)]
        public System.Decimal Balance { get; set; }
		[ProtoMember(8)]
		public byte[] Timestamp { get; set; }
		[ProtoMember(9)]
		public System.DateTime LastModDatetime { get; set; }

    }
}

namespace Lemon.DTO.Mapping
{
    [Export(typeof(IDataMapping))]
    public partial class TaxCodeDataMapping : IDataMapping
    {
        public void InitializeDataMappings()
        {
            DataMapper.Instance.RegisterMapping<Lemon.Model.Account, Lemon.DTO.Account>(ToDTO);
            DataMapper.Instance.RegisterMapping<Lemon.DTO.Account, Lemon.Model.Account>(FromDTO);
        }

        private Lemon.DTO.Account ToDTO(Lemon.Model.Account bo)
        {
            var dto = new Lemon.DTO.Account();
            dto.AccountId = bo.AccountId;
            dto.AccountType = bo.AccountType;
            dto.CategoryId = bo.CategoryId;
            dto.AccountNumber = bo.AccountNumber;
            dto.AccountDescription = bo.AccountDescription;
            dto.Balance = bo.Balance;
            dto.Timestamp = bo.Timestamp;
            dto.LastModDatetime = bo.LastModDatetime;
			dto.CslaMetadata = bo.CslaMetadata;
			ExtraToDTO(bo, dto);			
			return dto;
        }

        partial void ExtraToDTO(Lemon.Model.Account bo, Lemon.DTO.Account dto);

        private Lemon.Model.Account FromDTO(Lemon.DTO.Account dto)
        {
            var bo = new Lemon.Model.Account();
			using(new DoActionDeactivator(bo))
			{
                bo.AccountId = dto.AccountId;
                bo.AccountType = dto.AccountType;
                bo.CategoryId = dto.CategoryId;
                bo.AccountNumber = dto.AccountNumber;
                bo.AccountDescription = dto.AccountDescription;
                bo.Balance = dto.Balance;
                bo.Timestamp = dto.Timestamp;
                bo.LastModDatetime = dto.LastModDatetime;
				bo.CslaMetadata = dto.CslaMetadata;
				ExtraFromDTO(dto, bo);
				return bo;
			}
        }

        partial void ExtraFromDTO(Lemon.DTO.Account dto, Lemon.Model.Account bo);
    }
}
