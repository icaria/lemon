using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lemon.Models;

namespace Lemon.Mapping
{
    public class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            ToTable("Category");
            HasKey(c => c.CategoryId).Property(c => c.CategoryId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

    }
}
