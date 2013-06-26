using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Csla;

namespace Winterspring.Lemon.Base
{

    /// <summary>
    /// The number values here are actually important,
    /// because they're compared against the values returned from
    /// SQL server.
    /// Use between 50000 and 100000 for custom errors we define in SQL server
    /// and over 100000 for errors we define separately that aren't thrown
    /// from SQL server.
    /// </summary>
    public enum SqlErrorNumber
    {
        DbConnectionTimeout = -2,
        UnknownSqlErrorNumber = -1, 
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
        ItemTypeChangeError = 50008, //Custom exception
        ItemHasBeenDeleted = 100001,  //Custom exception
    }

    public class SqlErrorParser
    {
        private static SqlErrorParser publicInstance;

        public static SqlErrorParser PublicInstance
        {
            get 
            {
                if (publicInstance == null)
                {
                    publicInstance = new SqlErrorParser(true);
                }
                return publicInstance;
            }
        }

        public ServerReturnedException ParseSqlException(SqlException ex, IWinterspringBusinessBase bo)
        {
            ServerReturnedException e = ParseSqlException(ex);
            e.BusinessObject = bo;
            return e;
        }

        public ServerReturnedException ParseSqlException(SqlException ex)
        {
            ServerReturnedException re = new ServerReturnedException(ex.Message, ex);
            re.HasUnknownError = false;
            foreach (SqlError error in ex.Errors)
            {
                ServerReturnedError sre = ParseSqlError(error);
                if (sre != null)
                {
                    re.AddError(sre);
                }
                else
                {
                    re.HasUnknownError = true;                
                }
            }
            return re;
        }

        public SqlErrorParser(bool addCommonParsers) 
        {
            if (addCommonParsers)
            {
                AddCommonParsers();
            }
        }

        private List<ISqlErrorNumberParser> parsers = new List<ISqlErrorNumberParser>();

