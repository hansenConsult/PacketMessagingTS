using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using SharedCode;
using SharedCode.Helpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using ToggleButtonGroupControl;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp.Helpers;

namespace FormControlBaseClass
{
    //   // This is for deciding at runtime which form is supported by an assembly
    //   [AttributeUsage(AttributeTargets.Class)]
    //public class FormControlAttribute : Attribute
    //{
    //	public enum FormType
    //	{
    //		None,
    //		CountyForm,
    //		CityForm,
    //		HospitalForm,
    //           TestForm,
    //	};

    //       // Form file name
    //	public string FormControlName { get; set; }    // 

    //       // Form type (County, Hospital etc.)
    //	public FormType FormControlType { get; set; }

    //       // Menu text
    //	public string FormControlMenuName { get; set; }    // 
    //}

    public sealed class FormEventArgs : EventArgs
	{
		//public FormEventArgs() { }

		//public FormEventArgs(string tacticalCallsign)
		//{
		//	TacticalCallsign = tacticalCallsign;
		//}

		//public string TacticalCallsign
		//{ get; set; }

		public string SubjectLine
		{ get; set; }
	}

    public abstract partial class FormControlBase : FormControlBasics
    {
        public event EventHandler<FormEventArgs> EventSubjectChanged;
        //public event PropertyChangedEventHandler PropertyChanged;

        protected List<string> outpostData;
        protected List<string> _ICSPositionFiltered = new List<string>();

        protected string[] ICSPosition = new string[] {
                "Incident Commander",
                "Operations",
                "Planning",
                "Logistics",
                "Finance",
                "Public Info. Officer",
                "Liaison Officer",
                "Safety Officer"
        };

        readonly protected List<ComboBoxPackItItem> Hospitals = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem("El Camino Hospital Los Gatos"),
            new ComboBoxPackItItem("El Camino Hospital Mountain View"),
            new ComboBoxPackItItem("Good Samaritan Hospital"),
            new ComboBoxPackItItem("Kaiser Gan Jose Medical Center"),
            new ComboBoxPackItItem("Kaiser Santa Clara Hospital"),
            new ComboBoxPackItItem("Lucile Packard Children's Hospital"),
            new ComboBoxPackItItem("O'Connor Hospital"),
            new ComboBoxPackItItem("Palo Alto Veterans Hospital"),
            new ComboBoxPackItItem("Regional San Jose Medical Center"),
            new ComboBoxPackItItem("Saint Loise Regional Hospital"),
            new ComboBoxPackItItem("Stanford Hospital"),
            new ComboBoxPackItItem("Stanford School of Medicine"),
            new ComboBoxPackItItem("Valley Medical Center"),
        };

