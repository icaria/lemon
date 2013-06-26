using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Winterspring.LanguageExtensions;
using System.Threading;

namespace Winterspring.Lemon.Base
{
    public static class Util
    {
        //Tries an action a few times.  It either returns having completed the action with no exception, or throws an exception if the max tries were reached.
        public static void RetryAction(Action action, int maxTries, int millisecondsDelay)
        {
            int triesSoFar = 0;
            while (true)
            {
                try
                {
                    if (triesSoFar > 0)
                        Thread.Sleep(millisecondsDelay);

                    action();
                    //If we got through without an exception, success!
                    return;
                }
                catch (Exception ex)
                {
                    triesSoFar += 1;
                    if (triesSoFar >= maxTries)
                        throw;
                }
            }
        }

        //Tries an action a few times.  It either returns having completed the action with no exception, or throws an exception if the max tries were reached.
        public static async void RetryActionAsync(Task action, int maxTries, int millisecondsDelay)
        {
            int triesSoFar = 0;
            while (true)
            {
                try
                {
                    if (triesSoFar > 0)
                        //await TaskEx.Delay(millisecondsDelay);
                    await action;
                    //If we got through without an exception, success!
                    return;
                }
                catch (Exception ex)
                {
                    triesSoFar += 1;
                    if (triesSoFar >= maxTries)
                        throw;
                }
            }
        }

        //Tries an action a few times.  It either returns having completed the action with no exception, or throws an exception if the max tries were reached.
        public static async Task<T> RetryTaskAsync<T>(Func<Task<T>> action, int maxTries, int millisecondsDelay)
        {
            int triesSoFar = 0;
            while (true)
            {
                try
                {
                    if (triesSoFar > 0)
                        //await TaskEx.Delay(millisecondsDelay);
                    return await action();  //If we got through without an exception, success!
                }
                catch (Exception ex)
                {
                    triesSoFar += 1;
                    if (triesSoFar >= maxTries)
                        throw;
                }
            }
        }

        public static void RepeatAction(int numTimes, Action action)
        {
            for (int i = 0; i < numTimes; ++i)
                action();
        }

        public static string CodeToHumanReadable(string propName)
        {
            // parse out the Id at the back
            if (propName.EndsWith("Id")
                && propName.Length > 2
                && char.IsLower(propName[propName.Length - 3]))
                propName = propName.Substring(0, propName.Length - 2);

            Util.InsertSpaceBeforeUpperCase(ref propName);

            return propName;
        }

        public static void InsertSpaceBeforeUpperCase(ref string s)
        {
            for (int i = 1; i < s.Length; i++)
            {
                if (char.IsUpper(s[i]))
                {
                    s = s.Insert(i, " ");
                    i++;
                }
            }
        }

        public static bool HasNewItem<T>(IEnumerable<T> list) where T : Csla.Core.BusinessBase
        {
            foreach (Csla.Core.BusinessBase b in list)
                if (b.IsNew) return true;
            return false;
        }

        public static T GetObjectWithBiggestTimestamp<T>(IEnumerable<T> list) where T : IObjectWithTimestamp
        {
            byte[] biggestTimestamp = null;
            T o = default(T);
            foreach (T t in list)
            {
                //Find the biggest timestamp
                if (ArrayUtil.CompareArray<byte>(t.Timestamp, biggestTimestamp) > 0)
                {
                    biggestTimestamp = t.Timestamp;
                    o = t;
                }
            }
            return o;
        }

        public static string ByteArrayToHexString(byte[] bs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("0x");
            string init = "";
            foreach (byte b in bs)
            {
                sb.Append(init);
                sb.Append(String.Format("{0:X2}", b));
                init = "";
            }
            return sb.ToString();
        }

        private static string ByteArrayToHexArray(byte[] bs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            string init = "";
            foreach (byte b in bs)
            {
                sb.Append(init);
                sb.Append(String.Format("0x{0:X2}", b));
                init = ", ";
            }
            sb.Append("}");
            return sb.ToString();
        }

        public static PropertyDescriptor GetPropertyDescriptor(Type itemType, string propertyName)
        {
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(itemType))
            {
                if (prop.Name == propertyName)
                {
                    return prop;
                }
            }
            return null;
        }

        public static bool IsValidId(int? id)
        {
            return id != null && (int)id > 0;
        }

        public static bool IsEmpty(DateTime? date)
        {
            return date == null;
        }

        public static bool IsEmpty(decimal? value)
        {
            return value == null || value == Decimal.Zero;
        }

        public static bool IsEmpty(string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsEmpty(byte[] value)
        {
            return value == null || value.Length == 0;
        }

        //http://stackoverflow.com/questions/194157/c-how-to-get-program-files-x86-on-vista-x64
        public static string GetProgramFilesPath()
        {
            if (8 == IntPtr.Size ||
                (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                //x64 machines
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }
            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        #region Translation

        //Now if we have a helper method setup to do translation, do so.
        //In inFlow, this should be initialized to LanguageHelper.GetResource(s)
        public delegate string TranslateDelegate(string defaultText);

        private static TranslateDelegate _TranslateDelegate = null;

        public static void SetTranslateDelegate(TranslateDelegate f)
        {
            _TranslateDelegate = f;
        }

        public static string Translate(string s)
        {
            if (s == null) return "";

            if (Util._TranslateDelegate != null)
                return _TranslateDelegate(s);
            return s;
        }

        /// <summary>In the current implementation for inFlow, this should actually be a System.Windows.Forms.ContainerControl, or it's ignored</summary>
        public delegate void RegisterAndTranslateContainerDelegate(IComponent containerComponent);

        private static RegisterAndTranslateContainerDelegate _RegisterAndTranslateContainerDelegate = null;

        public static void SetRegisterAndTranslateContainerDelegate(RegisterAndTranslateContainerDelegate f)
        {
            _RegisterAndTranslateContainerDelegate = f;
        }

        public static void RegisterAndTranslateContainer(IComponent containerControl)
        {
            if (Util._RegisterAndTranslateContainerDelegate != null)
                _RegisterAndTranslateContainerDelegate(containerControl);
            return;
        }


        #endregion Translation
    }
}
