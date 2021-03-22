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
            None = 1,
            CountyForm = 2,
            CityForm = 3,
            HospitalForm = 4,
            TestForm = 5,
        };

        //public FormControlAttribute(string formControlName, string formControlMenuName, FormControlAttribute.FormType formType, int formControlMenuIndex)
        //{
        //    _FormControlName = formControlName;
        //    _FormControlMenuName = formControlMenuName;
        //    _FormControlMenuIndex = formControlMenuIndex;
        //    _FormControlType = formType;
        //}

        // Form file name
        protected string _FormControlName;
        public string FormControlName
        {
            get => _FormControlName;
            set => _FormControlName = value;
        }

        // Form file name
        //public string FormControlName { get; set; }    // 

        // Form type (County, Hospital etc.)
        protected FormControlAttribute.FormType _FormControlType;
        public FormControlAttribute.FormType FormControlType
        {
            get => _FormControlType;
            set => _FormControlType = value;
        }
        //public FormType FormControlType { get; set; }

        // Menu text
        protected string _FormControlMenuName;
        public string FormControlMenuName
        {
            get => _FormControlMenuName;
            set => _FormControlMenuName = value;
        }

        // Menu index
        //protected int _FormControlMenuIndex = -1;
        //public int FormControlMenuIndex
        //{
        //    get => _FormControlMenuIndex;
        //    set => _FormControlMenuIndex = value;
        //}
    }
}
