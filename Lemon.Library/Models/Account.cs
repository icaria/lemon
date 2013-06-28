using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lemon.Models
{
    public class Account
    {
        [Key]
        // primary account key
        public int AccountId { get; set; }
        // link to category key
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        // link to account type key
        public int AccountTypeId { get; set; }
        [ForeignKey("AccountTypeId")]
        public virtual AccountType AccountType { get; set; }
        
        // these are for storing actual information relating to each account
        public string AccountNumber { get; set; }  // not actually a number - sup bro
        public string AccountDescription { get; set; }
        public float Balance { get; set; }
        
    }
}
