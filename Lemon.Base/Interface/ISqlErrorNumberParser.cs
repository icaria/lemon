using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using System.Data.SqlClient;

namespace Lemon.Base
{
    public interface ISqlErrorNumberParser
    {
        //The error code number that this parser is registered to
        SqlErrorNumber Number { get; }
        
        /// <summary>
        /// Possibly parse a SqlError into an interpreted ServerReturnedError.
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="error"></param>
        /// <returns>null if not parsed successfully</returns>
        ServerReturnedError ParseSqlError(SqlError error);
    }
}
