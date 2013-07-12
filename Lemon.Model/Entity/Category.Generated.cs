﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v6.5.3, CSLA Templates: v4.0.0.0, CSLA Framework: v4.5.x.
//     Changes to this file will be lost after each regeneration.
//     To extend the functionality of this class, please modify the partial class 'Category.cs'.
//
//     Template path: EditableRoot.Generated.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
using System;
using System.Threading.Tasks;

using Csla;
using Csla.Data;

using Csla.Rules;

namespace Lemon.Model
{
    [Serializable]
    public partial class Category : BusinessBase<Category>
    {
        #region Contructor(s)

        public Category()
        { /* Require use of factory methods */ }

        #endregion
 
        #region Business Rules

        /// <summary>
        /// Contains the CodeSmith generated validation rules.
        /// </summary>
        protected override void AddBusinessRules()
        {
            // Call the base class, if this call isn't made than any declared System.ComponentModel.DataAnnotations rules will not work.
            base.AddBusinessRules();

            if(AddBusinessValidationRules())
                return;

        }

        #endregion

        #region Properties

        private static readonly PropertyInfo<System.Int32> _categoryIdProperty = RegisterProperty<System.Int32>(p => p.CategoryId, "Category Id");
        [System.ComponentModel.DataObjectField(true, true)]
        public System.Int32 CategoryId
        {
            get { return GetProperty(_categoryIdProperty); }
            internal set{ SetProperty(_categoryIdProperty, value); }
        }

        private static readonly PropertyInfo<System.String> _nameProperty = RegisterProperty<System.String>(p => p.Name, "Name", (System.String)null);
        public System.String Name
        {
            get { return GetProperty(_nameProperty); }
            set{ SetProperty(_nameProperty, value); }
        }

        // OneToMany
        private static readonly PropertyInfo<AccountList> _accountsProperty = RegisterProperty<AccountList>(p => p.Accounts, Csla.RelationshipTypes.Child);
        public AccountList Accounts
        {
            get
            {
                bool cancel = false;
                OnChildLoading(_accountsProperty, ref cancel);
    
                if (!cancel)
                {
                    if(!FieldManager.FieldExists(_accountsProperty))
                    {
                        var criteria = new Lemon.Model.AccountCriteria { CategoryId = CategoryId };


                        if (!Lemon.Model.AccountList.Exists(criteria))
                            LoadProperty(_accountsProperty, Lemon.Model.AccountList.NewList());
                        else
                            LoadProperty(_accountsProperty, Lemon.Model.AccountList.GetByCategoryId(CategoryId));
                    }
                }

                return GetProperty(_accountsProperty);
            }
        }


        #endregion

        #region Synchronous Factory Methods 

        /// <summary>
        /// Creates a new object of type <see cref="Category"/>. 
        /// </summary>
        /// <returns>Returns a newly instantiated collection of type <see cref="Category"/>.</returns>   
        public static Category NewCategory()
        {
            return DataPortal.Create<Category>();
        }

        internal static Category GetCategory(SafeDataReader reader)
        {
            return DataPortal.FetchChild<Category>(reader);
        }

        /// <summary>
        /// Returns a <see cref="Category"/> object of the specified criteria. 
        /// </summary>
        /// <param name="categoryId">No additional detail available.</param>
        /// <returns>A <see cref="Category"/> object of the specified criteria.</returns>
        public static Category GetByCategoryId(System.Int32 categoryId)
        {
            var criteria = new CategoryCriteria {CategoryId = categoryId};
            
            
            return DataPortal.Fetch<Category>(criteria);
        }

        public static void DeleteCategory(System.Int32 categoryId)
        {
            var criteria = new CategoryCriteria {CategoryId = categoryId};
            
            
            DataPortal.Delete<Category>(criteria);
        }

        #endregion

        #region Asynchronous Factory Methods

        public static async Task<Category> NewCategoryAsync()
        {
            return await DataPortal.CreateAsync<Category>();
        }

        public static async Task<Category> GetByCategoryIdAsync(System.Int32 categoryId)
        {
            var criteria = new CategoryCriteria{ CategoryId = categoryId};
            
            
            return await DataPortal.FetchAsync<Category>(criteria);
        }

