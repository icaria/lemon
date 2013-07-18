using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using System.Collections.Generic;
using Lemon.Base;
using Winterspring.Extensions;

namespace Lemon.Model
{
    [Serializable()]
    public class GenAccountList : WinterspringBusinessListBase<AccountList, Account>
    {

        #region Business Methods

        public IList<Account> Deleted { get { return DeletedList; } }

        public virtual int? GetLineNumber(Account item)
        {
            for (int i = 0; i < this.Count; ++i)
            {
                if (item == this[i])
                    return i + 1;
            }
            return null;
        }

        public virtual void ReenableListChangedEvents()
        {
            this.RaiseListChangedEvents = true;
            ResetBindings();
        }

        [NotUndoable()]
        [NonSerialized()]
        private Category _ParentCategory;

        public Category ParentCategory
        {
            get { return _ParentCategory; }
            internal set { _ParentCategory = value; }
        }

        protected override object ListParent { get { return ParentCategory; } }

        protected override bool HasDoPropertyChanged { get { return true; } }

        public virtual void AddNewAccount()
        {
            Account child =
              Account.NewAccount();
            this.Add(child);
            child.DoSetDefaultValues();
        }


        public new Account AddNew()
        {
            Account child =
              Account.NewAccount();
            this.Add(child);
            child.DoSetDefaultValues();
            return child;
        }

        /// <summary>
        /// "new row" implementation.
        /// http://forums.lhotka.net/forums/thread/1890.aspx
        /// http://forums.lhotka.net/forums/thread/6210.aspx
        /// </summary>
        /// <returns></returns>
        protected override object AddNewCore()
        {
            Account o = Account.NewAccount();
            this.Add(o);
            o.DoSetDefaultValues();
            return o;
        }

        #endregion

        #region Factory Methods

        protected GenAccountList()
        {
            this.AllowNew = true;
            MarkAsChild();
        }

        internal virtual void GenDoBeforeSave(GenCategory parent)
        {
            //Do anything the list object has to do before saving
            DoBeforeSave(parent);

            //Go through each child object and give them a chance
            //to do anything before save
            foreach (Account child in this)
            {
                child.GenDoBeforeSave(parent);
            }
        }

        //To be overridden by subclasses if necessary
        internal virtual void DoBeforeSave(GenCategory parent)
        {
        }

        internal virtual void GenDoAfterSave(GenCategory parent)
        {
            //Do anything the list object has to do after saving
            DoAfterSave(parent);

            //Go through each child object and give them a chance
            //to do anything after the save
            foreach (Account child in this)
            {
                child.GenDoAfterSave(parent);
            }
        }

        //To be overridden by subclasses if necessary
        internal virtual void DoAfterSave(GenCategory parent)
        {
        }

        public virtual void CloneFrom(AccountList source)
        {
            this.RaiseListChangedEvents = false;
            //Remove all items
            List<Account> tempList = new List<Account>(this);
            foreach (Account p in tempList)
            {
                this.Remove(p);
            }

            //Add clones of the others
            source.ForEach(x => AddNew().CloneFrom(x));

            this.ReenableListChangedEvents();
        }


        #endregion

        #region Data Access

        internal virtual void Fetch(NullableDataReader dr)
        {
            this.RaiseListChangedEvents = false;
            this.Clear();
            while (dr.Read())
            {
                this.Add(Account.GetAccount(dr));
            }
            this.RaiseListChangedEvents = true;  //On the server side, don't do a reset when turning list changed events back on
        }

        internal virtual void Update(GenCategory parent, SqlConnection cn)
        {
            RaiseListChangedEvents = false;
            // update (thus deleting) any deleted child objects
            foreach (Account item in DeletedList)
                item.DeleteSelf(parent, cn);
            // now that they are deleted, remove them from memory too
            DeletedList.Clear();

            // add/update any current child objects
            foreach (Account item in this)
            {
                if (item.IsNew)
                    item.Insert(parent, cn);
                else
                    item.Update(parent, cn);
            }
            this.RaiseListChangedEvents = true;  //On the server side, don't do a reset when turning list changed events back on
        }

        #endregion

        //Use this carefully, it's probably not a very good idea to use this
        //method when the list is going to continue to be used.
        public virtual List<Account> ExportToList()
        {
            List<Account> list = new List<Account>();
            foreach (Account o in this)
            {
                list.Add(o);
            }
            return list;
        }

    }
}
