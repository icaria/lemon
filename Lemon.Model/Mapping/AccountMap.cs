using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Lemon.Model
{
    public class AccountMap : EntityTypeConfiguration<Account>
    {
        public AccountMap()
        {
            ToTable("Account");
            HasKey(c => c.AccountId).Property(c => c.AccountId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(c => c.AccountType).IsRequired();
            Property(c => c.CategoryId).IsRequired();
        }

    }
}