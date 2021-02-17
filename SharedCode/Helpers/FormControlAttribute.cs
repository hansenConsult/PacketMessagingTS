using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.Helpers
{
    // This is for deciding at runtime which form is supported by an assembly
    [AttributeUsage(AttributeTargets.Class)]
    public class FormControlAttribute : Attribute
    {
        public enum FormType
        {
            Undefined,
            None,
            CountyForm,
            CityForm,
            HospitalForm,
            TestForm,
        };

        // Form file name
        public string FormControlName { get; set; }    // 

        // Form type (County, Hospital etc.)
        public FormType FormControlType { get; set; }

        // Menu text
        public string FormControlMenuName { get; set; }    // 

        // Menu index
        public int FormControlMenuIndex { get; set; }
    }
}
