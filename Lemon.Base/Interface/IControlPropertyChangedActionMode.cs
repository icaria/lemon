using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterspring.Lemon.Base
{
    public interface IControlPropertyChangedActionMode
    {
        PropertyChangedActionMode SyncMode { get; set; }
        PropertyChangedActionMode DoMode { get; set; }
    }
}
