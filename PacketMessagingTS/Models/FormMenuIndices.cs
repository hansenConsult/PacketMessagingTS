﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;

using MetroLog;

using PacketMessagingTS.Helpers;

using SharedCode;
using SharedCode.Helpers;

using Windows.Storage;
using Windows.Storage.FileProperties;

// 
// This source code was auto-generated by xsd, Version=4.8.4084.0.
// 

namespace PacketMessagingTS.Models
{ 
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.4084.0")]
    [System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class FormMenuIndexDefinitions {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<FormMenuIndexDefinitions>();
        private static LogHelper _logHelper = new LogHelper(log);

        public static FormMenuIndexDefinitions Instance { get; private set; } = new FormMenuIndexDefinitions();

        public static string FormMenuIndexDefinitionsFileName = "FormMenuOrder.xml";


        private string[] countyFormsMenuNamesField;
    
        private string[] cityFormsMenuNamesField;
    
        private string[] hospitalFormsMenuNamesField;

        private string[] otherFormsMenuNamesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("string", IsNullable=false)]
        public string[] CountyFormsMenuNames {
            get {
                return this.countyFormsMenuNamesField;
            }
            set {
                this.countyFormsMenuNamesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("string", IsNullable=false)]
        public string[] CityFormsMenuNames
        {
            get {
                return this.cityFormsMenuNamesField;
            }
            set {
                this.cityFormsMenuNamesField = value;
            }
        }
    
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("string", IsNullable=false)]
        public string[] HospitalFormsMenuNames
        {
            get {
                return this.hospitalFormsMenuNamesField;
            }
            set {
                this.hospitalFormsMenuNamesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("string", IsNullable = false)]
        public string[] OtherFormsMenuNames
        {
            get
            {
                return this.otherFormsMenuNamesField;
            }
            set
            {
                this.otherFormsMenuNamesField = value;
            }
        }

        private List<FormControlAttributes> ScanFormAttributes()
        {
            List<FormControlAttributes> formControlAttributeList = new List<FormControlAttributes>();

            foreach (Assembly assembly in SharedData.Assemblies)
            {
                Type[] expTypes = assembly.GetExportedTypes();
                foreach (Type classType in expTypes)
                {
                    //_logHelper.Log(LogLevel.Info, $"Type: {classType.Name}");
                    var attrib = classType.GetTypeInfo();
                    //foreach (CustomAttributeData customAttribute in attrib.CustomAttributes.Where(customAttribute => customAttribute.GetType() == typeof(CustomAttributeData)))
                    foreach (CustomAttributeData customAttribute in attrib.CustomAttributes)
                    {
                        if (customAttribute.AttributeType != typeof(FormControlAttribute))
                            continue;
                        IList<CustomAttributeNamedArgument> namedArguments = customAttribute.NamedArguments;
                        if (namedArguments.Count == FormControlAttributes.AttributesCount)
                        {
                            string formControlName = "";
                            FormControlAttribute.FormType formControlType = FormControlAttribute.FormType.Undefined;
                            string formControlMenuName = "";
                            foreach (CustomAttributeNamedArgument arg in namedArguments)
                            {
                                if (arg.MemberName == "FormControlName")
                                {
                                    formControlName = arg.TypedValue.Value as string;
                                }
                                else if (arg.MemberName == "FormControlType")
                                {
                                    formControlType = (FormControlAttribute.FormType)Enum.Parse(typeof(FormControlAttribute.FormType), arg.TypedValue.Value.ToString());
                                }
                                else if (arg.MemberName == "FormControlMenuName")
                                {
                                    formControlMenuName = arg.TypedValue.Value as string;
                                }
                            }
                            FormControlAttributes formControlAttributes = new FormControlAttributes(formControlName, formControlMenuName, formControlType);
                            formControlAttributeList.Add(formControlAttributes);
                        }
                    }
                }
            }
            return formControlAttributeList;
        }
        public async Task OpenAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            ulong size = 0;
            var storageItem = await localFolder.TryGetItemAsync(FormMenuIndexDefinitionsFileName);
            if (storageItem != null)
            {
                BasicProperties basicProperties = await storageItem.GetBasicPropertiesAsync();
                size = basicProperties.Size;   
            }
            if (storageItem is null || size == 0)
            {
                try
                {
                    var assetsFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                    StorageFile menuOrderDataFile = await assetsFolder.TryGetItemAsync(FormMenuIndexDefinitionsFileName)as StorageFile;
                    if (menuOrderDataFile != null)
                    {
                        StorageFile file = await menuOrderDataFile.CopyAsync(localFolder);
                    }
                    else
                    {
                        // Create a new file. Only used in special cases, normally copy from Assets
                        List<string> countyFormsList = new List<string>();
                        List<string> cityFormsList = new List<string>();
                        List<string> hospitalFormsList = new List<string>();
                        List<string> otherFormsList = new List<string>();

                        List<FormControlAttributes> formControlAttributeList = ScanFormAttributes();
                        foreach (FormControlAttributes formControlAttribute in formControlAttributeList)
                        {
                            switch (formControlAttribute.FormControlType)
                            {
                                case FormControlAttribute.FormType.Undefined:
                                    break;
                                case FormControlAttribute.FormType.None:
                                    countyFormsList.Add(formControlAttribute.FormControlMenuName);
                                    break;
                                case FormControlAttribute.FormType.CountyForm:
                                    countyFormsList.Add(formControlAttribute.FormControlMenuName);
                                    break;
                                case FormControlAttribute.FormType.CityForm:
                                    cityFormsList.Add(formControlAttribute.FormControlMenuName);
                                    break;
                                case FormControlAttribute.FormType.HospitalForm:
                                    hospitalFormsList.Add(formControlAttribute.FormControlMenuName);
                                    break;
                                case FormControlAttribute.FormType.TestForm:
                                    otherFormsList.Add(formControlAttribute.FormControlMenuName);
                                    break;
                            }
                        }
                        CountyFormsMenuNames = countyFormsList.ToArray();
                        CityFormsMenuNames = cityFormsList.ToArray();
                        HospitalFormsMenuNames = hospitalFormsList.ToArray();
                        OtherFormsMenuNames = otherFormsList.ToArray();

                        SaveAsync();
                    }
                }
                catch (Exception e)
                {
                    _logHelper.Log(LogLevel.Error, $"Error opening {e.Message} {e}");
                }
            }

            try
            {
                StorageFile FormMenuIndexDefinitionsDataFile = await localFolder.GetFileAsync(FormMenuIndexDefinitionsFileName);

                using (FileStream reader = new FileStream(FormMenuIndexDefinitionsDataFile.Path, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(FormMenuIndexDefinitions));
                    Instance = (FormMenuIndexDefinitions)serializer.Deserialize(reader);
                }
            }
            catch (FileNotFoundException )
            {
                _logHelper.Log(LogLevel.Error, $"File not found: {FormMenuIndexDefinitionsFileName}");
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"Error opening {e.Message} {e}");
            }
        }

        public static async void SaveAsync()
        {

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            try
            {
                var storageItem = await localFolder.CreateFileAsync(FormMenuIndexDefinitionsFileName, CreationCollisionOption.ReplaceExisting);
                if (storageItem != null)
                {
                    using (StreamWriter writer = new StreamWriter(new FileStream(storageItem.Path, FileMode.Create)))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(FormMenuIndexDefinitions));
                        serializer.Serialize(writer, Instance);
                    }
                }
                else
                {
                    _logHelper.Log(LogLevel.Error, $"File not found {FormMenuIndexDefinitionsFileName}");

                }
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"Error saving {FormMenuIndexDefinitionsFileName} {e}");
            }
        }

    }

    internal class Array<T>
    {
    }

    /// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.4084.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    //public partial class FormData
    //{

    //    private string menuNameField;

    //    private int menuIndexField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public string FormName
    //    {
    //        get
    //        {
    //            return this.menuNameField;
    //        }
    //        set
    //        {
    //            this.menuNameField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public int MenuIndex
    //    {
    //        get
    //        {
    //            return this.menuIndexField;
    //        }
    //        set
    //        {
    //            this.menuIndexField = value;
    //        }
    //    }
    //}
}
