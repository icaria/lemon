using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lemon.Models
{
    public class AccountType
    {
        [Key]
        public int AccountTypeId { get; set; }
        public string Type { get; set; }
    }
}
