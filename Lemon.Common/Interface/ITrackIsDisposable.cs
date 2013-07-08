using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lemon.Base
{
    public interface ITrackDisposable
    {
        bool IsDisposed { get; }
    }
}
