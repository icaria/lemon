using System;
using System.Collections.Generic;

namespace Lemon.Common
{
    public class DataChangedEventArgsWithSender : DataChangedEventArgs
    {
        public object Sender { get; private set; }

        public DataChangedEventArgsWithSender(object sender, DataChangeOperation operation, string tableName, List<int> deletedIds = null)
            : base(operation, tableName, deletedIds)
        {
            Sender = sender;
        }
    }

    public class DataChangedEventArgs
    {
        public DataChangeOperation Operation { get; private set; }
        public string TableName { get; private set; }
        public List<int> DeletedIds { get; private set; }

        public DataChangedEventArgs(DataChangeOperation operation, string tableName, List<int> deletedIds = null)
        {
            Operation = operation;
            TableName = tableName;
            DeletedIds = deletedIds;
        }
    }
}