        protected List<ComboBoxPackItItem> Priority = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem("Now", "Now", 0),
            new ComboBoxPackItItem("High (0-4 hrs.)", "High", 1),
            new ComboBoxPackItItem("Medium (5-12 hrs.)", "Medium", 2),
            new ComboBoxPackItItem("Low (12+ hrs.)", "Low", 3),
        };

        private static Dictionary<string, object> _properties = new Dictionary<string, object>();
        static Dictionary<string, bool> _propertyFirstTime = new Dictionary<string, bool>();

        public static string DrillTraffic = "\r\nDrill Traffic";

        protected PrintHelper _printHelper;


        protected FormControlBase()
        {
        }

        protected T GetProperty<T>(ref T backingStore, [CallerMemberName]string propertyName = "")
        {
            if (_properties != null && _properties.ContainsKey(propertyName))
            {
                // Retrieve value from dictionary
                object o = _properties[propertyName];
                backingStore = (T)o;
                return (T)o;
            }
            else
                return backingStore;
        }

        protected bool SetProperty<T>(ref T backingStore, T value, bool persist = false, bool forceUpdate = false,
                    [CallerMemberName]string propertyName = "", Action onChanged = null)
        {
            bool firstTime = false;
            if (_propertyFirstTime.ContainsKey(propertyName))
            {
                firstTime = _propertyFirstTime[propertyName];
            }
            else
            {
                firstTime = true;
            }
            _propertyFirstTime[propertyName] = false;
            //Do not update displayed value if not changed or not first time or not forced
            if (Equals(backingStore, value) && !firstTime && !forceUpdate)
            {
                return false;
            }
            //_propertyFirstTime[propertyName] = false;

            if (persist)
            {
                // store value
                _properties[propertyName] = value;
            }

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        //protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        //{
        //    add
        //    {
        //        throw new NotImplementedException();
        //    }

        //    remove
        //    {
        //        throw new NotImplementedException();
        //    }
        //}


        //private void NotifyPropertyChanged( String propertyName = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        //public void InitializeControls()
        //{
        //    foreach (FormControl formControl in _formControlsList)
        //    {
        //        Control control = formControl.InputControl;

        //        if (control is TextBox)
        //        {
        //            //if (IsFieldRequired(control))
        //            //{
        //            //    control.BorderBrush = formControl.RequiredBorderBrush;
        //            //}
        //            //else
        //            //{
        //            //    control.BorderBrush = formControl.BaseBorderColor;
        //            //}
        //        }
        //        //else if (control is AutoSuggestBox)
        //        //{
        //        //    control.BorderBrush = formControl.BaseBorderColor;
        //        //}
        //        //else if (control is ComboBox)
        //        //{
        //        //    if (IsFieldRequired(control))
        //        //    {
        //        //        control.BorderBrush = formControl.RequiredBorderBrush;
        //        //    }
        //        //    else
        //        //    {
        //        //        control.BorderBrush = formControl.BaseBorderColor;
        //        //    }
        //        //}
        //        else if (control is ToggleButtonGroup toggleButtonGroup)
        //        {
        //            toggleButtonGroup.Initialize(_radioButtonsList, control.Name);
        //        }
        //        //else if (control is CheckBox checkBox)
        //        //{
        //        //    checkBox.IsChecked = false;
        //        //}
        //    }
        //}

        public void InitializeToggleButtonGroups()
        {
            foreach (FormControl formControl in _formControlsList)
            {
                if (formControl.InputControl is ToggleButtonGroup toggleButtonGroup)
                {
                    toggleButtonGroup.Initialize(_radioButtonsList, formControl.InputControl.Name);
                }
            }
        }

        public virtual void UpdateFormFieldsRequiredColors(bool newForm = true)
        {
            foreach (FormControl formControl in _formControlsList)
            {
                Control control = formControl.InputControl;

                if (control is TextBox textBox)
                {
                    if (string.IsNullOrEmpty(textBox.Text) && IsFieldRequired(control) && newForm)
                    {
                        control.BorderBrush = formControl.RequiredBorderBrush;
                        control.BorderThickness = new Thickness(2);
                    }
                    else
                    {
                        control.BorderBrush = formControl.BaseBorderColor;
                        control.BorderThickness = new Thickness(1);
                    }
                }
                else if (control is AutoSuggestBox autoSuggestBox)
                {
                    if (string.IsNullOrEmpty(autoSuggestBox.Text) && IsFieldRequired(control) && newForm)
                    {
                        control.BorderBrush = formControl.RequiredBorderBrush;
                        control.BorderThickness = new Thickness(2);
                    }
                    else
                    {
                        control.BorderBrush = formControl.BaseBorderColor;
                        control.BorderThickness = new Thickness(1);
                    }
                }
                else if (control is ComboBox comboBox)
                {
                    if (comboBox.SelectedIndex < 0 && IsFieldRequired(control) && newForm)
                    {
                        control.BorderBrush = formControl.RequiredBorderBrush;
                        control.BorderThickness = new Thickness(2);
                    }
                    else
                    {
                        control.BorderBrush = formControl.BaseBorderColor;
                        control.BorderThickness = new Thickness(1);
                    }
                }
                else if (control is ToggleButtonGroup toggleButtonGroup)
                {
                    if (IsFieldRequired(control) && string.IsNullOrEmpty(toggleButtonGroup.GetRadioButtonCheckedState()) && newForm)
                    {
                        toggleButtonGroup.ToggleButtonGroupBrush = formControl.RequiredBorderBrush;
                    }
                    else
                    {
                        toggleButtonGroup.ToggleButtonGroupBrush = new SolidColorBrush(Colors.Black);
                    }

                }
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty OperatorCallsignProperty =
        //	DependencyProperty.Register("OperatorCallsign", typeof(string), typeof(FormControlBase), null);

        public PacketMessage FormPacketMessage
        { get; set; }

        public virtual string TacticalCallsign
        { get; set; }

        public virtual string OperatorCallsign
        { get; set; }

        //public List<FormControl> FormControlsList
        //{ get => _formControlsList; }

        //public string ValidationResultMessage
        //{
        //    get => _validationResultMessage;
        //    set => _validationResultMessage = value;
        //}

        private string _pif = "2.1";
        public virtual string PIF
        {
            get => _pif;
            set => _pif = value;
        }

        public virtual string PIFString
        {
            get => $"PIF: {PIF}";
        }

        public virtual string SenderMsgNo
        { get; set; }

        public virtual string MessageNo
        { get; set; }

        public virtual string ReceiverMsgNo
        { get; set; }

        public virtual string DestinationMsgNo
        { get; set; }

        public virtual string OriginMsgNo
        { get; set; }

        //public static string DefaultMessageTo
        //{ get; set; }

        protected string _msgDate;
        public virtual string MsgDate
        { get; set; }

        protected static string TimeCheck(string time)
        {
            try
            {
                var filteredTime = time.Split(new char[] { ':' });
                if (filteredTime.Length == 1 && time.Length > 2)
                {
                    string min = time.Substring(time.Length - 2);
                    if (min.Length > 2 || Convert.ToInt32(min) > 59)
                        return "";
                    string hour = time.Substring(0, time.Length - 2);
                    if (hour.Length > 2 || Convert.ToInt32(hour) > 23)
                        return "";
                    return $"{hour}:{min}";
                }
                else if (time.Length > 2 && time.Length < 6)
                {
                    return time;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        protected string _msgTime; 
        public virtual string MsgTime
        {
            get => _msgTime;
            set
            {
                string time = TimeCheck(value);
                Set(ref _msgTime, time);
            }
        }

        public virtual string Action
        { get; set; }

        public virtual string Reply
        { get; set; }

        public virtual string OperatorName
        { get; set; }

        public virtual string Severity
        { get; set; }

        public virtual string HandlingOrder
		{ get; set; }

        public virtual string ReceivedOrSent
        { get; set; }

        public virtual string HowReceivedSent
        { get; set; }

        private string _IncidentName;
        public virtual string IncidentName      // Required for setting Practice
        {
            get => _IncidentName;
            set => Set(ref _IncidentName, value);
        }

        public virtual string FacilityName      // Required for setting Practice
        { get; set; }

        public virtual string ReportType
        { get; set; }

        // Implemented this way to facilitate synchronizing two name fields and required for setting Practice 
        private string _shelterName;
        public virtual string ShelterName
        {
            get => _shelterName;
            set => Set(ref _shelterName, value);
        }

        public virtual string Subject       // Required for setting Practice
        { get; set; }

        private DateTime? messageReceivedTime = null;
        public virtual DateTime? MessageReceivedTime
        {
            get => messageReceivedTime;
            set => Set(ref messageReceivedTime, value);
        }

        private DateTime? messageSentTime = null;
        public virtual DateTime? MessageSentTime
        {
            get => messageSentTime;
            set => Set(ref messageSentTime, value);
        }

        public virtual FormControlAttribute.FormType FormControlType
        { get; }

        public abstract FormProviders FormProvider
        { get; }

        public abstract string PacFormName
		{ get; }

        public abstract string PacFormType
        { get; }

        public abstract string CreateSubject();

        public abstract void AppendDrillTraffic();

        public abstract string CreateOutpostData(ref PacketMessage packetMessage);

		protected virtual void CreateOutpostDataFromFormFields(ref PacketMessage packetMessage, ref List<string> outpostData)
        {
            if (packetMessage.FormFieldArray is null)
            {
                // This may happen if called from view Outpost DataDefault
                packetMessage.FormFieldArray = CreateFormFieldsInXML();
                FillFormFromFormFields(packetMessage.FormFieldArray);
            }
            foreach (FormField formField in packetMessage.FormFieldArray)
            {
                if (string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                string data = CreateOutpostDataString(formField, packetMessage.FormProvider);
                if (string.IsNullOrEmpty(data))
                {
                    continue;
                }
                outpostData.Add(data);
            }
            switch (packetMessage.FormProvider)
            {
                case FormProviders.PacForm:
                    outpostData.Add("#EOF");
                    break;
                case FormProviders.PacItForm:
                    outpostData.Add("!/ADDON!");
                    break;
            }
        }

        protected Control GetControlFromTagIndex(string id)
        {
            foreach (FormControl  formControl in _formControlsList)
            {
                Control control = formControl.InputControl;
                string tagIndex = GetTagIndex(control);
                if (id == tagIndex)
                {
                    return control;
                }
            }
            return null;
        }

        protected virtual string ConvertComboBoxFromOutpost(string id, ref string[] msgLines)
        {
            string comboBoxData = GetOutpostValue(id, ref msgLines);
            var comboBoxDataSet = comboBoxData?.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
            
            return comboBoxDataSet?[0];
        }

        protected virtual FormField[] ConvertFromOutpostPackItForm(FormField[] formFields, ref string[] msgLines)
        {
            //string senderMsgNo = "";
            //if (!string.IsNullOrEmpty(GetOutpostValue("MsgNo", ref msgLines)))
            //{
            //    senderMsgNo = GetOutpostValue("1", ref msgLines);
            //}
            //else if (!string.IsNullOrEmpty(GetOutpostValue("3", ref msgLines)))
            //{
            //    senderMsgNo = GetOutpostValue("3", ref msgLines);
            //}

            foreach (FormField formField in formFields)
            {
                (string id, Control control) = GetTagIndex(formField);
                formField.FormIndex = id;

                if (control is ToggleButtonGroup)
                {
                    string outpostValue = GetOutpostValue(id, ref msgLines);
                    if (!string.IsNullOrEmpty(outpostValue))
                    {
                        foreach (RadioButton radioButton in ((ToggleButtonGroup)control).RadioButtonGroup)
                        {
                            if (outpostValue == radioButton.Tag as string)
                            {
                                formField.ControlContent = radioButton.Name;
                            }
                            //if ((radioButton.Content as string).Contains(outpostValue))
                            //{
                            //    formField.ControlContent = radioButton.Name;
                            //}
                            //else if (radioButton.Name == outpostValue)
                            //{
                            //    formField.ControlContent = radioButton.Name;
                            //}
                        }
                    }
                }
                else if (control is CheckBox)
                {
                    formField.ControlContent = (GetOutpostValue(id, ref msgLines) == "checked" ? "True" : "False");
                }
                else if (control is ComboBox comboBox)
                {
                    formField.ControlContent = ConvertComboBoxFromOutpost(id, ref msgLines);
                }
                else if (control is TextBox || control is AutoSuggestBox)
                {
                    //if (control.Name == "senderMsgNo")
                    //{
                    //    formField.ControlContent = senderMsgNo;
                    //}
                    //else
                    {
                        formField.ControlContent = GetOutpostValue(id, ref msgLines);
                    }
                }
            }
            return formFields;
        }

        public virtual FormField[] ConvertFromOutpost(string msgNumber, ref string[] msgLines, FormProviders formProvider)
        {
            FormField[] formFields = CreateEmptyFormFieldsArray();

            // Populate Sender Message Number from received message number
            // Sender message number can be either in 1 or 3 in PACFORM
            if (formProvider == FormProviders.PacItForm)
            {
                formFields = ConvertFromOutpostPackItForm(formFields, ref msgLines);
            }
            else
            {
                string senderMsgNo = "";
                    
                //if (!string.IsNullOrEmpty(GetOutpostValue("1", ref msgLines)))
                //{
                //    senderMsgNo = GetOutpostValue("1", ref msgLines);
                //}
                //else if (!string.IsNullOrEmpty(GetOutpostValue("3", ref msgLines)))
                //{
                //    senderMsgNo = GetOutpostValue("3", ref msgLines);
                //}
                foreach (FormField formField in formFields)
                {
                    (string id, Control control) = GetTagIndex(formField);
                    formField.FormIndex = id;    
                    if (control is ToggleButtonGroup)
                    {
                        foreach (RadioButton radioButton in ((ToggleButtonGroup)control).RadioButtonGroup)
                        {
                            string radioButtonIndex = GetTagIndex(radioButton);
                            if ((GetOutpostValue(radioButtonIndex, ref msgLines)?.ToLower()) == "true")
                            {
                                formField.ControlContent = radioButton.Name;
                            }
                        }
                    }
                    else if (control is CheckBox)
                    {
                        formField.ControlContent = (GetOutpostValue(id, ref msgLines) == "true" ? "True" : "False");
                    }
                    else if (control is ComboBox comboBox)
                    {
                        formField.ControlContent = ConvertComboBoxFromOutpost(id, ref msgLines);
                    }
                    else if (control is TextBox || control is AutoSuggestBox)
                    {
                        if (control.Name == "senderMsgNo")
                        {
                            formField.ControlContent = senderMsgNo;
                        }
                        else
                        {
                            formField.ControlContent = GetOutpostValue(id, ref msgLines);
                            // Filter operator date and time
                            if (!string.IsNullOrEmpty(formField.ControlContent))
                            {
                                int index = formField.ControlContent.IndexOf("{odate");
                                if (index > 0)
                                {
                                    formField.ControlContent = formField.ControlContent.Substring(0, index);
                                }
                                index = formField.ControlContent.IndexOf("{otime");
                                if (index > 0)
                                {
                                    formField.ControlContent = formField.ControlContent.Substring(0, index);
                                }
                            }
                        }
                    }
                }
            }
            return formFields;
        }

        public (string id, Control control) GetTagIndex(FormField formField)
        {
            if (formField is null)
                return ("", null);

            Control control = null;
            try
            {
                // The control may not have a name, but the tag index is defined
                if (!string.IsNullOrEmpty(formField.FormIndex))
                {
                    // Index is known, no name
                    foreach (FormControl frmControl in _formControlsList)
                    {
                        string tag = frmControl.InputControl.Tag as string;
                        if (!string.IsNullOrEmpty(tag))
                        {
                            string[] tags = tag.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if (tags[0] == formField.FormIndex)
                            {
                                return (tags[0], frmControl.InputControl);
                            }
                        }
                    }
                }
                else
                {
                    // Name is known
                    FormControl formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);
                    control = formControl?.InputControl;

                    string tag = (string)control.Tag;
                    if (!string.IsNullOrEmpty(tag))
                    {
                        string[] tags = tag.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (!(tags[0].Contains("required") || tags[0].Contains("conditionallyrequired")))
                        {
                            return (tags[0], control);
                        }
                    }
                }
            }
            catch
            {
            }
            return ("", control);
        }

        public static string GetTagIndex(Control control)
        {
            try
            {
                string tag = (string)control.Tag;
                if (!string.IsNullOrEmpty(tag))
                {
                    string[] tags = tag.Split(new char[] { ',' });
                    if (!tags[0].Contains("required"))
                    {
                        return tags[0];
                    }
                }
            }
            catch
            {
            }
            return "";
        }

        //public (string id, Control control) GetTagIndex(FormField formField, FormProviders formProvider)
        //{
        //    // Should not be used anymore!!

        //    if (formField is null)
        //        return ("", null);

        //    Control control = null;
        //    try
        //    {
        //        //control = formField.InputControl;
        //        FormControl formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);
        //        control = formControl?.InputControl;

        //        string tag = (string)control.Tag;
        //        if (!string.IsNullOrEmpty(tag))
        //        {
        //            string[] tags = tag.Split(new char[] { ',' });
        //            if (!(tags[0].Contains("required") || tags[0].Contains("conditionallyrequired")))
        //            {
        //                return (tags[0], control);
        //            }
        //        }
        //    }
        //    catch
        //    {                
        //    }
        //    return ("", control);
        //}

        //public static string GetTagIndex(Control control, FormProviders formProvider)
        //{
        //    try
        //    {
        //        string tag = (string)control.Tag;
        //        if (!string.IsNullOrEmpty(tag))
        //        {
        //            string[] tags = tag.Split(new char[] { ',' });
        //            if (!tags[0].Contains("required"))
        //            {
        //                string[] formProviderTags = tags[0].Split(new char[] { '|' });
        //                if (formProviderTags.Length > 1)
        //                {
        //                    return formProviderTags[(int)formProvider];
        //                }
        //                else
        //                {
        //                    return tags[0];
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    return "";
        //}

        public string GetTagErrorMessage(FormField formField)
        {
            //Control control = formField.InputControl;
            string name = _formControlsList[1].InputControl.Name;
            FormControl formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);
            Control control = formControl.InputControl;
            string tag = control.Tag as string;
            if (string.IsNullOrEmpty(tag))
                return "";

            string[] tags = tag.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (tags.Length == 2 && tags[0].Contains("required"))
            {
                return tags[1];
            }
            else if (tags.Length == 3)
            {
                return tags[2];
            }
            else
            {
                return "";
            }
        }

        protected virtual string CreateComboBoxOutpostDataString(FormField formField, string id)
        {
            switch (FormProvider)
            {
                case FormProviders.PacForm:
                    string[] data = formField.ControlContent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (data.Length == 2)
                    {
                        if (data[1] == (-1).ToString() || string.IsNullOrEmpty(data[1]))
                        {
                            return $"{id}: [ }}0]";
                        }
                        else
                        {
                            return $"{id}: [{data[0]}}}{data[1]}]";
                        }
                    }
                    else if (data[0] == "-1" || string.IsNullOrEmpty(data[0]))
                    {
                        return $"{id}: [ }}0]";
                    }
                    break;
                case FormProviders.PacItForm:
                    if (!string.IsNullOrEmpty(formField.ControlComboxContent?.Data))
                    {
                        return $"{id}: [{formField.ControlComboxContent.Data}]";
                    }
                    else
                    {
                        string[] selection = formField.ControlContent.Split(new char[] { ' ' });
                        return $"{id}: [{selection[0]}]";
                    }
            }
            return "";
        }

        public string CreateOutpostDataString(FormField formField, FormProviders formProvider)
        {
            (string id, Control control) = GetTagIndex(formField);
            if (string.IsNullOrEmpty(id))
                return "";

            if (control is TextBox)
            {
                if (formProvider == FormProviders.PacForm)
                {
                    if (((TextBox)control).AcceptsReturn)
                    {
                        return $"{id}: [\\n{formField.ControlContent}]";
                    }
                    else
                    {
                        if (formField.ControlName == "operatorDate")
                        {
                            return $"{id}: [{formField.ControlContent}" + "{odate]";
                        }
                        else if (formField.ControlName == "operatorTime")
                        {
                            return $"{id}: [{formField.ControlContent}" + "{otime]";
                        }
                        else
                        {
                            return $"{id}: [{formField.ControlContent}]";
                        }
                    }
                }
                else if (formProvider == FormProviders.PacItForm)
                {
                    return $"{id}: [{formField.ControlContent}]";
                }
            }
            else if (control is AutoSuggestBox)
            {
                return $"{id}: [{formField.ControlContent}]";
            }
            else if (control is RadioButton)
            {
                if (formProvider == FormProviders.PacForm)
                {

                    if (formField.ControlContent == "True")
                    {
                        return $"{id}: [true]";
                    }
                    else
                    {
                        return "";
                    }
                }
                //else if (formProvider == FormProviders.PacItForm)
                //{
                //    if (formField.ControlContent == "True")
                //    {
                //        return $"{id}: [checked]";
                //    }
                //    else
                //    {
                //        return "";
                //    }
                //}
            }
            else if (control is CheckBox)
            {
                if (formField.ControlContent == "True")
                {
                    if (formProvider == FormProviders.PacForm)
                    {
                        return $"{id}: [true]";
                    }
                    else if (formProvider == FormProviders.PacItForm)
                    {
                        return $"{id}: [checked]";
                    }
                }
                else
                {
                    return "";
                }
                        }
            else if (control is ToggleButtonGroup toggleButtonGroup)
            {
                if (formProvider == FormProviders.PacItForm)
                {
                    return $"{id}: [{toggleButtonGroup.GetCheckedRadioButtonOutpostData(formField.ControlContent)}]";
                }
            }
            else if (control is ComboBox comboBox)
            {
                return CreateComboBoxOutpostDataString(formField, id);
            }
            return "";
        }

        protected string CreateOutpostMessageBody(List<string> outpostData)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string s in outpostData)
			{
				sb.Append(s + "\r");
			}
			string outpostDataMessage = sb.ToString();
			return outpostDataMessage;
		}

		public FormField[] CreateEmptyFormFieldsArray()
		{
			FormField[] formFields = new FormField[_formControlsList.Count];

            for (int i = 0; i < _formControlsList.Count; i++)
            {
                //int tagIndex;
                string tagIndexString = GetTagIndex(_formControlsList[i].InputControl);
                FormField formField = new FormField()
                {
                    //InputControl = _formControlsList[i].InputControl;
                    ControlName = _formControlsList[i].InputControl.Name,
                    ControlContent = "",
                    FormIndex = tagIndexString,  //tagIndex,
                };
                formFields.SetValue(formField, i);

            }
            return formFields;
		}

		public FormField[] CreateFormFieldsInXML()
		{
			FormField[] formFields = new FormField[_formControlsList.Count];

			for (int i = 0; i < _formControlsList.Count; i++)
			{
                FormField formField = new FormField()
                {
                    //InputControl = _formControlsList[i].InputControl,
                    ControlName = _formControlsList[i].InputControl.Name,
                    FormIndex = GetTagIndex(_formControlsList[i].InputControl),
                };

                if (_formControlsList[i].InputControl is TextBox textBox)
                {
					formField.ControlContent = textBox.Text;
					//if (((TextBox)formFieldsList[i]).IsSpellCheckEnabled)
					//{
					//	for (int j = 0; j < ((TextBox)formFieldsList[i]).Text.Length; j++)
					//	{
					//		int spellingErrorIndex = ((TextBox)formFieldsList[i]).GetSpellingErrorStart(j);
					//		if (spellingErrorIndex < 0)
					//		{
					//			continue;
					//		}
					//		else
					//		{
					//			int spellingErrorLength = ((TextBox)formFieldsList[i]).GetSpellingErrorLength(spellingErrorIndex);
					//			string misSpelledWord = ((TextBox)formFieldsList[i]).Text.Substring(spellingErrorIndex, spellingErrorLength);
					//			j += spellingErrorLength;
					//			if (formField.MisSpells is null || formField.MisSpells.Length == 0)
					//				formField.MisSpells = misSpelledWord;
					//			else
					//				formField.MisSpells += (", " + misSpelledWord);
					//		}
					//	}
					//}
				}
				else if (_formControlsList[i].InputControl is AutoSuggestBox autoSuggestBox)
				{
					formField.ControlContent = autoSuggestBox.Text;
				}
				else if (_formControlsList[i].InputControl is ComboBox comboBox)
                {
                    if (FormProvider == FormProviders.PacForm)
                    {
                        // Note the Item Type must have a ToString() method
                        formField.ControlContent = $"{comboBox.SelectedItem?.ToString()},{comboBox.SelectedIndex}";
                    }
                    else if (FormProvider == FormProviders.PacItForm)
                    {
                        if (comboBox.SelectedItem as ComboBoxPackItItem != null)
                        {
                            ComboBoxPackItItem comboBoxPackItItem = comboBox.SelectedItem as ComboBoxPackItItem;
                            comboBoxPackItItem.SelectedIndex = comboBox.SelectedIndex;
                            formField.ControlComboxContent = comboBoxPackItItem;
                            formField.ControlContent = comboBoxPackItItem?.Item;
                        }
                        else
                        {
                            formField.ControlContent = comboBox.SelectedItem?.ToString();
                            //if (string.IsNullOrEmpty(comboBox.SelectedItem?.ToString()))
                            //{
                            //    formField.ControlContent = null;
                            //}
                            //else
                            //{
                            //    formField.ControlContent = comboBox.SelectedItem?.ToString();
                            //    //string[] item = (comboBox.SelectedItem.ToString()).Split(new char[] { ' ' });
                            //    //formField.ControlContent = $"{item[0]}";
                            //}
                        }
                    }
				}
                else if (_formControlsList[i].InputControl is ToggleButtonGroup toggleButtonGroup)
                {
					formField.ControlContent = toggleButtonGroup.GetRadioButtonCheckedState();
				}
                else if (_formControlsList[i].InputControl is CheckBox checkBox)
                {
					formField.ControlContent = checkBox.IsChecked.ToString();
				}
                else if (_formControlsList[i].InputControl is RadioButton radioButton)
                {
                    formField.ControlContent = radioButton.IsChecked.ToString();
                }
                formFields.SetValue(formField, i);
			}
			return formFields;
		}

        protected virtual void FillComboBoxFromFormFields(FormField formField, ComboBox comboBox)
        {
            var data = formField.ControlContent.Split(new char[] { ',' });
            if (data.Length == 2)
            {
                // This is a PacForm ComboBox
                int index = Convert.ToInt32(data[1]);
                if (index < 0 && comboBox.IsEditable)
                {
                    comboBox.Text = data[0];
                    //comboBox.SelectedIndex = index;
                    //comboBox.Visibility = Visibility.Visible;
                }
                else
                {
                    comboBox.SelectedIndex = index;
                }
            }
            else
            {
                // Received forms neds to have their ControlComboxContent updated
                var items = comboBox.Items;
                var selectedItem = comboBox.Items[0];
                var itemSource = comboBox.ItemsSource;
                //Type typeofitem = comboBox.Items[0].GetType();
                if (formField.ControlComboxContent != null)
                {
                    comboBox.SelectedIndex = formField.ControlComboxContent.SelectedIndex;
                }
                else
                {
                    comboBox.SelectedValue = formField.ControlContent;
                }
            }
        }

        public virtual void FillFormFromFormFields(FormField[] formFields)
		{
			foreach (FormField formField in formFields)
			{
                FormControl formControl;
                if (string.IsNullOrEmpty(formField.ControlName))
                {
                    formControl = _formControlsList.Find(x => GetTagIndex(x.InputControl) == formField.FormIndex);
                }
                else
                {
                    formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);                    
                }

                Control control = formControl?.InputControl;

				if (control is null || string.IsNullOrEmpty(formField.ControlContent))
					continue;

				if (control is TextBox textBox)
				{
                    textBox.Text = formField.ControlContent;
                    // Fields that use Binding requires special handling
                    switch (control.Name)
                    {
                        case "msgDate":
                            MsgDate = textBox.Text;
                            break;
                        case "msgTime":
                            MsgTime = textBox.Text;
                            break;
                        case "incidentName":
                            IncidentName = textBox.Text;
                            break;
                        case "subject":
                            Subject = textBox.Text;
                            break;
                        case "operatorCallsign":
                            OperatorCallsign = textBox.Text;
                            break;
                        case "operatorName":
                            OperatorName = textBox.Text;
                            break;
                        case null:
                            continue;
                    }
                }
				else if (control is AutoSuggestBox autoSuggsetBox)
				{
					autoSuggsetBox.Text = formField.ControlContent;
				}
				else if (control is ComboBox comboBox)
				{
                    FillComboBoxFromFormFields(formField, comboBox);
                }
				else if (control is ToggleButtonGroup toggleButtonGroup)
				{
                    toggleButtonGroup.SetRadioButtonCheckedState(formField.ControlContent);
				}
				else if (control is CheckBox checkBox)
				{
                    checkBox.IsChecked = formField.ControlContent == "True" ? true : false;
				}
			}
            UpdateFormFieldsRequiredColors();
        }

		public static string GetOutpostFieldValue(string field)
		{
			int startIndex = field.IndexOf('[');
			int endIndex = field.IndexOf(']');
			if (startIndex != -1 && endIndex != -1)
			{
                if (field.Substring(startIndex + 1, endIndex - startIndex - 1).StartsWith("\\n"))   // For PacForms
                {
                    return field.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                {
                    return field.Substring(startIndex + 1, endIndex - startIndex - 1);
                }
			}
			else
			{
				return "";
			}
		}

        public static string GetOutpostValue(string msgLine)
        {
            int index = msgLine.IndexOf(':');
            if (index == -1)
            {
                return "";
            }
            return GetOutpostFieldValue(msgLine);
        }

        public static string GetOutpostValue(string fieldIdent, ref string[] msgLines)
		{
			for (int i = 4; i < msgLines.Length; i++)
			{
				int index = msgLines[i].IndexOf(':');
				if (index == -1)
				{
					continue;
				}
                if (fieldIdent == msgLines[i].Substring(0, index))
				{
					return GetOutpostFieldValue(msgLines[i]);
				}
			}
			return null;
		}

		protected void Subject_Changed(object sender, RoutedEventArgs e)
		{
            foreach (FormControl formControl in _formControlsList)
            {
                if (sender is RadioButton radioButton)
                {
                    //if (radioButton.Name == "emergency") No longer allowed
                    //{
                    //    HandlingOrder = "immediate";
                    //}
                    if (formControl.InputControl is ToggleButtonGroup toggleButtonGroup && toggleButtonGroup.Name == radioButton.GroupName)
                    {
                        toggleButtonGroup.CheckedControlName = radioButton.Name;
                        if (string.IsNullOrEmpty(toggleButtonGroup.GetRadioButtonCheckedState()))
                        {
                            toggleButtonGroup.ToggleButtonGroupBrush = formControl.RequiredBorderBrush;
                        }
                        else
                        {
                            toggleButtonGroup.ToggleButtonGroupBrush = new SolidColorBrush(Colors.Black);
                        }
                        break;
                    }
                }
                else if (sender is TextBox textBox && textBox.Name == formControl.InputControl.Name)
                {
                    if (IsFieldRequired(sender as TextBox) && string.IsNullOrEmpty(textBox.Text))
                    {
                        textBox.BorderThickness = new Thickness(2);
                        textBox.BorderBrush = formControl.RequiredBorderBrush;
                    }
                    else
                    {
                        textBox.BorderThickness = new Thickness(1);
                        textBox.BorderBrush = formControl.BaseBorderColor;
                    }
                    break;
                }
                else if (sender is ComboBox comboBox && comboBox.Name == formControl.InputControl.Name)
                {
                    if (comboBox.SelectedIndex < 0)
                    {
                        comboBox.BorderBrush = formControl.RequiredBorderBrush;
                    }
                    else
                    {
                        comboBox.BorderBrush = formControl.BaseBorderColor;
                    }
                    break;
                }
            }
            EventHandler<FormEventArgs> OnSubjectChange = EventSubjectChanged;
            FormEventArgs formEventArgs = new FormEventArgs() { SubjectLine = MessageNo };
			OnSubjectChange?.Invoke(this, formEventArgs);
		}

        protected string ConvertTabsToSpaces(string text, int tabWidth)
		{
			StringBuilder sb = new StringBuilder();
			var lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
			string[] convertedLines = new string[lines.Length];
			foreach (string line in lines)
			{
				string convertedLine = ConvertLineTabsToSpaces(line, tabWidth);
				sb.AppendLine(convertedLine);
			}
			return sb.ToString();
		}

		protected static string ConvertLineTabsToSpaces(string line, int tabWidth)
		{
			string convertedLine;
			StringBuilder sb = new StringBuilder();
			int j = 0;
			for (int i = 0; i < line.Length; i++, j++)
			{
				if (line[i] != '\t')
				{
					sb.Append(line[i]);
				}
				else
				{
					j--;
					int spaceCount = tabWidth - j % tabWidth;
					for (int k = 0; k < spaceCount; k++)
					{
						sb.Append(' ');
						j++;
					}
				}
			}
			convertedLine = sb.ToString();

			return convertedLine;
		}

        protected virtual void TextBoxFromICSPosition_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
            sender.Text = args.SelectedItem as string;
        }

        protected virtual void TextBoxFromICSPosition_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                //sender.ItemsSource = null;
                _ICSPositionFiltered = new List<string>();
                foreach (string s in ICSPosition)
                {
                    string lowerS = s.ToLower();
                    if (string.IsNullOrEmpty(sender.Text) || lowerS.StartsWith(sender.Text.ToLower()))
                    {
                        _ICSPositionFiltered.Add(s);
                    }
                }
                sender.ItemsSource = _ICSPositionFiltered;
            }
            AutoSuggestBox_TextChanged(sender, null);
        }

        protected virtual void UpdateRequiredFields(bool required)
        {
            if (!required)
            {
            }
            else
            {
            }
            UpdateFormFieldsRequiredColors();
        }

        protected virtual void ReportType_Checked(object sender, RoutedEventArgs e)
        {
            bool required = (bool)(sender as RadioButton).IsChecked && (sender as RadioButton).Name == "complete";
            UpdateRequiredFields(required);
            //ValidateForm();
        }

        protected virtual void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (FormPacketMessage != null && FormPacketMessage.FormFieldArray != null && comboBox.ItemsSource is List<ComboBoxPackItItem>)
            {
                foreach (FormField formField in FormPacketMessage.FormFieldArray)
                {
                    if (formField.ControlName == comboBox.Name)
                    {
                        int index = 0;
                        foreach (ComboBoxPackItItem packItItem in comboBox.ItemsSource as List<ComboBoxPackItItem>)
                        {
                            //if (packItItem.Item == formField.ControlContent)
                            if (packItItem.Data == formField.ControlContent)
                            {
                                packItItem.SelectedIndex = index;
                                comboBox.SelectedIndex = index;
                                break;
                            }
                            index++;
                        }
                        break;
                    }
                }
            }
        }

        //protected static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        //{
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        //        if (child != null && child is T)
        //            return (T)child;
        //        else
        //        {
        //            T childOfChild = FindVisualChild<T>(child);
        //            if (childOfChild != null)
        //                return childOfChild;
        //        }
        //    }
        //    return null;
        //}

        #region Print
        public virtual async void PrintForm()
        {
            //DirectPrintContainer.Children.Remove(PrintableContent);

            //_printHelper = new PrintHelper(Container);
            //_printHelper.AddFrameworkElementToPrint(PrintableContent);

            //_printHelper.OnPrintCanceled += PrintHelper_OnPrintCanceled;
            //_printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
            //_printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;

            //// Create a new PrintHelperOptions instance

            //await _printHelper.ShowPrintUIAsync("ICS 309");
        }

        //public virtual void PrepareForPrinting()
        //{

        //}

        //public virtual void RestoreFromPrinting()
        //{
        //    //if (!DirectPrintContainer.Children.Contains(PrintableContent))
        //    //{
        //    //    DirectPrintContainer.Children.Add(PrintableContent);
        //    //}
        //}

        protected virtual void ReleasePrintHelper()
        {
            //_printHelper.Dispose();

            //if (!DirectPrintContainer.Children.Contains(PrintableContent))
            //{
            //    DirectPrintContainer.Children.Add(PrintableContent);
            //}
        }

        protected virtual void PrintHelper_OnPrintSucceeded()
        {
            ReleasePrintHelper();
        }

        protected virtual void PrintHelper_OnPrintFailed()
        {
            ReleasePrintHelper();

            //_logHelper.Log(LogLevel.Error, $"Print failed. {_ICS309ViewModel.OperationalPeriod}");
        }

        protected virtual void PrintHelper_OnPrintCanceled()
        {
            ReleasePrintHelper();
        }

        #endregion Print

    }
}


