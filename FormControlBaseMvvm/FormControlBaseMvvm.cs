﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedCode;
using SharedCode.Models;

using ToggleButtonGroupControl;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FormControlBaseMvvmNameSpace
{
    public class FormControlBaseMvvm : UserControl
    {
        //public static SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);
        //public static SolidColorBrush WhiteBrush = new SolidColorBrush(Colors.White);
        //public static SolidColorBrush BlackBrush = new SolidColorBrush(Colors.Black);
        //public static SolidColorBrush TextBoxBorderBrush = new SolidColorBrush(Color.FromArgb(66, 0, 0, 0));
        //public static SolidColorBrush LightSalmonBrush = new SolidColorBrush(Colors.LightSalmon);
        //public static SolidColorBrush LightGreenBrush = new SolidColorBrush(Colors.LightGreen);
        //public static SolidColorBrush PinkBrush = new SolidColorBrush(Colors.Pink);
        //public static SolidColorBrush LightGrayBrush = new SolidColorBrush(Colors.LightGray);
        //public static SolidColorBrush GainsboroBrush = new SolidColorBrush(Colors.Gainsboro);
        //public static SolidColorBrush YellowBrush = new SolidColorBrush(Colors.Yellow);
        //public static SolidColorBrush OrangeBrush = new SolidColorBrush(Colors.Orange);

        //protected List<FormControl> _formControlsList = new List<FormControl>();
        //protected List<RadioButton> _radioButtonsList = new List<RadioButton>();


        public PacketMessage FormPacketMessage2
        { get; set; }

        //public virtual FormControlBaseMvvm RootPanel
        //{ get; set; }


        //protected virtual void ScanControls(DependencyObject panelName, FrameworkElement formUserControl = null)
        //{
        //    int count = VisualTreeHelper.GetChildrenCount(panelName);

        //    for (int i = 0; i < count; i++)
        //    {
        //        DependencyObject control = VisualTreeHelper.GetChild(panelName, i);

        //        if (control is StackPanel || control is Grid || control is Border || control is RelativePanel)
        //        {
        //            ScanControls(control, formUserControl);
        //        }
        //        else if (control is TextBox textBox)
        //        {
        //            FormControl formControl = new FormControl((FrameworkElement)control, formUserControl)
        //            {
        //                //if (textBox.IsReadOnly)
        //                //{
        //                //    formControl.BaseBorderColor = WhiteBrush;
        //                //}
        //                //else
        //                //{
        //                BaseBorderColor = textBox.BorderBrush
        //            };
        //            //}
        //            _formControlsList.Add(formControl);
        //        }
        //        else if (control is ComboBox comboBox)
        //        {
        //            FormControl formControl = new FormControl((FrameworkElement)control, formUserControl)
        //            {
        //                BaseBorderColor = comboBox.BorderBrush
        //            };
        //            _formControlsList.Add(formControl);
        //        }
        //        else if (control is CheckBox || control is ToggleButtonGroup || control is RichTextBlock)
        //        {
        //            FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
        //            _formControlsList.Add(formControl);
        //        }
        //        else if (control is AutoSuggestBox)
        //        {
        //            FormControl formControl = new FormControl((FrameworkElement)control, formUserControl)
        //            {
        //                BaseBorderColor = TextBoxBorderBrush
        //            };
        //            _formControlsList.Add(formControl);
        //        }
        //        else if (control is RadioButton)
        //        {
        //            FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
        //            _formControlsList.Add(formControl);

        //            _radioButtonsList.Add((RadioButton)control);
        //        }
        //        else if (control is ToggleSwitch)
        //        {
        //            FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
        //            _formControlsList.Add(formControl);
        //        }
        //    }
        //}

        //public virtual void LockForm()
        //{
        //    if (FormPacketMessage.MessageState != MessageState.Locked)
        //        return;

        //    //TextBox
        //    RootPanel.Resources["TextControlBorderBrushPointerOver"] = RootPanel.Resources["ComboBoxFocusedBackgroundThemeBrush"];
        //    RootPanel.Resources["TextControlBorderBrushFocused"] = RootPanel.Resources["ComboBoxFocusedBackgroundThemeBrush"];
        //    // CheckBox
        //    RootPanel.Resources["CheckBoxForegroundCheckedDisabled"] = RootPanel.Resources["CheckBoxForegroundChecked"];
        //    RootPanel.Resources["CheckBoxBackgroundCheckedDisabled"] = RootPanel.Resources["CheckBoxBackgroundChecked"];
        //    RootPanel.Resources["CheckBoxBorderBrushCheckedDisabled"] = RootPanel.Resources["CheckBoxBorderBrushChecked"];
        //    RootPanel.Resources["CheckBoxCheckBackgroundStrokeCheckedDisabled"] = RootPanel.Resources["CheckBoxCheckBackgroundStrokeChecked"];
        //    RootPanel.Resources["CheckBoxCheckBackgroundFillCheckedDisabled"] = RootPanel.Resources["CheckBoxCheckBackgroundFillChecked"];
        //    RootPanel.Resources["CheckBoxCheckGlyphForegroundCheckedDisabled"] = RootPanel.Resources["CheckBoxCheckGlyphForegroundChecked"];

        //    RootPanel.Resources["CheckBoxForegroundUncheckedDisabled"] = RootPanel.Resources["CheckBoxForegroundUnchecked"];
        //    RootPanel.Resources["CheckBoxBackgroundUncheckedDisabled"] = RootPanel.Resources["CheckBoxBackgroundUnchecked"];
        //    RootPanel.Resources["CheckBoxBorderBrushUncheckedDisabled"] = RootPanel.Resources["CheckBoxBorderBrushUnchecked"];
        //    RootPanel.Resources["CheckBoxCheckBackgroundStrokeUncheckedDisabled"] = RootPanel.Resources["CheckBoxCheckBackgroundStrokeUnchecked"];
        //    RootPanel.Resources["CheckBoxCheckBackgroundFillUncheckedDisabled"] = RootPanel.Resources["CheckBoxCheckBackgroundFillUnchecked"];
        //    RootPanel.Resources["CheckBoxCheckGlyphForegroundUncheckedDisabled"] = RootPanel.Resources["CheckBoxCheckGlyphForegroundUnchecked"];

        //    // RadioButton
        //    RootPanel.Resources["RadioButtonForegroundDisabled"] = RootPanel.Resources["RadioButtonForeground"];
        //    RootPanel.Resources["RadioButtonOuterEllipseFillDisabled"] = RootPanel.Resources["RadioButtonOuterEllipseFill"];
        //    RootPanel.Resources["RadioButtonOuterEllipseCheckedFillDisabled"] = RootPanel.Resources["RadioButtonOuterEllipseCheckedFill"];
        //    RootPanel.Resources["RadioButtonCheckGlyphFillDisabled"] = RootPanel.Resources["RadioButtonCheckGlyphFill"];

        //    //ToggleSwitch
        //    RootPanel.Resources["ToggleSwitchContainerBackgroundDisabled"] = RootPanel.Resources["ToggleSwitchContainerBackground"];
        //    RootPanel.Resources["ToggleSwitchContentForegroundDisabled"] = RootPanel.Resources["ToggleSwitchContentForeground"];
        //    RootPanel.Resources["ToggleSwitchFillOffDisabled"] = RootPanel.Resources["ToggleSwitchFillOff"];
        //    RootPanel.Resources["ToggleSwitchFillOnDisabled"] = RootPanel.Resources["ToggleSwitchFillOn"];
        //    RootPanel.Resources["ToggleSwitchKnobFillOffDisabled"] = RootPanel.Resources["ToggleSwitchKnobFillOff"];
        //    RootPanel.Resources["ToggleSwitchKnobFillOnDisabled"] = RootPanel.Resources["ToggleSwitchKnobFillOn"];


        //    foreach (FormControl formControl in _formControlsList)
        //    {
        //        FrameworkElement control = formControl.InputControl;

        //        if (control is TextBox textBox)
        //        {
        //            textBox.IsReadOnly = true;
        //            textBox.IsSpellCheckEnabled = false;
        //            textBox.PlaceholderText = "";
        //            textBox.BorderBrush = WhiteBrush;
        //        }
        //        else if (control is AutoSuggestBox autoSuggestBox)
        //        {
        //            if (FindName($"{autoSuggestBox.Name}TextBox") is TextBox autoSuggestBoxAsTextBox)
        //            {
        //                autoSuggestBox.Visibility = Visibility.Collapsed;

        //                autoSuggestBoxAsTextBox.Visibility = Visibility.Visible;
        //                autoSuggestBoxAsTextBox.IsReadOnly = true;
        //                autoSuggestBoxAsTextBox.IsSpellCheckEnabled = false;
        //                autoSuggestBoxAsTextBox.PlaceholderText = "";
        //                autoSuggestBoxAsTextBox.BorderBrush = WhiteBrush;
        //                autoSuggestBoxAsTextBox.VerticalAlignment = VerticalAlignment.Center;
        //                autoSuggestBoxAsTextBox.HorizontalAlignment = HorizontalAlignment.Left;

        //                FormField formField = FormPacketMessage.FormFieldArray.FirstOrDefault(f => f.ControlName == autoSuggestBox.Name);
        //                if (formField != null && formField.ControlContent != null)
        //                {
        //                    autoSuggestBoxAsTextBox.Text = formField.ControlContent;
        //                }
        //            }
        //        }
        //        else if (control is ComboBox comboBox)
        //        {
        //            if (FindName($"{comboBox.Name}TextBox") is TextBox comboBoxAsTextBox)
        //            {
        //                comboBox.Visibility = Visibility.Collapsed;

        //                comboBoxAsTextBox.Visibility = Visibility.Visible;
        //                comboBoxAsTextBox.IsReadOnly = true;
        //                comboBoxAsTextBox.IsSpellCheckEnabled = false;
        //                comboBoxAsTextBox.VerticalAlignment = VerticalAlignment.Center;
        //                comboBoxAsTextBox.HorizontalAlignment = HorizontalAlignment.Left;

        //                FormField formField = FormPacketMessage.FormFieldArray.FirstOrDefault(f => f.ControlName == comboBox.Name);
        //                if (formField != null && formField.ControlContent != null)
        //                {
        //                    comboBoxAsTextBox.Text = formField.ControlContent;
        //                }
        //            }
        //        }
        //        else if (control is RadioButton radioButton)
        //        {
        //            radioButton.IsEnabled = false;
        //        }
        //        else if (control is CheckBox checkBox)
        //        {
        //            checkBox.IsEnabled = false;
        //        }
        //        else if (control is ToggleSwitch toggleSwitch)
        //        {
        //            toggleSwitch.IsEnabled = false;
        //        }
        //    }
        //}

    }
}
