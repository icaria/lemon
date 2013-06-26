using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Data.SqlClient;

namespace Winterspring.Lemon.Base
{
    [Serializable]
    public abstract class WinterspringCommandBase<T> : CommandBase<T> where T: WinterspringCommandBase<T>
    {
        /// <summary>
        /// You might choose to override this with a new SqlErrorParser
        /// and customize the SqlException -> ServerReturnedException
        /// conversion step from the default.
        /// If you don't want to use SqlErrorParser at all, then override
        /// the DataPortal_Execute method.
        /// </summary>
        /// <returns></returns>
        protected virtual SqlErrorParser DataPortal_GetSqlErrorParser()
        {
            return SqlErrorParser.PublicInstance;
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Execute()
        {
            try
            {
                DataPortal_ExecuteBody();
            }
            catch (Exception ex)
            {
                //If it's already been wrapped with a DataPortalException, then
                //unwrap it and handle that exception.
                if (ex is DataPortalException && (ex as DataPortalException).BusinessException != null)
                {
                    ex = (ex as DataPortalException).BusinessException;
                }
                if (ex is Csla.Rules.ValidationException)
                {
                    throw ex;
                }
                if (ex is ServerReturnedException)
                {
                    throw ex;
                }
                else if (ex is SqlException)
                {
                    throw DataPortal_GetSqlErrorParser().ParseSqlException(ex as SqlException);
                }
                else if (ex is OperationException)
                {
                    throw;
                }
                else if (ex is InvalidOperationException && ex.Message.Contains("SQL Server Service Broker"))
                {
                    ServerReturnedException sre = new ServerReturnedException(ex.Message, ex);
                    sre.AddError(new ServerReturnedError(ServerReturnedError.ErrorNumber.SqlServerServiceBrokerError));
                    throw sre;
                }
                else
                {
                    throw new ServerReturnedException(ex.Message, ex);
                }
            }

        }

        /// <summary>
        /// Subclasses should implement one of these variants of DataPortal_ExecuteBody (depending on whether they want
        /// to create a connection in the default way or not) with the main SQL logic.
        /// It will be wrapped by the error handling routine in DataPortal_Execute()
        /// </summary>
        protected virtual void DataPortal_ExecuteBody()
        {
            using (var mgr = WinterspringConnectionManager.GetManager())
            {
                var cn = mgr.Connection;
                DataPortal_ExecuteBody(cn);
            }
        }

        protected virtual void DataPortal_ExecuteBody(SqlConnection cn)
        {
        }

    }
}
