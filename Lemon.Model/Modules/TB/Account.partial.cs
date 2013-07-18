using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lemon.Base;

namespace Lemon.Model
{
    [Serializable()]
    public partial class Account : GenAccount
    {
        protected static Account securityInstance;
        /// This is a public dummy instance that should only be
        /// used for accessing security rights like CanEditObject().
        /// It's kind of a hack, but it's a trick that lets us have
        /// overrideable CanEditObject() sort of rights while also
        /// making them statically accessible.
        public static Account SecurityInstance
        {
            get
            {
                if (securityInstance == null)
                {
                    securityInstance = new Account();
                }
                return securityInstance;
            }
        }

        /// <summary>
        /// This accessor method creates a new object.
        /// The object is explicitly marked clean.
        /// Note that this differs from the usual CSLA semantics of
        /// new objects being dirty.
        /// </summary>
        /// <returns>A clean new TaxCode</returns>
        internal static Account NewAccount()
        {
            return new Account();
        }

        internal static Account GetAccount(NullableDataReader dr)
        {
            return new Account(dr);
        }
    }
}
