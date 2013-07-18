using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lemon.Base;

namespace Lemon.Model
{
    [Serializable()]
    public partial class AccountList : GenAccountList
    {
        internal static AccountList NewAccountList()
        {
            return new AccountList();
        }

        internal static AccountList GetAccountList(NullableDataReader dr)
        {
            AccountList o = new AccountList();
            o.Fetch(dr);
            return o;
        }

    }
}
