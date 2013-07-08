using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lemon.Modules;

namespace Lemon.Mapping
{
    public class AccountTypeMap : EntityTypeConfiguration<AccountType>
    {
        public AccountTypeMap()
        {
            ToTable("AccountType");
            HasKey(c => c.AccountTypeId).Property(c => c.AccountTypeId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(c => c.Type).IsRequired();
        }

    }
}