using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Lemon.Base
{
    [Serializable]
    public class ServerReturnedException : Exception
    {
        public IWinterspringBusinessBase BusinessObject { get; set; }
        public bool HasUnknownError { get; internal set; }
        public List<ServerReturnedError> Errors { get; private set; }

        public ServerReturnedException()
        {
            Errors = new List<ServerReturnedError>();
        }

        public ServerReturnedException(ServerReturnedError error)
        {
            Errors = new List<ServerReturnedError>();
            AddError(error);
        }

        public ServerReturnedException(string message, Exception innerException)
            : base(message, innerException)
        {
            Errors = new List<ServerReturnedError>();
        }

        public void AddError(ServerReturnedError error)
        {
            Errors.Add(error);
        }

        //Sadly, custom exceptions seem to need their own serialization handling
        //http://blog.gurock.com/articles/creating-custom-exceptions-in-dotnet/
        protected ServerReturnedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            HasUnknownError = false;
            Errors = new List<ServerReturnedError>();

            if (info != null)
            {
                BusinessObject = (IWinterspringBusinessBase)info.GetValue("BusinessObject", typeof(IWinterspringBusinessBase));
                HasUnknownError = info.GetBoolean("HasUnknownError");
                Errors = (List<ServerReturnedError>) info.GetValue("Errors", typeof(List<ServerReturnedError>));
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue("BusinessObject", BusinessObject);
                info.AddValue("HasUnknownError", HasUnknownError);
                info.AddValue("Errors", Errors);
            }
        }
    }
}
