using Csla;
using Csla.Core;
using Csla.Core.FieldManager;
using Csla.Rules;
using Csla.Security;
using Csla.Serialization.Mobile;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Reflection;
using Csla.Reflection;

namespace Winterspring.Lemon.Base
{
    [Serializable]
    public class WinterspringReadOnlyBase<T> : Csla.ReadOnlyBase<T> where T : WinterspringReadOnlyBase<T>
    {
        protected static PropertyInfo<P> RegisterProperty<P>(Expression<Func<T, object>> propertyLambdaExpression, string friendlyName, P defaultValue, RelationshipTypes relationship)
        {
            PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);

            return RegisterProperty(Csla.Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), reflectedPropertyInfo.Name, friendlyName, defaultValue, relationship));
        }
        
    }
}
