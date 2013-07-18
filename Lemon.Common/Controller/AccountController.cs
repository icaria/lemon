using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winterspring.Extensions;

namespace Lemon.Common
{
    public static class AccountController
    {
        public static IAccountCache Cache { get { return ServiceLocator.Get<IAccountCache>(); } }

        public static AccountCacheObject GetByAccountNumber(string accountNumber)
        {
            return Cache.FirstOrDefault(x => x.AccountNumber.Trim().ToLower() == accountNumber.Trim().ToLower());
        }

        public static string GetAccountNumber(int accountId)
        {
            return Cache[accountId].TryOr(x => x.AccountNumber, () => "");
        }
    }
}
