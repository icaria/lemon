using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lemon.Modules
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public string Name { get; set; }
    }
}
