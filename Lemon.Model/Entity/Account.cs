﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated using CodeSmith: v6.5.3, CSLA Templates: v4.0.0.0, CSLA Framework: v4.5.x.
//       Changes to this template will not be lost.
//
//     Template: EditableChild.cst
//     Template website: http://code.google.com/p/codesmith/
// </autogenerated>
//------------------------------------------------------------------------------
using System;

using Csla;
using Csla.Security;

using Csla.Rules;

namespace Lemon.Model
{    
    /// <summary>
    /// The Account class is a CSLA editable child class.  See CSLA documentation for a more detailed description.
    /// </summary>
    public partial class Account
    {

        #region Business Rules
    
        /// <summary>
        /// All custom rules need to be placed in this method.
        /// </summary>
        /// <returns>Return true to override the generated rules; If false generated rules will be run.</returns>
        protected bool AddBusinessValidationRules()
        {
            // TODO: add validation rules
            //BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(MyProperty));

            return false;
        }

        #endregion

        #region Authorization Rules

        /// <summary>
        /// Allows the specification of CSLA based authorization rules.  Specifies what roles can 
        /// perform which operations for a given business object
        /// </summary>
        public static void AddObjectAuthorizationRules()
        {
            //Csla.Rules.BusinessRules.AddRule(typeof(Account), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, "SomeRole"));
            //Csla.Rules.BusinessRules.AddRule(typeof(Account), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, "SomeRole"));
            //Csla.Rules.BusinessRules.AddRule(typeof(Account), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, "SomeRole", "SomeAdminRole"));
        }
        #endregion
    }
}