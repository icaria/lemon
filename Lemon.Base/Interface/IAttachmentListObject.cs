using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using Csla.Data;

namespace Archon.Rapid.Base
{
    public interface IAttachmentListObject
    {

        int Id
        {
            get;
        }

        int ParentId
        {
            get;
        }

        int FileAttachmentId
        {
            get;
        }
    }
}
