using System;
using System.Collections.Generic;
using System.Text;

namespace Winterspring.Lemon.Base
{
    public interface IObjectWithTimestamp
    {
        byte[] Timestamp { get; }
    }
}
