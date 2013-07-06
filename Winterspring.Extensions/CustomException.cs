using System;
using System.Runtime.Serialization;

namespace Winterspring.Extensions
{
    [Serializable]
    public class CustomException : Exception 
    {
        public CustomException() 
        {
            Title = "";
        }

        public CustomException(string message) : base(message) { }
        public CustomException(string message, Exception innerException) : base(message, innerException) { }
        
        public CustomException(string message, string title) : base(message) 
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

        protected CustomException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                Title = info.GetString("Title");
            }
        }
    }
}