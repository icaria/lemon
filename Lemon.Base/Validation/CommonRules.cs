using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Winterspring.Extensions;
using Csla.Rules;
using Csla;
using Lemon.Base;

namespace Lemon.Common
{
    public static class CommonValidationRules
    {
        //Lazy initialize some errors that shouldn't be shown in popup dialogs.
        private static HashSet<string> _rulesNotToShow;
        private static HashSet<string> RulesNotToShow
        {
            get
            {
                if (_rulesNotToShow == null)
                {
                    _rulesNotToShow = new HashSet<string>();
                    //_rulesNotToShow.Add("rule://ProdIdRequired/ProdId");  //These names are initialized automatically by CSLA.
                    //_rulesNotToShow.Add("rule://ItemRequired/ProdId");
                }
                return _rulesNotToShow;
            }
        }

        //This is called by codegen -- it's a predicate on which BrokenRules to show.
        public static bool BrokenRulesToShow(BrokenRule rule)
        {
            return !RulesNotToShow.Contains(rule.RuleName);
        }

    }
}
