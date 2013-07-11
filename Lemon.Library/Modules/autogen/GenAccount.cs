using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Csla;
using Csla.Rules;
using Lemon.Base;
using Lemon.Common;
using Winterspring.Extensions;

namespace Lemon.Modules
{
    [Serializable()]
    public abstract class GenAccount : WinterspringBusinessBase<Account>, IWinterspringBusinessBase
    {
        [NonSerialized]
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Account));

        #region Property Definitions

        public static readonly PropertyInfo<System.Int32> AccountIdProperty = RegisterProperty<System.Int32>(p => p.AccountId, RelationshipTypes.PrivateField);
        protected System.Int32 _AccountId;
        public static readonly PropertyInfo<System.Int32> CategoryIdProperty = RegisterProperty<System.Int32>(p => p.CategoryId, RelationshipTypes.PrivateField);
        protected System.Int32 _CategoryId;
        public static readonly PropertyInfo<System.Int32> AccountTypeIdProperty = RegisterProperty<System.Int32>(p => p.AccountTypeId, RelationshipTypes.PrivateField);
        protected System.Int32 _AccountTypeId;
        public static readonly PropertyInfo<System.String> AccountNumberProperty = RegisterProperty<System.String>(p => p.AccountNumber, "AccountNumber", string.Empty, RelationshipTypes.PrivateField);
        protected System.String _AccountNumber = string.Empty;
        public static readonly PropertyInfo<System.String> AccountDescriptionProperty = RegisterProperty<System.String>(p => p.AccountDescription, "AccountDescription", string.Empty, RelationshipTypes.PrivateField);
        protected System.String _AccountDescription = string.Empty;
        public static readonly PropertyInfo<System.Single> BalanceProperty = RegisterProperty<System.Single>(p => p.Balance, "Balance", 0, RelationshipTypes.PrivateField);
        protected System.Single _Balance = 0;
        public System.Int32 AccountId
        {
            get
            {
                return GetProperty(AccountIdProperty, _AccountId);
            }
            set
            {
                if (_AccountId != value)
                {
                    SetProperty(AccountIdProperty, ref _AccountId, value);
                }
            }
        }
        public System.Int32 CategoryId
        {
            get
            {
                return GetProperty(CategoryIdProperty, _CategoryId);
            }
            set
            {
                if (_CategoryId != value)
                {
                    SetProperty(CategoryIdProperty, ref _CategoryId, value);
                }
            }
        }
        public System.Int32 AccountTypeId
        {
            get
            {
                return GetProperty(AccountTypeIdProperty, _AccountTypeId);
            }
            set
            {
                if (_AccountTypeId != value)
                {
                    SetProperty(AccountTypeIdProperty, ref _AccountTypeId, value);
                }
            }
        }
        public System.String AccountNumber
        {
            get
            {
                return GetProperty(AccountNumberProperty, _AccountNumber);
            }
            set
            {
                if (value == null) value = string.Empty;
                if (_AccountNumber != value)
                {
                    SetProperty(AccountNumberProperty, ref _AccountNumber, value);
                }
            }
        }
        public System.String AccountDescription
        {
            get
            {
                return GetProperty(AccountDescriptionProperty, _AccountDescription);
            }
            set
            {
                if (value == null) value = string.Empty;
                if (_AccountDescription != value)
                {
                    SetProperty(AccountDescriptionProperty, ref _AccountDescription, value);
                }
            }
        }
        public System.Single Balance
        {
            get
            {
                return GetProperty(BalanceProperty, _Balance);
            }
            set
            {
                if (!_Balance.Equals(value))
                {
                    SetProperty(BalanceProperty, ref _Balance, value);
                }
            }
        }

        #endregion


        #region Validation Rules

        public virtual List<BrokenRule> GetFullBrokenRules()
        {
            var rules = BrokenRulesCollection.ToList();
            return rules.Where(Lemon.CommonRules.CommonValidationRules.BrokenRulesToShow).ToList();
        }

        public override void CheckAllRules()
        {
            BusinessRules.CheckRules();
        }

        protected override void AddBusinessRules()
        {
            BusinessRules.AddRule(new Lemon.CommonRules.MaxLength(AccountNumberProperty, 100));
            BusinessRules.AddRule(new Lemon.CommonRules.MaxLength(AccountDescriptionProperty, 200));
            DoAddCustomBusinessRules();
        }

        protected virtual void DoAddCustomBusinessRules()
        {
        }

        #endregion

        #region Authorization Rules

        public virtual bool CanAddObject()
        {
            //The way we're using this, people can always add an object,
            //but they'll need the Edit rights to save it.
            return true;
        }

        public virtual bool CanGetObject()
        {
            return true;
        }

        public virtual bool CanDeleteObject()
        {
            bool canDelete = true;
            return canDelete;
        }

        public virtual bool CanEditObject()
        {
            bool canEdit = true;
            return canEdit;
        }

        #endregion

        #region Factory Methods
        /* Abstract method used by the base public class to implement some things. */
        protected override object GetIdValue()
        {
            return _AccountId;
        }

        public static void DeleteAccount(int id)
        {
            if (!Account.SecurityInstance.CanDeleteObject())
                throw new System.Security.SecurityException("User not authorized to remove Account") { Action = System.Security.Permissions.SecurityAction.Demand };
            DataPortal.Delete<Account>(new Criteria(id));
        }

        protected async override System.Threading.Tasks.Task<Account> SaveAsync(bool forceUpdate, object userState, bool isSync)
        {
            GenDoBeforeSave();

            try
            {
                var o = await base.SaveAsync(forceUpdate, userState, isSync);
                o.GenDoAfterSave();
                return o;
            }
            catch (Csla.Rules.ValidationException ex)
            {
                ex.Data["BusinessObject"] = this;
                throw;
            }
        }

        internal void GenDoBeforeSave()
        {
            DoBeforeSave();
        }

        internal void GenDoAfterSave()
        {
            DoAfterSave();
            InitializeOnClientSide();
        }

        internal virtual void DoBeforeSave()
        {
        }

        internal virtual void DoAfterSave()
        {
        }


        public virtual void CloneFrom(Account source)
        {
            this.CloneFrom(source, true);
        }

        protected virtual void CloneFrom(Account source, bool doCustomCloneFrom)
        {
            this._CategoryId = source._CategoryId;
            this._AccountTypeId = source._AccountTypeId;
            this._AccountNumber = source._AccountNumber;
            this._AccountDescription = source._AccountDescription;
            this._Balance = source._Balance;
            //Mark this as dirty so it'll be saved
            this.MarkDirty();

            //Copy the child objects

            if (doCustomCloneFrom)
                DoCustomCloneFrom(source);

            //Revalidate the rules
            BusinessRules.CheckRules();
        }

        ///<summary>
        ///Copies all data from the other object to this one.
        ///This bypasses the change notification, so you may wish
        ///to mark it old, dirty, and call OnPropertyChanged after.
        ///</sumary>
        protected void UpdateAllDataFrom(Account other)
        {
            this._CategoryId = other._CategoryId;
            this._AccountTypeId = other._AccountTypeId;
            this._AccountNumber = other._AccountNumber;
            this._AccountDescription = other._AccountDescription;
            this._Balance = other._Balance;

            //Copy the child objects
        }

        internal virtual void DoCustomCloneFrom(Account source)
        {

        }


        protected virtual void AddPropertyChangedHandler()
        {
            PropertyChangedEventHandler h = DoGetPropertyChangedEventHandler();
            if (h != null)
                PropertyChanged += h;
        }

        //Dummy implementation to override if you actually want to put something here
        protected virtual PropertyChangedEventHandler DoGetPropertyChangedEventHandler()
        {
            return null;
        }

        /* require use of factory methods */
        protected GenAccount()
        {
            SetStaticDefaultValues();
            AddPropertyChangedHandler();
        }

        //This is called when an object is deseralized.  In this case, it doesn't call the constructor,
        //so we need to attach listeners here.
        //The strategy for the ParentObject link in the container classes is from 
        //http://forums.lhotka.net/forums/thread/5627.aspx
        protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            AddPropertyChangedHandler();

        }

        protected virtual void SetStaticDefaultValues()
        {
            _AccountNumber = "";
            _AccountDescription = "";
            _Balance = 0;
        }

        public void MarkAsDirty()
        {
            base.MarkDirty();
        }

        private bool suppressUnknowPropertyChanged = false;

        public void MarkAsClean()
        {

            // suppress CSLA from calling all of the properties changed handler
            // which is a major performance hit
            suppressUnknowPropertyChanged = true;
            base.MarkClean();
            suppressUnknowPropertyChanged = false;
        }

        protected override void OnUnknownPropertyChanged()
        {
            if (!suppressUnknowPropertyChanged)
                base.OnUnknownPropertyChanged();
        }


        #endregion


        #region Data Access

        [Serializable()]
        protected class Criteria : Csla.CriteriaBase<Criteria>
        {
            public int Id { get; protected set; }
            public Criteria(int id)
            {
                Id = id;
            }

        }

        protected virtual SqlErrorParser DataPortal_GetSqlErrorParser()
        {
            return SqlErrorParser.PublicInstance;
        }

        protected virtual void DoBeforeSaveServer(SqlConnection cn)
        {
        }

        protected virtual void DoAfterSaveServer(SqlConnection cn)
        {
        }

        [RunLocal]
        protected override void DataPortal_Create()
        {
            DoSetDefaultValues();
            InitializeOnClientSide();
        }

        public virtual void DoSetDefaultValues()
        {
        }



        public virtual void RecursivelyRunSyncProperties()
        {
            PropertyChangedActionManager<Account>.Instance.RunSyncActions((Account)this);
        }

        protected virtual void DataPortal_Fetch(Criteria criteria)
        {
            try
            {
                DataPortal_FetchBody(criteria);
            }
            catch (Exception ex)
            {
                if (ex is ServerReturnedException || ex is WinterspringCustomException || ex is DataPortalException)
                {
                    //Note, just "throw;" will keep the
                    //stack trace of the original exception intact.
                    //"throw ex;" will reset it to here.
                    throw;
                }
                else if (ex is SqlException)
                {
                    throw DataPortal_GetSqlErrorParser().ParseSqlException(ex as SqlException, this);
                }
                else
                {
                    throw new ServerReturnedException(ex.Message, ex);
                }
            }
        }

        protected virtual void DataPortal_FetchBody(Criteria criteria)
        {
            using (WinterspringConnectionManager mgr = WinterspringConnectionManager.GetManager())
            {
                SqlConnection cn = mgr.Connection;
                DataPortal_FetchBody(criteria, cn);
            }
        }

        //Read the data for this object, not including child or later descendent objects
        //assuming the DataReader is already in the right place.
        protected virtual void ReadOwnData(NullableDataReader dr)
        {
            _AccountId = dr.GetInt32("AccountId");
            _AccountNumber = dr.GetString("AccountNumber");
            _AccountDescription = dr.GetString("AccountDescription");
            _AccountTypeId = dr.GetInt32("AccountTypeId");
            _CategoryId = dr.GetInt32("CategoryId");
            _Balance = dr.GetFloat("Balance");
        }

        protected virtual void DataPortal_FetchBody(Criteria criteria, SqlConnection cn)
        {
            using (SqlCommand cm = cn.CreateCommand())
            {
                cm.CommandType = CommandType.StoredProcedure;
                cm.CommandText = "BASE_Company_Retrieve";
                cm.Parameters.AddWithValue("@CompanyId", criteria.Id);

                using (NullableDataReader dr =
                  new NullableDataReader(cm.ExecuteReader()))
                {
                    dr.Read();
                    ReadOwnData(dr);



                }
            }
            RecursivelyRunSyncProperties();
        }


        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Insert()
        {
            try
            {
                DataPortal_InsertBody();
            }
            catch (Exception ex)
            {
                if (ex is ServerReturnedException || ex is WinterspringCustomException || ex is DataPortalException)
                {
                    //Note, just "throw;" will keep the
                    //stack trace of the original exception intact.
                    //"throw ex;" will reset it to here.
                    throw;
                }
                else if (ex is SqlException)
                {
                    throw DataPortal_GetSqlErrorParser().ParseSqlException(ex as SqlException, this);
                }
                else
                {
                    throw new ServerReturnedException(ex.Message, ex);
                }
            }
        }

        protected virtual void DataPortal_InsertBody()
        {
            using (WinterspringConnectionManager mgr = WinterspringConnectionManager.GetManager())
            {
                SqlConnection cn = mgr.Connection;
                DataPortal_InsertBody(cn);
            }
        }

        protected virtual void DataPortal_InsertBody(SqlConnection cn)
        {
            DoBeforeSaveServer(cn);

            using (SqlCommand cm = cn.CreateCommand())
            {
                cm.CommandType = CommandType.StoredProcedure;
                cm.CommandText = "BASE_Account_Create";
                cm.Parameters.AddWithValue("@CategoryId", _CategoryId);
                cm.Parameters.AddWithValue("@AccountTypeId", _AccountTypeId);
                cm.Parameters.AddWithValue("@AccountNumber", _AccountNumber);
                cm.Parameters.AddWithValue("@AccountDescription", _AccountDescription);
                cm.Parameters.AddWithValue("@Balance", _Balance);
                SqlParameter param =
                  new SqlParameter("@newId", SqlDbType.Int);
                param.Direction = ParameterDirection.Output;
                cm.Parameters.Add(param);
                //param = new SqlParameter("@newTimestamp", SqlDbType.Timestamp);
                //param.Direction = ParameterDirection.Output;
                //cm.Parameters.Add(param);

                cm.ExecuteNonQuery();

                _AccountId = (int)cm.Parameters["@newId"].Value;
                //_Timestamp = (byte[])cm.Parameters["@newTimestamp"].Value;
            }

            DoAfterSaveServer(cn);
        }


        //The transaction attribute has to be on the main
        //DataPortal method for it to be detected and handled
        //by CSLA.
        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Update()
        {
            try
            {
                DataPortal_UpdateBody();
            }
            catch (Exception ex)
            {
                if (ex is ServerReturnedException || ex is WinterspringCustomException || ex is DataPortalException)
                {
                    //Note, just "throw;" will keep the
                    //stack trace of the original exception intact.
                    //"throw ex;" will reset it to here.
                    throw;
                }
                else if (ex is SqlException)
                {
                    throw DataPortal_GetSqlErrorParser().ParseSqlException(ex as SqlException, this);
                }
                else
                {
                    throw new ServerReturnedException(ex.Message, ex);
                }
            }
        }

        protected virtual void DataPortal_UpdateBody()
        {
            using (WinterspringConnectionManager mgr = WinterspringConnectionManager.GetManager())
            {
                SqlConnection cn = mgr.Connection;
                DataPortal_UpdateBody(cn);
            }
        }

        protected virtual void DataPortal_UpdateBody(SqlConnection cn)
        {
            DoBeforeSaveServer(cn);

            if (base.IsDirty)
            {

                using (SqlCommand cm = cn.CreateCommand())
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandText = "BASE_Account_Update";
                    cm.Parameters.AddWithValue("@CategoryId", _CategoryId);
                    cm.Parameters.AddWithValue("@AccountTypeId", _AccountTypeId);
                    cm.Parameters.AddWithValue("@AccountNumber", _AccountNumber);
                    cm.Parameters.AddWithValue("@AccountDescription", _AccountDescription);
                    cm.Parameters.AddWithValue("@Balance", _Balance);
                    //SqlParameter timestampParam = new SqlParameter("@newTimestamp", SqlDbType.Timestamp);
                    //timestampParam.Direction = ParameterDirection.Output;
                    //cm.Parameters.Add(timestampParam);

                    cm.ExecuteNonQuery();

                    //_Timestamp = (byte[])cm.Parameters["@newTimestamp"].Value;
                }
            }

            DoAfterSaveServer(cn);
        }



        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_DeleteSelf()
        {
            DataPortal_Delete(new Criteria(_AccountId));
        }

        [Transactional(TransactionalTypes.TransactionScope)]
        protected virtual void DataPortal_Delete(Criteria criteria)
        {
            try
            {
                DataPortal_DeleteBody(criteria);
            }
            catch (Exception ex)
            {
                if (ex is ServerReturnedException || ex is WinterspringCustomException || ex is DataPortalException)
                {
                    //Note, just "throw;" will keep the
                    //stack trace of the original exception intact.
                    //"throw ex;" will reset it to here.
                    throw;
                }
                else if (ex is SqlException)
                {
                    throw DataPortal_GetSqlErrorParser().ParseSqlException(ex as SqlException, this);
                }
                else
                {
                    throw new ServerReturnedException(ex.Message, ex);
                }
            }
        }

        protected virtual void DataPortal_DeleteBody(Criteria criteria)
        {
            using (WinterspringConnectionManager mgr = WinterspringConnectionManager.GetManager())
            {
                SqlConnection cn = mgr.Connection;
                DataPortal_DeleteBody(criteria, cn);
            }
        }

        protected virtual void DataPortal_DeleteBody(Criteria criteria, SqlConnection cn)
        {
            using (SqlCommand cm = cn.CreateCommand())
            {
                cm.CommandType = CommandType.StoredProcedure;
                cm.CommandText = "BASE_Account_Delete";
                cm.Parameters.AddWithValue("@AccountId", criteria.Id);
                cm.ExecuteNonQuery();
            }
        }


        #endregion
    }
}
