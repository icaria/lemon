using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Winterspring.Lemon.Base
{
    public class ChangingPropertyInfo
    {
        public MemberInfo Member { get; private set; }
        public string Name { get; private set; }
        public ChangingPropertyInfo(MemberInfo member) { this.Member = member; this.Name = member.Name; }
    }
}
