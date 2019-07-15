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

    public abstract class FormControlBase : FormControlBasics
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

        private static Dictionary<string, object> _properties = new Dictionary<string, object>();
        static Dictionary<string, bool> _propertyFirstTime = new Dictionary<string, bool>();


        public FormControlBase()
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
            bool firstTime;
            if (_propertyFirstTime.ContainsKey(propertyName))
            {
                firstTime = _propertyFirstTime[propertyName];
            }
            else
            {
                firstTime = true;
            }
            // Do not update displayed value if not changed or not first time or not forced
            if (Equals(backingStore, value) && !firstTime && !forceUpdate)
            {
                return false;
            }
            _propertyFirstTime[propertyName] = false;

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

        public void InitializeControls()
        {
            foreach (FormControl formControl in _formControlsList)
            {
                Control control = formControl.InputControl;

                if (control is TextBox)
                {
                    //if (IsFieldRequired(control))
                    //{
                    //    control.BorderBrush = formControl.RequiredBorderBrush;
                    //}
                    //else
                    //{
                    //    control.BorderBrush = formControl.BaseBorderColor;
                    //}
                }
                //else if (control is AutoSuggestBox)
                //{
                //    control.BorderBrush = formControl.BaseBorderColor;
                //}
                //else if (control is ComboBox)
                //{
                //    if (IsFieldRequired(control))
                //    {
                //        control.BorderBrush = formControl.RequiredBorderBrush;
                //    }
                //    else
                //    {
                //        control.BorderBrush = formControl.BaseBorderColor;
                //    }
                //}
                else if (control is ToggleButtonGroup toggleButtonGroup)
                {
                    toggleButtonGroup.Initialize(_radioButtonsList, control.Name);
                }
                //else if (control is CheckBox checkBox)
                //{
                //    checkBox.IsChecked = false;
                //}
            }
        }

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

        public virtual void InitializeFormRequiredColors(bool newForm = false)
        {
            foreach (FormControl formControl in _formControlsList)
            {
                Control control = formControl.InputControl;

                if (control is TextBox || control is ComboBox)
                {
                    if (IsFieldRequired(control) && newForm)
                    {
                        control.BorderBrush = formControl.RequiredBorderBrush;
                    }
                    else
                    {
                        control.BorderBrush = formControl.BaseBorderColor;
                    }
                }
                else if (control is AutoSuggestBox)
                {
                    if (IsFieldRequired(control) && newForm)
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
                //else if (control is ComboBox)
                //{
                //    if (IsFieldRequired(control) && newForm)
                //    {
                //        control.BorderBrush = formControl.RequiredBorderBrush;
                //    }
                //    else
                //    {
                //        control.BorderBrush = formControl.BaseBorderColor;
                //    }
                //}
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

        public virtual string TacticalCallsign
        { get; set; }

        public virtual string OperatorCallsign
        { get; set; }

        public List<FormControl> FormControlsList
        { get => _formControlsList; }

        public string ValidationResultMessage
        {
            get => _validationResultMessage;
            set => _validationResultMessage = value;
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

        public virtual string MsgDate
        { get; set; }

        protected string TimeCheck(string time)
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

        public virtual string IncidentName
        { get; set; }

        public virtual string Subject
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

        //public virtual FormProviders DefaultFormProvider
        //{ get; }

        public abstract FormProviders FormProvider
        { get; }

        public abstract string PacFormName
		{ get; }

        public abstract string PacFormType
        { get; }

        public abstract string CreateSubject();

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

        protected virtual string ConvertComboBoxFromOutpost(string id, ref string[] msgLines)
        {
            string comboBoxData = GetOutpostValue(id, ref msgLines);
            var comboBoxDataSet = comboBoxData.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);

            return comboBoxDataSet[0];
        }

        protected virtual FormField[] ConvertFromOutpostPackItForm(FormField[] formFields, ref string[] msgLines)
        {
            string senderMsgNo = "";
            if (!string.IsNullOrEmpty(GetOutpostValue("1", ref msgLines)))
            {
                senderMsgNo = GetOutpostValue("1", ref msgLines);
            }
            else if (!string.IsNullOrEmpty(GetOutpostValue("3", ref msgLines)))
            {
                senderMsgNo = GetOutpostValue("3", ref msgLines);
            }

            foreach (FormField formField in formFields)
            {
                (string id, Control control) = GetTagIndex(formField);
                formField.PacFormIndex = id;

                if (control is ToggleButtonGroup)
                {
                    string outpostValue = GetOutpostValue(id, ref msgLines);
                    if (!string.IsNullOrEmpty(outpostValue))
                    {
                        foreach (RadioButton radioButton in ((ToggleButtonGroup)control).RadioButtonGroup)
                        {
                            //if (outpostValue == radioButton.Content as string)
                            //{
                            //    formField.ControlContent = radioButton.Name;
                            //}
                            if ((radioButton.Content as string).Contains(outpostValue))
                            {
                                formField.ControlContent = radioButton.Name;
                            }
                            else if (radioButton.Name == outpostValue)
                            {
                                formField.ControlContent = radioButton.Name;
                            }
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

                    //string conboBoxData = GetOutpostValue(id, ref msgLines);
                    //var comboBoxDataSet = conboBoxData.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
                    //formField.ControlContent = comboBoxDataSet[0];
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
                    }
                }
            }
            return formFields;
        }

    public virtual FormField[] ConvertFromOutpost(string msgNumber, ref string[] msgLines, FormProviders formProvider)
        {
            FormField[] formFields = CreateEmptyFormFieldsArray();

            // Populate Sender Message Number from received message number
            // Sender message number can be either in 1 or 3
            if (formProvider == FormProviders.PacItForm)
            {
                formFields = ConvertFromOutpostPackItForm(formFields, ref msgLines);
            }
            else
            {
                string senderMsgNo = "";

                if (!string.IsNullOrEmpty(GetOutpostValue("1", ref msgLines)))
            {
                senderMsgNo = GetOutpostValue("1", ref msgLines);
            }
            else if (!string.IsNullOrEmpty(GetOutpostValue("3", ref msgLines)))
            {
                senderMsgNo = GetOutpostValue("3", ref msgLines);
            }

            foreach (FormField formField in formFields)
            {
                (string id, Control control) = GetTagIndex(formField, formProvider);
                formField.PacFormIndex = id;    //Convert.ToInt32(id == "" ? "-1" : id);

                if (control is ToggleButtonGroup)
                {
                    foreach (RadioButton radioButton in ((ToggleButtonGroup)control).RadioButtonGroup)
                    {
                        string radioButtonIndex = GetTagIndex(radioButton, formProvider);
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

                    //string conboBoxData = GetOutpostValue(id, ref msgLines);
                    //var comboBoxDataSet = conboBoxData.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
                    //formField.ControlContent = comboBoxDataSet[0];
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
                //control = formField.InputControl;
                FormControl formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);
                control = formControl?.InputControl;

                string tag = (string)control.Tag;
                if (!string.IsNullOrEmpty(tag))
                {
                    string[] tags = tag.Split(new char[] { ',' });
                    if (!tags[0].Contains("required"))
                    {
                        return (tags[0], control);
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

        public (string id, Control control) GetTagIndex(FormField formField, FormProviders formProvider)
        {
            if (formField is null)
                return ("", null);

            Control control = null;
            try
            {
                //control = formField.InputControl;
                FormControl formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);
                control = formControl?.InputControl;

                string tag = (string)control.Tag;
                if (!string.IsNullOrEmpty(tag))
                {
                    string[] tags = tag.Split(new char[] { ',' });
                    if (!tags[0].Contains("required"))
                    {
                        string[] formProviderTags = tags[0].Split(new char[] { '|' });
                        if (formProviderTags.Length > 1)
                        {
                            return (formProviderTags[(int)formProvider], control);
                        }
                        else
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

        public static string GetTagIndex(Control control, FormProviders formProvider)
        {
            try
            {
                string tag = (string)control.Tag;
                if (!string.IsNullOrEmpty(tag))
                {
                    string[] tags = tag.Split(new char[] { ',' });
                    if (!tags[0].Contains("required"))
                    {
                        string[] formProviderTags = tags[0].Split(new char[] { '|' });
                        if (formProviderTags.Length > 1)
                        {
                            return formProviderTags[(int)formProvider];
                        }
                        else
                        {
                            return tags[0];
                        }
                    }
                }
            }
            catch
            {
            }
            return "";
        }

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
                    string[] selection = formField.ControlContent.Split(new char[] { ' ' });
                    return $"{id}: [{selection[0]}]";
            }
            return "";
        }

        public string CreateOutpostDataString(FormField formField, FormProviders formProvider)
        {
            //if (string.IsNullOrEmpty(formField.ControlContent))
            //    return "";

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
                    if (toggleButtonGroup.Name == "severity" || toggleButtonGroup.Name == "handlingOrder")
                    {
                        return $"{id}: [{formField.ControlContent.ToUpper()}]";
                    }
                    else if (formField.ControlContent.ToLower().Contains("yes"))
                    {
                        return $"{id}: [Yes]";
                    }
                    else if (formField.ControlContent.ToLower().Contains("no"))
                    {
                        return $"{id}: [No]";
                    }
                    else if (toggleButtonGroup.Tag as string == "Rec-Sent")
                    {
                        return $"{id}: [sender]";
                    }
                    else if (toggleButtonGroup.Name == "howRecevedSent")
                    {
                        foreach (RadioButton radioButton in toggleButtonGroup.RadioButtonGroup)
                        {
                           if (formField.ControlContent == radioButton.Name)
                                return $"{id}: [{radioButton.Content}]";
                        }                        
                    }
                    else return "";
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
                //string tagIndexString = GetTagIndex(_formControlsList[i].InputControl, FormProvider);
                string tagIndexString = GetTagIndex(_formControlsList[i].InputControl);
                FormField formField = new FormField()
                {
                    //InputControl = _formControlsList[i].InputControl;
                    ControlName = _formControlsList[i].InputControl.Name,
                    ControlContent = "",
                    PacFormIndex = tagIndexString,  //tagIndex,
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
                    PacFormIndex = GetTagIndex(_formControlsList[i].InputControl),
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
                comboBox.SelectedItem = formField.ControlContent;  // data[0];
            }
        }

        public void FillFormFromFormFields(FormField[] formFields)
		{
			foreach (FormField formField in formFields)
			{
				FormControl formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);

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
                    if (radioButton.Name == "emergency")
                    {
                        HandlingOrder = "immediate";
                    }
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
                    }
                }
                else if (sender is TextBox textBox && textBox.Name == formControl.InputControl.Name)
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        textBox.BorderBrush = formControl.RequiredBorderBrush;
                    }
                    else
                    {
                        textBox.BorderBrush = formControl.BaseBorderColor;
                    }
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

		protected string ConvertLineTabsToSpaces(string line, int tabWidth)
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

    }
}


