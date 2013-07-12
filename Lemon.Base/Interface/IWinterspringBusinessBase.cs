using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Csla.Rules;

namespace Lemon.Base
{
    ///<exception>This class will not throw database exceptions</exception>
    public interface IWinterspringBusinessBase : IControlPropertyChangedActionMode
    {
        //Return a list of all the broken validation rules.
        //Implementations shouldn't throw exceptions.
        //List<BrokenRule> GetFullBrokenRules();

        //Return true if the current user is authorized to add an object.
        //Implementations shouldn't throw exceptions.
        bool CanAddObject();

        //Return true if the current user is authorized to retrieve an object.
        //Implementations shouldn't throw exceptions.
        bool CanGetObject();

        //Return true if the current user is authorized to delete an object.
        //Implementations shouldn't throw exceptions.
        bool CanDeleteObject();

        //Return true if the current user is authorized to edit an object.
        //Implementations shouldn't throw exceptions.
        bool CanEditObject();

        bool HasDataConflict { get; set; }

        //Returns a descriptive name for this type of object that's human readable.
        //Usually this can just be retrieved by reflection on the name.
        string GetHumanReadableName();
    }
}
