using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Winterspring.Lemon.Base
{
    public interface IDisplayObjectWithActive : IDisplayObject
    {
        bool IsActive { get; }
    }
    
    ///<exception>This class will not throw database exceptions</exception>
    public interface IDisplayObject : IObjectWithId
    {
        string Name { get; }        
    }

    ///<exception>This class will not throw database exceptions</exception>
    public interface IDisplayDataSource : ICollection
    {
        void AddUnknownItem(object id);
    }

    ///<exception>This class will not throw database exceptions</exception>
    public interface IDisplayDataSourceManager
    {
        void WeakSubscribe(Action action);

        void Unsubscribe(Action action);

        //Return the data source used for display.  This should not throw any exceptions.
        //i.e. if there's an exception, it should return empty or something.
        IDisplayDataSource DataSource { get; }
    }
}
