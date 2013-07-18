/* AUTO-GENERATED FILE - Replace the hash code below with the word HAND to mark as hand-edited, or WIPE to overwrite it next time.*/
/* INTEGRITY CHECK:5a1dfda57149a025f963d54f2dcfb635*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lemon.Base;
using Lemon.Common;
using ProtoBuf;
using Csla;
using System.ComponentModel.Composition;
using Winterspring.DataPortal;

namespace Lemon.DTO
{
    [ProtoContract]
    public partial class AccountList
    {
        [ProtoMember(1)]
        public List<Lemon.DTO.Account> Items { get; set; }
        [ProtoMember(2)]
        public bool HasDataConflict { get; set; }
    }
}

namespace Lemon.DTO.Mapping
{
    [Export(typeof(IDataMapping))]
    public partial class TaxCodeListDataMapping : IDataMapping
    {
        public void InitializeDataMappings()
        {
            DataMapper.Instance.RegisterMapping<Lemon.Model.AccountList, Lemon.DTO.AccountList>(ToDTO);
            DataMapper.Instance.RegisterMapping<Lemon.DTO.AccountList, Lemon.Model.AccountList>(FromDTO);
        }

        private Lemon.DTO.AccountList ToDTO(Lemon.Model.AccountList bo)
        {
            var dto = new Lemon.DTO.AccountList();
            dto.Items = bo.Select(x => (Lemon.DTO.Account)DataMapper.Instance.Map(x)).ToList();
            dto.HasDataConflict = bo.HasDataConflict;
            ExtraToDTO(bo, dto);
            return dto;
        }

        partial void ExtraToDTO(Lemon.Model.AccountList bo, Lemon.DTO.AccountList dto);

        private Lemon.Model.AccountList FromDTO(Lemon.DTO.AccountList dto)
        {
            var bo = Lemon.Model.AccountList.NewAccountList();
            using (new DoActionDeactivator(bo))
            {
                if (dto.Items != null)
                    bo.AddRange(dto.Items.Select(x => (Lemon.Model.Account)DataMapper.Instance.Map(x)));
                bo.HasDataConflict = dto.HasDataConflict;
                ExtraFromDTO(dto, bo);
                return bo;
            }
        }

        partial void ExtraFromDTO(Lemon.DTO.AccountList dto, Lemon.Model.AccountList bo);

    }
}