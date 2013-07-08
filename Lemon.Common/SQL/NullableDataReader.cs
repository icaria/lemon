using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace Lemon.Base
{
    public class NullableDataReader : SafeDataReader
    {

        public NullableDataReader(SqlDataReader dr)
            : base(dr)
        {
        }

        public bool? GetNullableBoolean(string name)
        {
            return GetNullableBoolean(this.GetOrdinal(name));
        }

        public bool? GetNullableBoolean(int i)
        {
            if (this.IsDBNull(i))
                return null;
            else
                return base.GetBoolean(i);
        }


        public short? GetNullableInt16(string name)
        {
            return GetNullableInt16(this.GetOrdinal(name));
        }

        public short? GetNullableInt16(int i)
        {
            if (this.IsDBNull(i))
                return null;
            else
                return base.GetInt16(i);
        }


        public int? GetNullableInt32(string name)
        {
            return GetNullableInt32(this.GetOrdinal(name));
        }

        public int? GetNullableInt32(int i)
        {
            if (this.IsDBNull(i))
                return null;
            else
                return base.GetInt32(i);
        }


        public long? GetNullableInt64(string name)
        {
            return GetNullableInt64(this.GetOrdinal(name));
        }

        public long? GetNullableInt64(int i)
        {
            if (this.IsDBNull(i))
                return null;
            else
                return base.GetInt64(i);
        }


        public float? GetNullableFloat(string name)
        {
            return GetNullableFloat(this.GetOrdinal(name));
        }

        public float? GetNullableFloat(int i)
        {
            if (this.IsDBNull(i))
                return null;
            else
                return base.GetFloat(i);
        }


        public decimal? GetNullableDecimal(string name)
        {
            return GetNullableDecimal(this.GetOrdinal(name));
        }

        public decimal? GetNullableDecimal(int i)
        {
            if (this.IsDBNull(i))
                return null;
            else
                return base.GetDecimal(i);
        }

        public DateTime? GetNullableDateTime(string name)
        {
            return GetNullableDateTime(this.GetOrdinal(name));
        }

        private DateTime? GetNullableDateTime(int i)
        {
            if (this.IsDBNull(i))
                return null;
            else
                return base.GetDateTime(i);
        }

    }
}
