using System;
using System.Collections.Generic;
using System.Text;

namespace Lemon.Base
{
    
    [Serializable()]
    public class ServerReturnedError
    {
        public enum ErrorNumber
        {
            DbConnectionTimeout = -2,
            ErrorEstablishingDbConnection = 2,
            ErrorEstablishingConnection = 53,
            TransportErrorNoProcessPipe = 233,
            TransactionCountDifference = 266,
            CheckConstraintFailed = 547,
            MustBeUnique = 2627,
            StatementHasBeenTerminated = 3621,
            DbAccessDenied = 4060,
            DbConnectionLost = 10054,
            DbConnectionCouldNotBeMade = 10061,
            CustomErrorMessageNotSpecified = 18054,
            DbLoginFailed = 18456,
            RowHasBeenEdited = 50001,  //Custom exception
            MustBeStockedProduct = 50002,  //Custom exception
            NegativeInventory = 50003, //Custom exception
            NeedActiveAdministratorUser = 50004, //Custom exception
            BomCircularReference = 50005, //Custom exception
            NegativeSerialNumberInventory = 50006, //Custom exception
            WorkOrderPartNegativeInventory = 50007, //Custom exception
            ItemTypeChangeError = 50008, //Custom exception -- can't change item type once an item has been used.
            UniqueSerialNumberInventory = 50009, //Custom exception
            ItemHasBeenDeleted = 100001,
            DBVersionMismatch = 100002,
            HitRecordLimit = 100003,
            UnknownDatabaseConnectionError = 100004, //When we can't connect to the database server at all, we get an unknown error code
            SqlServerServiceBrokerError = 100005, //This is returned as an InvalidOperationException, not a SqlException
        }

        private ErrorNumber number;
        private string message;
        private Dictionary<string, object> data;

        public ErrorNumber Number
        {
            get { return number; }
            internal set { number = value; }
        }

        public string Message
        {
            get { return message; }
            internal set { message = value; }
        }

        public Dictionary<string, object> Data
        {
            get { return data; }
        }

        public ServerReturnedError(ErrorNumber number)
        {
            this.number = number;
            this.message = string.Empty;
            this.data = new Dictionary<string, object>();
        }

        public ServerReturnedError(ErrorNumber number, string message)
        {
            this.number = number;
            this.message = message;
            this.data = new Dictionary<string, object>();
        }

    }
}
