using System;
using System.Runtime.Serialization;

namespace Winterspring.Extensions
{
    [Serializable]
    public class WinterspringCustomException : Exception 
    {
        public WinterspringCustomException() 
        {
            Title = "";
        }

        public WinterspringCustomException(string message) : base(message) { }
        public WinterspringCustomException(string message, Exception innerException) : base(message, innerException) { }
        
        public WinterspringCustomException(string message, string title) : base(message) 
        {
            Title = title;
        }

        public string Title { get; private set; }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue("Title", Title);
            }
        }

        protected WinterspringCustomException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                Title = info.GetString("Title");
            }
        }
    }
}