        public static async Task DeleteCategoryAsync(System.Int32 categoryId)
        {
            var criteria = new CategoryCriteria{CategoryId = categoryId};
            
            
            await DataPortal.DeleteAsync<Category>(criteria);
        }

        #endregion

        #region DataPortal partial methods

        /// <summary>
        /// CodeSmith generated stub method that is called when creating the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object creation should proceed.</param>
        partial void OnCreating(ref bool cancel);

        /// <summary>
        /// CodeSmith generated stub method that is called after the <see cref="Category"/> object has been created. 
        /// </summary>
        partial void OnCreated();

        /// <summary>
        /// CodeSmith generated stub method that is called when fetching the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="criteria"><see cref="CategoryCriteria"/> object containing the criteria of the object to fetch.</param>
        /// <param name="cancel">Value returned from the method indicating whether the object fetching should proceed.</param>
        partial void OnFetching(CategoryCriteria criteria, ref bool cancel);

        /// <summary>
        /// CodeSmith generated stub method that is called after the <see cref="Category"/> object has been fetched. 
        /// </summary>    
        partial void OnFetched();

        /// <summary>
        /// CodeSmith generated stub method that is called when mapping the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object mapping should proceed.</param>
        partial void OnMapping(ref bool cancel);
 
        /// <summary>
        /// CodeSmith generated stub method that is called when mapping the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancel">Value returned from the method indicating whether the object mapping should proceed.</param>
        partial void OnMapping(SafeDataReader reader, ref bool cancel);

        /// <summary>
        /// CodeSmith generated stub method that is called after the <see cref="Category"/> object has been mapped. 
        /// </summary>
        partial void OnMapped();

        /// <summary>
        /// CodeSmith generated stub method that is called when inserting the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object insertion should proceed.</param>
        partial void OnInserting(ref bool cancel);

        /// <summary>
        /// CodeSmith generated stub method that is called after the <see cref="Category"/> object has been inserted. 
        /// </summary>
        partial void OnInserted();

        /// <summary>
        /// CodeSmith generated stub method that is called when updating the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object creation should proceed.</param>
        partial void OnUpdating(ref bool cancel);

        /// <summary>
        /// CodeSmith generated stub method that is called after the <see cref="Category"/> object has been updated. 
        /// </summary>
        partial void OnUpdated();

        /// <summary>
        /// CodeSmith generated stub method that is called when self deleting the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="cancel">Value returned from the method indicating whether the object self deletion should proceed.</param>
        partial void OnSelfDeleting(ref bool cancel);

        /// <summary>
        /// CodeSmith generated stub method that is called after the <see cref="Category"/> object has been deleted. 
        /// </summary>
        partial void OnSelfDeleted();

        /// <summary>
        /// CodeSmith generated stub method that is called when deleting the <see cref="Category"/> object. 
        /// </summary>
        /// <param name="criteria"><see cref="CategoryCriteria"/> object containing the criteria of the object to delete.</param>
        /// <param name="cancel">Value returned from the method indicating whether the object deletion should proceed.</param>
        partial void OnDeleting(CategoryCriteria criteria, ref bool cancel);

        /// <summary>
        /// CodeSmith generated stub method that is called after the <see cref="Category"/> object with the specified criteria has been deleted. 
        /// </summary>
        partial void OnDeleted();
        partial void OnChildLoading(Csla.Core.IPropertyInfo childProperty, ref bool cancel);

        #endregion

        #region Exists Command

        /// <summary>
        /// Determines if a record exists in the Category table in the database for the specified criteria. 
        /// </summary>
        /// <param name="criteria">The criteria parameter is an <see cref="Category"/> object.</param>
        /// <returns>A boolean value of true is returned if a record is found.</returns>
        public static bool Exists(CategoryCriteria criteria)
        {
            return Lemon.Model.ExistsCommand.Execute(criteria);
        }

        /// <summary>
        /// Determines if a record exists in the Category table in the database for the specified criteria. 
        /// </summary>
        public static async Task<bool> ExistsAsync(CategoryCriteria criteria)
        {
            return await Lemon.Model.ExistsCommand.ExecuteAsync(criteria);
        }

        #endregion

    }
}