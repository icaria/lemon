using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Lemon.Common;
using Ninject.Extensions.Conventions;

namespace Lemon.Common
{
    //We should minimize using this class if possible. We should let ninject resolve all the dependencies from the root level via constructor injection
    //Only use this if it's really hard to code constructor injection because of serialization problems, etc
    public static class ServiceLocator
    {
        private static IKernel _kernel;

        private static IKernel GetMockKernel()
        {
            return new StandardKernel(new MockCacheModule());
        }

        private static IKernel Kernel 
        { 
            get 
            {
                //If we're running from the designer or CodeGeneration, then we may not have called Initialize.  In that case, get a mock kernel so that things don't crash.
                if (_kernel == null)
                    _kernel = GetMockKernel();
                
                return _kernel; 
            } 
        }

        public static void Initialize(IKernel kernel)
        {                        
            _kernel = kernel;
        }

        public static T Get<T>() where T : class
        {
            if (Kernel == null) return null;
            return Kernel.Get<T>();            
        }

        public static IEnumerable<T> GetAll<T>()
        {
            if (Kernel == null) return new T[] { };
            return Kernel.GetAll<T>();
        }

        public static T TryGet<T>() where T : class
        {
            if (Kernel == null) return null;
            return Kernel.TryGet<T>();
        }
    }
}