        private void AddCommonParsers()
        {
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.DbConnectionTimeout, ServerReturnedError.ErrorNumber.DbConnectionTimeout));
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.ErrorEstablishingConnection, ServerReturnedError.ErrorNumber.ErrorEstablishingConnection));
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.ErrorEstablishingDbConnection, ServerReturnedError.ErrorNumber.ErrorEstablishingDbConnection));
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.TransportErrorNoProcessPipe, ServerReturnedError.ErrorNumber.TransportErrorNoProcessPipe));
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.TransactionCountDifference, ServerReturnedError.ErrorNumber.TransactionCountDifference));
            parsers.Add(new IgnoreSqlErrorNumberParser(SqlErrorNumber.TransactionCountDifference));
            parsers.Add(new UniqueConstraintParser());
            parsers.Add(new StatementHasBeenTerminatedParser());
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.RowHasBeenEdited, ServerReturnedError.ErrorNumber.RowHasBeenEdited));
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.CustomErrorMessageNotSpecified, ServerReturnedError.ErrorNumber.CustomErrorMessageNotSpecified));
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.DbConnectionLost, ServerReturnedError.ErrorNumber.DbConnectionLost));
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.DbConnectionCouldNotBeMade, ServerReturnedError.ErrorNumber.DbConnectionCouldNotBeMade));
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.DbAccessDenied, ServerReturnedError.ErrorNumber.DbAccessDenied));
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.DbLoginFailed, ServerReturnedError.ErrorNumber.DbLoginFailed));
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.MustBeStockedProduct, ServerReturnedError.ErrorNumber.MustBeStockedProduct));
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.NeedActiveAdministratorUser, ServerReturnedError.ErrorNumber.NeedActiveAdministratorUser));
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.BomCircularReference, ServerReturnedError.ErrorNumber.BomCircularReference));
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.ItemTypeChangeError, ServerReturnedError.ErrorNumber.ItemTypeChangeError));
            parsers.Add(new GenericSqlErrorNumberParser(SqlErrorNumber.CheckConstraintFailed, ServerReturnedError.ErrorNumber.CheckConstraintFailed));
            parsers.Add(new UnknownSqlErrorNumber());
        }
        
        public void AddParser(ISqlErrorNumberParser parser)
        {
            if(this == PublicInstance)
            {
                throw new InvalidOperationException("Should not call AddParser to the shared instance of SqlErrorParser.  Create your own instance instaed.");
            }
            parsers.Add(parser);
        }

        public ServerReturnedError ParseSqlError(SqlError error)
        {
            foreach (ISqlErrorNumberParser parser in parsers)
            {
                if (error.Number == (int)parser.Number)
                {
                    ServerReturnedError e = parser.ParseSqlError(error);
                    if (e != null) return e;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Ignores a particular error
    /// </summary>
    internal class IgnoreSqlErrorNumberParser : ISqlErrorNumberParser
    {
        private SqlErrorNumber number;

        public IgnoreSqlErrorNumberParser(SqlErrorNumber number)
        {
            this.number = number;
        }
        public SqlErrorNumber Number
        {
            get { return number; }
        }
        public ServerReturnedError ParseSqlError(SqlError error)
        {
            return null;
        }
    }

    /// <summary>
    /// This can be used as for simple errors with no parameters or extra data
    /// that has to be passed back.
    /// It maps from SqlErrorNumber to ServerReturnedError
    /// </summary>
    internal class GenericSqlErrorNumberParser : ISqlErrorNumberParser
    {
        private SqlErrorNumber number;
        private ServerReturnedError.ErrorNumber targetNumber;

        public GenericSqlErrorNumberParser(SqlErrorNumber number, ServerReturnedError.ErrorNumber targetNumber)
        {
            this.number = number;
            this.targetNumber = targetNumber;
        }

        public SqlErrorNumber Number
        {
            get { return number; }
        }

        public ServerReturnedError ParseSqlError(SqlError error)
        {
            if(error.Number == (int)number)
            {
                ServerReturnedError e = new ServerReturnedError(targetNumber);
                return e;
            }
            //Nothing found, return null
            return null;
        }
    }

    internal class UnknownSqlErrorNumber : ISqlErrorNumberParser
    {
        public SqlErrorNumber Number
        {
            get { return SqlErrorNumber.UnknownSqlErrorNumber; }
        }

        public ServerReturnedError ParseSqlError(SqlError error)
        {
            if (error.Message.StartsWith("An error has occurred while establishing a connection to the server."))
            {
                ServerReturnedError e = new ServerReturnedError(ServerReturnedError.ErrorNumber.UnknownDatabaseConnectionError);
                return e;
            }
            return null;
        }
    }

    internal class StatementHasBeenTerminatedParser : ISqlErrorNumberParser
    {
        public SqlErrorNumber Number
        {
            get { return SqlErrorNumber.StatementHasBeenTerminated; }
        }

        public ServerReturnedError ParseSqlError(SqlError error)
        {
            if (error.Number == (int)SqlErrorNumber.StatementHasBeenTerminated)
            {
                ServerReturnedError e = new ServerReturnedError(ServerReturnedError.ErrorNumber.StatementHasBeenTerminated);
                return e;
            }
            //Nothing found, return null
            return null;
        }
    }

    internal class UniqueConstraintParser : ISqlErrorNumberParser
    {
        public SqlErrorNumber Number
        {
            get { return SqlErrorNumber.MustBeUnique; }
        }

        public ServerReturnedError ParseSqlError(SqlError error)
        {
            if (error.Number == (int)SqlErrorNumber.MustBeUnique)
            {
                ServerReturnedError e = new ServerReturnedError(ServerReturnedError.ErrorNumber.MustBeUnique);
                string regexPattern = @"Violation of (?<s1>.*) constraint '(?<s2>.*)'\. Cannot insert duplicate key in object '(?<s3>.*)'";
                Regex re = new Regex(regexPattern, RegexOptions.ExplicitCapture);
                Match m = re.Match(error.Message);
                if (m.Success)
                {
                    e.Data["ConstraintType"] = m.Groups["s1"].ToString();
                    e.Data["ConstraintName"] = m.Groups["s2"].ToString();
                    e.Data["TableName"] = m.Groups["s3"].ToString();
                }
                return e;
            }
            //Nothing found, return null
            return null;
        }

    }
}
