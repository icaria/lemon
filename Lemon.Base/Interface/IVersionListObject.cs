using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using Csla.Data;

namespace Winterspring.Lemon.Base
{
    public interface IVersionListObject
    {

        int Id
        {
            get;
        }

        int Version
        {
            get;
        }
        int LastModUserId
        {
            get;
        }
        string LastModUserName
        {
            get;
        }
        DateTime LastModDttm
        {
            get;
            set;
        }
    }
}
