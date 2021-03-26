using MetroLog;

using SharedCode;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Windows.Storage;

namespace SharedCode.Models
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public sealed class UserAddressArray
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<AddressBook>();
        private static LogHelper _logHelper = new LogHelper(log);

        private const string addressBookFileName = "addressBook.xml";

        private AddressBookEntry[] _userAddressEntriesField = new AddressBookEntry[0];

        private static volatile UserAddressArray _instance;
        private static object _syncRoot = new Object();

        private UserAddressArray()
        { }

        public static UserAddressArray Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance is null)
                            _instance = new UserAddressArray();
                    }
                }
                return _instance;
            }
        }

        /// <remarks/>
        [XmlElement("AddressEntry")]
        public AddressBookEntry[] UserAddressBook
        {
            get => _userAddressEntriesField;
            set => _userAddressEntriesField = value;
        }

        private List<AddressBookEntry> userAddressList;
        [System.Xml.Serialization.XmlIgnore]
        public List<AddressBookEntry> UserAddressList
        {
            get
            {
                if (userAddressList is null || userAddressList.Count == 0)
                {
                    userAddressList = _userAddressEntriesField.ToList();
                }
                return userAddressList;
            }
            set
            {
                userAddressList = value;
            }
        }

        public async Task OpenAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                var storageItem = await localFolder.TryGetItemAsync(addressBookFileName);
                if (storageItem is null)
                    return;

                StorageFile file = await localFolder.GetFileAsync(addressBookFileName);

                using (FileStream stream = new FileStream(file.Path, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(UserAddressArray));
                        _instance = (UserAddressArray)serializer.Deserialize(reader);
                    }
                }



                //using (FileStream reader = new FileStream(file.Path, FileMode.Open))
                //{
                //    XmlSerializer serializer = new XmlSerializer(typeof(UserAddressArray));
                //    _instance = (UserAddressArray)serializer.Deserialize(reader);
                //}
            }
            catch (FileNotFoundException e)
            {
                _logHelper.Log(LogLevel.Error, $"Open Address Book file failed: {e.Message}");
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"Error opening {e.Message} {e}");
            }
        }

        public async Task SaveAsync()
        {
            AddressBookEntry[] addresseList = UserAddressList.ToArray();
            _userAddressEntriesField = new AddressBookEntry[UserAddressList.Count];
            addresseList.CopyTo(_userAddressEntriesField, 0);

            if (UserAddressBook is null || UserAddressBook.Length == 0)
                return;

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await localFolder.CreateFileAsync(addressBookFileName, CreationCollisionOption.ReplaceExisting);
                using (StreamWriter writer = new StreamWriter(new FileStream(file.Path, FileMode.OpenOrCreate)))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(UserAddressArray));
                    serializer.Serialize(writer, this);
                }
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"Error saving {addressBookFileName} {e}");
            }
        }

        public bool AddAddressAsync(AddressBookEntry addressBookEntry)
        {
            // Validate entries
            // If @ check if BBS. If BBS remove BBS and rely on primary/secondary. If not BBS save whole address.
            int index = addressBookEntry.Callsign.IndexOf('@');
            if (index > 0)
            {
                string bbsCallsign = addressBookEntry.Callsign.Substring(index + 1, 5);
                if (bbsCallsign == "W1XSC" || bbsCallsign == "W2XSC" || bbsCallsign == "W3XSC" || bbsCallsign == "W4XSC" || bbsCallsign == "W5XSC")
                {
                    addressBookEntry.Callsign = addressBookEntry.Callsign.Substring(0, index);
                }
            }

            Dictionary<string, AddressBookEntry> addressBookDictionary = AddressBook.Instance.AddressBookDictionary;
            bool foundEntry = addressBookDictionary.TryGetValue(addressBookEntry.Callsign, out AddressBookEntry oldAddressBookEntry);
            if (!foundEntry)
            {
                //string temp = addressBookEntry.Callsign.Substring(index + 1);
                //temp = temp.ToLower();
                //index = temp.IndexOf('.');
                //if (index < 0)
                //    return false;

                //temp = temp.Substring(0, index);
                //bool result = ValidateBBS(temp);        // BBS in address
                //result &= ValidateBBS(addressBookEntry.BBSPrimary.ToLower());       // Primary BBS
                ////result &= ValidateBBS(addressBookEntry.BBSSecondary.ToLower());     // Secondary BBS can be undefined
                //if (!result)
                //    return false;

                addressBookDictionary.Add(addressBookEntry.Callsign, addressBookEntry);
                // Insert in userAddressBook
                UserAddressList.Add(addressBookEntry);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddAddressAsync(string address, string prefix = "", string bbsPrimary = "", string bbsSecondary = "", bool primaryActive = true)
        {
            // extract callsign
            int index = address.IndexOf('@');
            if (index < 0)
                return;

            Dictionary<string, AddressBookEntry> addressDictionary = AddressBook.Instance.AddressBookDictionary;
            string callsign = address.Substring(0, index).ToUpper();
            bool entryFound = addressDictionary.TryGetValue(callsign, out AddressBookEntry addressBookEntry);
            if (!entryFound)
            {
                string temp = address.Substring(index + 1);
                temp = temp.ToLower();
                index = temp.IndexOf('.');
                if (index < 0)
                    return;

                temp = temp.Substring(0, index);
                if (temp.StartsWith("w") && temp.EndsWith("xsc"))
                {
                    bbsPrimary = temp.ToUpper();
                }
                AddressBookEntry entry = new AddressBookEntry()
                {
                    Callsign = callsign,
                    NameDetail = address,
                    Prefix = prefix,
                    BBSPrimary = bbsPrimary,
                    BBSSecondary = "",
                    BBSPrimaryActive = primaryActive
                };
                addressDictionary.Add(callsign, entry);
                // Insert in userAddressBook
                UserAddressList.Add(entry);
                //SaveAsync();
            }
        }

        public void DeleteAddress(AddressBookEntry addressBookEntry)
        {
            UserAddressList.Remove(addressBookEntry);
            //SaveAsync();
            AddressBook.Instance.AddressBookDictionary.Remove(addressBookEntry.Callsign);
        }

    }

}
