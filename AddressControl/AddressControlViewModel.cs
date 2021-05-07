using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AddressControl
{
    class AddressControlViewModel : ObservableObject
    {
        public static AddressControlViewModel Instance { get; } = new AddressControlViewModel();

        private string messageSubject;
        public string MessageSubject
        {
            get => messageSubject;
            set
            {
                string validatedSubject = ValidateSubject(value);
                SetProperty(ref messageSubject, validatedSubject);
            }
        }

        private string messageFrom;
        public string MessageFrom
        {
            get => messageFrom;
            set => SetProperty(ref messageFrom, value);
        }

        private string messageTo;
        public string MessageTo
        {
            get => messageTo;
            set => SetProperty(ref messageTo, value);
        }

        private bool isToIndividuals = true;
        public bool IsToIndividuals
        {
            get => isToIndividuals;
            set => SetProperty(ref isToIndividuals, value);
        }

        private static string ValidateSubject(string subject)
        {
            if (string.IsNullOrEmpty(subject))
                return string.Empty;

            try
            {
                return Regex.Replace(subject, @"[^\w\.@-\\%/\-\ ,()]", "~",
                                     RegexOptions.Singleline, TimeSpan.FromSeconds(1.0));
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return string.Empty;
            }
        }


    }
}
