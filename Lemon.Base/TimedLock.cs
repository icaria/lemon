using System;
using System.Threading;
using System.Diagnostics;

// This class will try to avoid deadlock by doing a time out
// http://www.interact-sw.co.uk/iangblog/2004/04/26/yetmoretimedlocking

// Thanks to Eric Gunnerson for recommending this be a struct rather
// than a class - avoids a heap allocation.
// Thanks to Change Gillespie and Jocelyn Coulmance for pointing out
// the bugs that then crept in when I changed it to use struct...
// Thanks to John Sands for providing the necessary incentive to make
// me invent a way of using a struct in both release and debug builds
// without losing the debug leak tracking.

namespace Lemon.Base
{
    public struct TimedLock : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static TimedLock Lock(object o)
        {
            return Lock(o, TimeSpan.FromSeconds(BaseConstants.TimedLockWaitSeconds));
        }

        public static TimedLock Lock(object o, TimeSpan timeout)
        {
            TimedLock tl = new TimedLock(o);
            if (!Monitor.TryEnter(o, timeout))
            {
                log.Warn("WARNING : Potential deadlock.  Cannot get lock for " + o.GetType().Name);
                if (log.IsDebugEnabled)
                {
                    log.Debug(Environment.StackTrace);
                }

                tl.enteredMonitor = false;
#if DEBUG
                System.GC.SuppressFinalize(tl.leakDetector);
                log.Fatal("WARNING : Potential deadlock.  Cannot get lock for " + o.GetType().Name);
                log.Fatal(Environment.StackTrace);
                throw new ApplicationException("Timeout waiting for lock.");
#endif
            }

            return tl;
        }

        private TimedLock(object o)
        {
            target = o;
            enteredMonitor = true;
#if DEBUG
            leakDetector = new Sentinel();
#endif
        }

        private object target;
        private bool enteredMonitor;
        public void Dispose()
        {
            if (enteredMonitor)
                Monitor.Exit(target);

            // It's a bad error if someone forgets to call Dispose,
            // so in Debug builds, we put a finalizer in to detect
            // the error. If Dispose is called, we suppress the
            // finalizer.
#if DEBUG
            GC.SuppressFinalize(leakDetector);
#endif
        }

#if DEBUG
        // (In Debug mode, we make it a class so that we can add a finalizer
        // in order to detect when the object is not freed.)
        private class Sentinel
        {
            ~Sentinel()
            {
                // If this finalizer runs, someone somewhere failed to
                // call Dispose, which means we've failed to leave
                // a monitor!
                log.Fatal("Undisposed lock");
                System.Diagnostics.Debug.Fail("Undisposed lock");
            }
        }
        private Sentinel leakDetector;
#endif
    }
}
