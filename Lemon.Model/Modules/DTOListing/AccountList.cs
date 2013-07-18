using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Csla;
using System.ComponentModel.Composition;

namespace Lemon.DTO
{
    public partial class AccountList
    {
        //Add extra stuff here
        //[ProtoMember(100)]
        //public int ExtraInformation { get; set; }
    }
}

namespace Lemon.DTO.Mapping
{
    public partial class AccountListDataMapping
    {
        //Implement these methods to add extra functionality at the end of ToDTO and FromDTO
        //partial void ExtraToDTO(Archon.Rapid.Module.TaxCodeList bo, Archon.Rapid.DTO.TaxCodeList dto);		
        //partial void ExtraFromDTO(Archon.Rapid.DTO.TaxCodeList dto, Archon.Rapid.Module.TaxCodeList bo);
    }
}