﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lemon.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public List<Account> Account { get; set; }
        public string Name { get; set; }
    }
}
