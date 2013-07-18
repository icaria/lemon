using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lemon.DTO
{
    public partial class Account
    {
        //Add extra stuff here
        //[ProtoMember(1000)]
        //public int ExtraInformation { get; set; }
    }
}

namespace Lemon.DTO.Mapping
{
    public partial class AccountDataMapping
    {
        //Implement these methods to add extra functionality at the end of ToDTO and FromDTO
        //partial void ExtraToDTO(Archon.Rapid.Module.TaxCode bo, Archon.Rapid.DTO.TaxCode dto);		
        //partial void ExtraFromDTO(Archon.Rapid.DTO.TaxCode dto, Archon.Rapid.Module.TaxCode bo);
    }
}
