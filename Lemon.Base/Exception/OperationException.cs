using System;
using System.Collections.Generic;
using System.Text;

namespace Winterspring.Lemon.Base
{
    /// <summary>
    /// Use this class to throw exceptions for errors in doing some operation,
    /// like completing a document.  In these cases, ValidationException is not
    /// quite right because you might want to let the user save the record
    /// in an intermediate state, except when it's being completed.
    /// </summary>
    public class OperationException : System.Exception
    {
        private string title;
        public string Title { get { return this.title; } set { this.title = value; } }
        private string description;
        public string Description { get { return this.description; } set { this.description = value; } }

        public OperationException(string title, string description)
            : base(description)
        {
            this.title = title;
            this.description = description;
        }
    }
}
