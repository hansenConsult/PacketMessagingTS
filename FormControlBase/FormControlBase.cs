using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using ToggleButtonGroupControl;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using SharedCode;

namespace FormControlBaseClass
{
    // This is for deciding at runtime which form is supported by an assembly
    [AttributeUsage(AttributeTargets.Class)]
	public class FormControlAttribute : Attribute
	{
		public enum FormType
		{
			None,
			CountyForm,
			CityForm,
			HospitalForm
		};

        // Form file name
		public string FormControlName { get; set; }    // 
         
        // Form type (County, Hospital etc.)
		public FormType FormControlType { get; set; }

        // Menu text
		public string FormControlMenuName { get; set; }    // 
	}

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

	public abstract class FormControlBase : FormControlBasics, INotifyPropertyChanged

    {
		public event EventHandler<FormEventArgs> EventSubjectChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected List<string> outpostData;

        private static Dictionary<string, object> _properties = new Dictionary<string, object>();
        static Dictionary<string, bool> _propertyFirstTime = new Dictionary<string, bool>();


        public FormControlBase()
		{
        }

        protected void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
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

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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


        private void NotifyPropertyChanged( String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetTextBlockString(TextBlock textBlock) => textBlock.Text;

		public void SetTextBlockString(TextBlock textBlock, string text)
		{
			textBlock.Text = text;
		}

		public string GetTextBoxString(TextBox textBox)
		{
			return textBox.Text;
		}

		public string GetAutoSuggestBoxString(AutoSuggestBox autoSuggestBox)
		{
			return autoSuggestBox.Text;
		}

		public void SetTextBoxString(TextBox textBox, string text)
		{
			textBox.Text = text;
		}

        public int GetComboBoxSelectedIndex(ComboBox comboBox) => comboBox.SelectedIndex;

		object GetCBSelectedItem(ComboBox comboBox) => comboBox.SelectedItem;

		public string GetComboBoxSelectedItem(ComboBox comboBox)
		{
			return GetCBSelectedItem(comboBox)?.ToString();
		}

        public string GetComboBoxSelectedValuePath(ComboBox comboBox)
        {
            return comboBox.SelectedValuePath;
        }

		public void SetAutoSuggestBoxString(AutoSuggestBox autoSuggestBox, string text)
		{
			autoSuggestBox.Text = text;
		}

		public void SetComboBoxString(ComboBox comboBox, string text)
        {
            comboBox.SelectedValue = text;
        }

        public bool? GetCheckBoxCheckedState(CheckBox checkBox)
		{
			return checkBox.IsChecked;
		}

		public void SetCheckBoxCheckedState(CheckBox checkBox, bool? isChecked)
		{
			checkBox.IsChecked = isChecked;
		}

		public void InitializeControls()
		{
			foreach (FormControl formControl in _formControlsList)
			{
				Control control = formControl.InputControl;

				if (control is TextBox)
				{
					control.BorderBrush = formControl.BaseBorderColor;
				}
				else if (control is AutoSuggestBox)
				{
					control.BorderBrush = formControl.BaseBorderColor;
				}
				else if (control is ComboBox)
				{
					control.BorderBrush = formControl.BaseBorderColor;
				}
				else if (control is ToggleButtonGroup toggleButtonGroup)
				{
                    toggleButtonGroup.Initialize(_radioButtonsList, control.Name);
				}
				else if (control is CheckBox checkBox)
				{
                    checkBox.IsChecked = false;
				}
			}
		}

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty OperatorCallsignProperty =
        //	DependencyProperty.Register("OperatorCallsign", typeof(string), typeof(FormControlBase), null);

        private string operatorCallsign;
        public virtual string OperatorCallsign
		{
			get { return operatorCallsign; }
			set { Set(ref operatorCallsign, value); }
		}

		public List<FormControl> FormControlsList
        { get => _formControlsList; }

        public string ValidationResultMessage
        {
            get => validationResultMessage;
            set => validationResultMessage = value;
        }

        public virtual string SenderMsgNo
        { get; set; }

        public virtual string MessageNo
        { get; set; }

        public virtual string ReceiverMsgNo
        { get; set; }

		public static string DefaultMessageTo
		{ get; set; }

        private string msgDate;
        public virtual string MsgDate
        {
            get => msgDate;
            set => Set(ref msgDate, value);
        }

        public virtual string MsgTime
        { get; set; }

        public virtual string OperatorName
        { get; set; }

        private string operatorDate;
        public virtual string OperatorDate
		{
            get => operatorDate;
            set => Set(ref operatorDate, value);
        }

		public virtual string OperatorTime
		{ get; set; }

        public virtual string Severity
        { get; set; }

        public virtual string HandlingOrder
		{ get; set; }

        public virtual string ReceivedOrSent
        { get; set; }

        public virtual string HowReceivedSent
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
                // This may happen if called from view Outpost Data
                packetMessage.FormFieldArray = CreateFormFieldsInXML();
                FillFormFromFormFields(packetMessage.FormFieldArray);
            }
            foreach (FormField formField in packetMessage.FormFieldArray)
            {
                if (string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                string data = CreateOutpostDataString(formField);
                if (string.IsNullOrEmpty(data))
                {
                    continue;
                }
                outpostData.Add(data);
            }
            outpostData.Add("#EOF");
        }

