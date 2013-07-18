/* AUTO-GENERATED FILE - Replace the hash code below with the word HAND to mark as hand-edited, or WIPE to overwrite it next time.*/
/* INTEGRITY CHECK:f3fabb5ad00c309309ab814d25e4beed*/
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using Csla.Rules;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Winterspring.Extensions;
using System.Threading.Tasks;
using Lemon.Base;
using Lemon.Common;

namespace Lemon.Model
{
    [Serializable()]
    public abstract class GenAccount : WinterspringBusinessBase<Account>, IWinterspringBusinessBase, IObjectWithTimestamp
    {
        [NonSerialized]
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Account));

        #region Property Definitions

        public static readonly PropertyInfo<System.Int32> AccountIdProperty = RegisterProperty<System.Int32>(p => p.AccountId, RelationshipTypes.PrivateField);
        protected System.Int32 _AccountId;
        public static readonly PropertyInfo<AccountType> AccountTypeProperty = RegisterProperty<AccountType>(p => p.AccountType, RelationshipTypes.PrivateField);
        protected AccountType _AccountType;
        public static readonly PropertyInfo<System.Int32> CategoryIdProperty = RegisterProperty<System.Int32>(p => p.CategoryId, RelationshipTypes.PrivateField);
        protected System.Int32 _CategoryId;
        public static readonly PropertyInfo<System.String> AccountNumberProperty = RegisterProperty<System.String>(p => p.AccountNumber, "AccountNumber", string.Empty, RelationshipTypes.PrivateField);
        protected System.String _AccountNumber = string.Empty;
        public static readonly PropertyInfo<System.String> AccountDescriptionProperty = RegisterProperty<System.String>(p => p.AccountDescription, "AccountDescription", string.Empty, RelationshipTypes.PrivateField);
        protected System.String _AccountDescription = string.Empty;
        public static readonly PropertyInfo<System.Decimal> BalanceProperty = RegisterProperty<System.Decimal>(p => p.Balance, "Balance", 0, RelationshipTypes.PrivateField);
        protected System.Decimal _Balance = 0;
        public static readonly PropertyInfo<byte[]> TimestampProperty = RegisterProperty<byte[]>(p => p.Timestamp, RelationshipTypes.PrivateField);
        protected byte[] _Timestamp = new byte[8];
        public static readonly PropertyInfo<System.DateTime> LastModDatetimeProperty = RegisterProperty<System.DateTime>(p => p.LastModDatetime, RelationshipTypes.PrivateField);
        protected System.DateTime _LastModDatetime;
        [System.ComponentModel.DataObjectField(true, true)]
        [Key]
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
        [Column("AccountTypeId")]
        public AccountType AccountType
        {
            get
            {
                return GetProperty(AccountTypeProperty, _AccountType);
            }
            set
            {
                if (_AccountType != value)
                {
                    SetProperty(AccountTypeProperty, ref _AccountType, value);
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
        public System.Decimal Balance
        {
            get
            {
                return GetProperty(BalanceProperty, _Balance);
            }
            set
            {
                if (_Balance != value)
                {
                    SetProperty(BalanceProperty, ref _Balance, value);
                }
            }
        }
        public byte[] Timestamp
        {
            get
            {
                return GetProperty(TimestampProperty, _Timestamp);
            }
            set
            {
                if (_Timestamp != value)
                {
                    SetProperty(TimestampProperty, ref _Timestamp, value);
                }
            }
        }
        public System.DateTime LastModDatetime
        {
            get
            {
                return GetProperty(LastModDatetimeProperty, _LastModDatetime);
            }
            set
            {
                if (_LastModDatetime != value)
                {
                    SetProperty(LastModDatetimeProperty, ref _LastModDatetime, value);
                }
            }
        }

        #endregion

        #region Validation Rules

        public virtual List<BrokenRule> GetFullBrokenRules()
        {
            var rules = BrokenRulesCollection.ToList();
            return rules.Where(Lemon.Common.CommonValidationRules.BrokenRulesToShow).ToList();
        }

        public override void CheckAllRules()
        {
            BusinessRules.CheckRules();
        }

        protected override void AddBusinessRules()
        {
            BusinessRules.AddRule(new Lemon.Common.Required(AccountIdProperty));
            BusinessRules.AddRule(new Lemon.Common.Required(AccountTypeProperty));
            BusinessRules.AddRule(new Lemon.Common.Required(CategoryIdProperty));
            BusinessRules.AddRule(new Lemon.Common.MaxLength(AccountNumberProperty, 10));
            BusinessRules.AddRule(new Lemon.Common.MaxLength(AccountDescriptionProperty, 100));
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


        internal void GenDoBeforeSave(GenCategory parent)
        {
            DoBeforeSave();
        }

        internal void GenDoAfterSave(GenCategory parent)
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
            this._AccountType = source._AccountType;
            this._CategoryId = source._CategoryId;
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

        private static int _lastId = -1;
        private static Object _lastLockObject = new Object();

        /// <summary>
        /// Set a unique primary key on this child object.  CSLA requires
        /// that all instance of a child object have a unique GetIdValue(),
        /// so we use a trick from 
        /// http://www.lhotka.net/Article.aspx?area=4&id=252e4f20-f202-4ba5-b6ff-4d629a2b7dcc
        /// http://forums.lhotka.net/forums/thread/6391.aspx
        /// to implement this.  This uses negative values for "new" primary keys.
        /// </summary>
        /// <returns></returns>
        private int SetNewId()
        {
            using (TimedLock.Lock(_lastLockObject))
            {
                int newId = _lastId--;
                if (newId == System.Int32.MinValue)
                {
                    newId = -1;
                    _lastId = -1;
                }
                _AccountId = newId;
                return _AccountId;
            }
        }

        //#2369 - Make sure that the listeners are attached even if this is reserialized
        //This is called when an object is deseralized.  In this case, it doesn't call the constructor,
        //so we need to attach listeners here.
        //The strategy for the ParentObject link in the container classes is from 
        //http://forums.lhotka.net/forums/thread/5627.aspx
        protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            AddPropertyChangedHandler();
        }

        /* require use of factory methods */
        protected GenAccount()
        {
            SetNewId();
            MarkAsChild();
            SetStaticDefaultValues();
            AddPropertyChangedHandler();
        }

        protected GenAccount(NullableDataReader dr)
        {
            MarkAsChild();
            SetStaticDefaultValues();
            Fetch(dr);
            AddPropertyChangedHandler();
        }

        protected virtual void SetStaticDefaultValues()
        {
        }

        internal void MarkAsDirty()
        {
            base.MarkDirty();
        }

        private bool suppressUnknowPropertyChanged = false;

        internal void MarkAsClean()
        {
            // suppress CSLA from calling all of the properties changed handler
            // which is a major performance hit
            suppressUnknowPropertyChanged = true;
            this.MarkClean();
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

        //Assume that this is called by someone that
        //will handle exceptions and transactions.
        protected virtual void Fetch(NullableDataReader dr)
        {
            _AccountId = dr.GetInt32("AccountId");
            _AccountType = (AccountType)dr.GetInt32("AccountTypeId");
            dr.GetBytes("Timestamp", 0, _Timestamp, 0, 8);
            _LastModDatetime = dr.GetDateTime("LastModDatetime");
            _CategoryId = dr.GetInt32("CategoryId");
            _AccountNumber = dr.GetString("AccountNumber");
            _AccountDescription = dr.GetString("AccountDescription");
            _Balance = dr.GetDecimal("Balance");
            _LastModDatetime = dr.GetDateTime("LastModDatetime");
            MarkOld();
        }

        //Assume an external caller will handle exceptions
        internal virtual void Insert(GenCategory parent, SqlConnection cn)
        {
            // if we're not dirty then don't update the database
            if (!this.IsDirty) return;

            using (SqlCommand cm = cn.CreateCommand())
            {
                cm.CommandType = CommandType.StoredProcedure;
                cm.CommandText = "Account_Create";
                cm.Parameters.AddWithValue("@AccountTypeId", (Int32)_AccountType);
                cm.Parameters.AddWithValue("@CategoryId", parent.CategoryId);
                _CategoryId = parent.CategoryId;
                cm.Parameters.AddWithValue("@AccountNumber", _AccountNumber);
                cm.Parameters.AddWithValue("@AccountDescription", _AccountDescription);
                cm.Parameters.AddWithValue("@Balance", _Balance);

                SqlParameter param =
                  new SqlParameter("@newId", SqlDbType.Int);
                param.Direction = ParameterDirection.Output;
                cm.Parameters.Add(param);
                param = new SqlParameter("@newTimestamp", SqlDbType.Timestamp);
                param.Direction = ParameterDirection.Output;
                cm.Parameters.Add(param);
                param = new SqlParameter("@newLastModDatetime", SqlDbType.DateTime);
                param.Direction = ParameterDirection.Output;
                cm.Parameters.Add(param);
                cm.ExecuteNonQuery();

                _AccountId = (int)cm.Parameters["@newId"].Value;
                _Timestamp = (byte[])cm.Parameters["@newTimestamp"].Value;
                _LastModDatetime = (System.DateTime)cm.Parameters["@newLastModDatetime"].Value;
            }

            MarkOld();
        }

        //Assume that a parent call will handle exceptions
        //and transactions
        internal virtual void Update(GenCategory parent, SqlConnection cn)
        {
            // if we're not dirty then don't update the database
            if (!this.IsDirty) return;

            using (SqlCommand cm = cn.CreateCommand())
            {
                cm.CommandType = CommandType.StoredProcedure;
                cm.CommandText = "Account_Update";
                cm.Parameters.AddWithValue("@AccountId", _AccountId);
                cm.Parameters.AddWithValue("@AccountTypeId", (Int32)_AccountType);
                cm.Parameters.AddWithValue("@CategoryId", parent.CategoryId);
                cm.Parameters.AddWithValue("@AccountNumber", _AccountNumber);
                cm.Parameters.AddWithValue("@AccountDescription", _AccountDescription);
                cm.Parameters.AddWithValue("@Balance", _Balance);
                cm.Parameters.AddWithValue("@Timestamp", _Timestamp);


                SqlParameter timestampParam = new SqlParameter("@newTimestamp", SqlDbType.Timestamp);
                timestampParam.Direction = ParameterDirection.Output;
                cm.Parameters.Add(timestampParam);
                SqlParameter lastModParam = new SqlParameter("@newLastModDatetime", SqlDbType.DateTime);
                lastModParam.Direction = ParameterDirection.Output;
                cm.Parameters.Add(lastModParam);

                cm.ExecuteNonQuery();

                _Timestamp = (byte[])cm.Parameters["@newTimestamp"].Value;
                _LastModDatetime = (System.DateTime)cm.Parameters["@newLastModDatetime"].Value;

            }

            MarkOld();
        }

        //Assume that somebody else will handle
        //exceptions and transactions resulting from this.
        internal virtual void DeleteSelf(GenCategory parent, SqlConnection cn)
        {
            // if we're not dirty then don't update the database
            if (!this.IsDirty) return;

            // if we're new then don't update the database
            if (this.IsNew) return;

            //Clear the child objects and remove them from the database

            using (SqlCommand cm = cn.CreateCommand())
            {
                cm.CommandType = CommandType.StoredProcedure;
                cm.CommandText = "Account_Delete";
                cm.Parameters.AddWithValue("@AccountId", _AccountId);
                cm.ExecuteNonQuery();
            }
            MarkNew();
        }

        #endregion
    }
}