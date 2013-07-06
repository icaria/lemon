using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Lemon.Models;

namespace Lemon.Mapping
{
    public class AccountMap : EntityTypeConfiguration<Account>
    {
        public AccountMap()
        {
            ToTable("Account");
            HasKey(c => c.AccountId).Property(c => c.AccountId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(c => c.AccountTypeId).IsRequired();
            Property(c => c.CategoryId).IsRequired();
        }

    }
}