        public virtual FormField[] ConvertFromOutpost(string msgNumber, ref string[] msgLines)
        {
            FormField[] formFields = CreateEmptyFormFieldsArray();

            foreach (FormField formField in formFields)
            {
                (string id, Control control) = GetTagIndex(formField);
                formField.PacFormIndex = Convert.ToInt32(id == "" ? "-1" : id);

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
                    string conboBoxData = GetOutpostValue(id, ref msgLines);
                    var comboBoxDataSet = conboBoxData.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
                    formField.ControlContent = comboBoxDataSet[0];
                }
                else if (control is TextBox || control is AutoSuggestBox)
                {
                    formField.ControlContent = GetOutpostValue(id, ref msgLines);
                    // Filter operator date and time
                    if (formField.ControlContent != null)
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
            // Move received message number to sender message number field
            formFields[0].ControlContent = GetOutpostValue("1", ref msgLines);
            return formFields;
        }

        public (string id, Control control) GetTagIndex(FormField formField)
        {
            Control control = null;
            try
            {
                FormControl formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);
                control = formControl?.InputControl;

                string tag = (string)control.Tag;
                if (!string.IsNullOrEmpty(tag))
                {
                    string[] tags = tag.Split(new char[] { ',' });
                    if (int.TryParse(tags[0], out int idint))   // Is tag an integer?
                    {
                        return (tags[0], control);
                    }
                }
            }
            catch
            {                
            }
            return ("-1", control);
        }

        public static string GetTagIndex(Control control)
        {
            try
            {
                string tag = (string)control.Tag;
                if (!string.IsNullOrEmpty(tag))
                {
                    string[] tags = tag.Split(new char[] { ',' });
                    if (int.TryParse(tags[0], out int idint))
                    {
                        return tags[0];
                    }
                }
            }
            catch
            {
            }
            return "-1";
        }

        public string GetTagErrorMessage(FormField formField)
        {
            string name = _formControlsList[1].InputControl.Name;
            FormControl formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);
            Control control = formControl.InputControl;
            string tag = control.Tag as string;
            if (string.IsNullOrEmpty(tag))
                return "";

            string[] tags = tag.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (tags.Length == 2 && tags[0] == "required")
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

        public string CreateOutpostDataString(FormField formField)
        {
            (string id, Control control) = GetTagIndex(formField);
            if (string.IsNullOrEmpty(id))
                return "";

            if (control is TextBox)
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
            else if (control is AutoSuggestBox)
            {
                return $"{id}: [{formField.ControlContent}]";
            }
            else if (control is RadioButton || control is CheckBox)
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
            else if (control is ComboBox comboBox)
            {
                var data = formField.ControlContent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 2)
                {
                    return $"{id}: [{data[0]}}}{data[1]}]";
                }
                else if (data[0] == "-1")
                {
                    return $"{id}: [ }}0]";
                }
                else
                {
                    return "";
                }
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
                int tagIndex;
                string tagIndexString = GetTagIndex(_formControlsList[i].InputControl);
                if (string.IsNullOrEmpty(tagIndexString))
                {
                    tagIndex = -1;

                }
                else
                {
                    tagIndex = Convert.ToInt32(tagIndexString);
                }
                FormField formField = new FormField()
                {
                    ControlName = _formControlsList[i].InputControl.Name,
                    ControlContent = "",
                    PacFormIndex = tagIndex,
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
                FormField formField = new FormField() { ControlName = _formControlsList[i].InputControl.Name };

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
					formField.ControlContent = $"{GetComboBoxSelectedItem(comboBox)},{comboBox.SelectedIndex}";
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

		public void FillFormFromFormFields(FormField[] formFields)
		{
			foreach (FormField formField in formFields)
			{
				FormControl formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);

				Control control = formControl?.InputControl;

				if (control is null || formField.ControlContent is null)
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
                        case "operatorCallsign":
                            OperatorCallsign = textBox.Text;
                            break;
                        case "operatorName":
                            OperatorName = textBox.Text;
                            break;
                        case "operatorDate":
                            OperatorDate = textBox.Text;
                            break;
                        case "operatorTime":
                            OperatorTime = textBox.Text;
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
                    var data = formField.ControlContent.Split(new char[] { ',' });
                    comboBox.SelectedItem = data[0];
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

		protected string GetOutpostFieldValue(string field)
		{
			int startIndex = field.IndexOf('[');
			int endIndex = field.IndexOf(']');
			if (startIndex != -1 && endIndex != -1)
			{
                if (field.Substring(startIndex + 1, endIndex - startIndex - 1).StartsWith("\\n"))
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

		public string GetOutpostValue(string fieldIdent, ref string[] msgLines)
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
            if (sender is RadioButton radioButton)
            {
                if (radioButton.Name == "emergency")
                {
                    HandlingOrder = "immediate";
                }
                foreach (FormControl formControl in _formControlsList)
                {
                    if (formControl.InputControl is ToggleButtonGroup toggleButtonGroup && toggleButtonGroup.Name == radioButton.GroupName)
                    {
                        toggleButtonGroup.CheckedControlName = radioButton.Name;
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
	}
}


