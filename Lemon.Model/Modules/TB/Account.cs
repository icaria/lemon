using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Lemon.Base;
using Lemon.Common;

namespace Lemon.Model
{
    public partial class Account : GenAccount
    {
        static Account()
	    {
            Mapper.CreateMap<Account, AccountCacheObject>();
            Mapper.CreateMap<AccountCacheObject, Account>();
	    }

        /* require use of factory methods */
        public Account() : base()
        {  
        }

        public Account(AccountCacheObject cacheObject)
        {
            Mapper.Map(cacheObject, this);            
            MarkOld();
        }

        protected Account(NullableDataReader dr)
            : base(dr)
        {
        }
    }
}
