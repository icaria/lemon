﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v6.5.3, CSLA Templates: v4.0.0.0, CSLA Framework: v4.5.x.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'AccountList.cs'.
//
//     Template: EditableChildList.DataAccess.ParameterizedSQL.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Csla;
using Csla.Data;


namespace Lemon.Model
{
    public partial class AccountList
    {
        protected override void Child_Create()
        {
            bool cancel = false;
            OnCreating(ref cancel);
            if (cancel) return;
        
            OnCreated();
        }
    
        private void Child_Fetch(AccountCriteria criteria)
        {
            bool cancel = false;
            OnFetching(criteria, ref cancel);
            if (cancel) return;

            RaiseListChangedEvents = false;

            // Fetch Child objects.
            string commandText = String.Format("SELECT [AccountId], [CategoryId], [AccountTypeId], [AccountNumber], [AccountDescription], [Balance], [AccountTypeId1], [HasDataConflict], [SyncMode], [DoMode], [CslaMetadata_IsDirty], [CslaMetadata_IsNew] FROM [dbo].[Account] {0}", ConnectionHelper.BuildWhereStatement(criteria.StateBag));
            using (var connection = new SqlConnection(ConnectionHelper.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddRange(ConnectionHelper.SqlParameters(criteria.StateBag));

                    using(var reader = new SafeDataReader(command.ExecuteReader()))
                    {
                        if(reader.Read())
                        {
                            do
                            {
                                this.Add(Lemon.Model.Account.GetAccount(reader));
                            } while(reader.Read());
                        }
                    }
                }
            }

            RaiseListChangedEvents = true;

            OnFetched();
        }
        
        /// <summary>
        /// Updates the child object.
        /// </summary>
        /// <param name="parameters">The parameters collection may contain more parameters than needed based on the context it was called. We need to filter this list.</param>
        protected override void Child_Update(params object[] parameters)
        {
            bool cancel = false;
            OnUpdating(ref cancel);
            if (cancel) return;

            // We require that one of the parameters be a connection so we can do the CRUD operations.
            var connection = parameters.OfType<SqlConnection>().FirstOrDefault();
            if (connection == null)
                throw new ArgumentNullException("parameters", "Must contain a SqlConnection parameter.");

            RaiseListChangedEvents = false;

            foreach (var item in DeletedList)
            {
                DataPortal.UpdateChild(item, connection);
            }

            DeletedList.Clear();
            
            // Trim down the list to what is actually contained in the child class.
            var list = new System.Collections.Generic.Dictionary<string, object>() {};
            foreach (object o in parameters)
            {
                if(o == null) continue;

                var key = o.GetType().ToString();
                if (!list.ContainsKey(key))
                    list.Add(key, o);
            }

            foreach (var item in Items)
            {
                DataPortal.UpdateChild(item, list.Values.ToArray());
            }

            RaiseListChangedEvents = true;

            OnUpdated();
        }
    }
}