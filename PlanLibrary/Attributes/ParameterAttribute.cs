using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanLibrary.Attributes
{
    /// <summary>
    /// The list of parameter for a plan.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ParameterAttribute : Attribute
    {
        public List<string> Params
        {
            get;
            private set;
        }

        public ParameterAttribute(params string[] parameters)
        {
            Params = new List<string>(parameters);
        }

    }
}
