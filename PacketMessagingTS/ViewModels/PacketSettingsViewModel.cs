using System;
using System.Collections.ObjectModel;
using System.Linq;

using MetroLog;

using Microsoft.Toolkit.Mvvm.Input;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

using SharedCode;
using SharedCode.Helpers;


namespace PacketMessagingTS.ViewModels
{
    public class PacketSettingsViewModel : ViewModelBase
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<PacketSettingsViewModel>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        public static PacketSettingsViewModel Instance { get; } = new PacketSettingsViewModel();

        Profile _SavedProfile;

        public PacketSettingsViewModel()
        {

        }

        public override void ResetChangedProperty()
        {
            base.ResetChangedProperty();
            IsAppBarSaveEnabled = false;
        }

        public ObservableCollection<Profile> ObservableProfileCollection
        {
            get
            {
                if (ProfileArray.Instance.ProfileList is null)
                {
                    return null;
                }
                return new ObservableCollection<Profile>(ProfileArray.Instance.ProfileList);
            }
        }
        //private List<Profile> _observableProfileCollection;
        //public List<Profile> ObservableProfileCollection
        //{
        //    get => ProfileArray.Instance.ProfileList;
        //    set => Set(ref _observableProfileCollection, value);
        //}

        private bool _ProfileNameVisibility = true;
        public bool ProfileNameVisibility
        {
            get => _ProfileNameVisibility;
            set
            {
                SetProperty(ref _ProfileNameVisibility, value);
                NewProfileNameVisibility = !value;
            }
        }

        private bool _NewProfileNameVisibility = false;
        public bool NewProfileNameVisibility
        {
            get => _NewProfileNameVisibility;
            set => SetProperty(ref _NewProfileNameVisibility, value);
        }

        private string _NewProfileName;
        public string NewProfileName
        {
            get => _NewProfileName;
            set => SetProperty(ref _NewProfileName, value);
        }

        private int _profileSelectedIndex;
        public int ProfileSelectedIndex
        {
            get => GetProperty(ref _profileSelectedIndex);
            set
            {
                if (ProfileArray.Instance.ProfileList is null)
                {
                    SetPropertyPrivate(ref _profileSelectedIndex, -1, true);
                    return;
                }
                if (value >= 0 && value < ProfileArray.Instance.ProfileList.Count)
                {
                    _SavedProfile = ProfileArray.Instance.ProfileList[value];
                }
                else if (value >= ProfileArray.Instance.ProfileList.Count)
                {
                    _SavedProfile = ProfileArray.Instance.ProfileList[0];
                }

                if (value >= 0 && value < ProfileArray.Instance.ProfileList.Count)
                {
                    SetPropertyPrivate(ref _profileSelectedIndex, value, true);
                }
                else
                {
                    _logHelper.Log(LogLevel.Error, $"ProfileSelectedIndex = {value}");
                    SetPropertyPrivate(ref _profileSelectedIndex, 0, true);
                }
                CurrentProfile = ProfileArray.Instance.ProfileList[_profileSelectedIndex];
            }
        }

        private DateTime GetNextNetDate(string netDay)
        {
            DayOfWeek dayOfWeek;
            if (netDay == "PKTMON")
            {
                dayOfWeek = DayOfWeek.Monday;
            }
            else
                dayOfWeek = DayOfWeek.Tuesday;

            //switch (netDay)
            //{
            //    case "PKTMON":
            //        dayOfWeek = DayOfWeek.Monday;
            //        break;
            //    case "PKTTUE":
            //        dayOfWeek = DayOfWeek.Tuesday;
            //        break;
            //    case "PKTWED":
            //        dayOfWeek = DayOfWeek.Wednesday;
            //        break;
            //    case "PKTTHU":
            //        dayOfWeek = DayOfWeek.Thursday;
            //        break;
            //    case "PKTFRI":
            //        dayOfWeek = DayOfWeek.Friday;
            //        break;
            //    case "PKTSAT":
            //        dayOfWeek = DayOfWeek.Saturday;
            //        break;
            //    case "PKTSUN":
            //        dayOfWeek = DayOfWeek.Sunday;
            //        break;
            //}

            DateTime now = DateTime.Now;
            DateTime date = now;
            while (date.DayOfWeek != dayOfWeek)
            {
                date = date.Add(new TimeSpan(24, 0, 0));
            };
            
            return date;
        }

        private Profile _currentProfile;
        public Profile CurrentProfile
        {
            get
            {
                if (_currentProfile is null)
                {
                    ProfileSelectedIndex = Utilities.GetProperty(nameof(ProfileSelectedIndex));
                }
                return _currentProfile;
            }
            set
            {
                _currentProfile = value;

                TNCFromSelectedProfile = TNCDeviceArray.Instance.TNCDeviceList.Where(tnc => tnc.Name == _currentProfile.TNC).FirstOrDefault();
                BBSFromSelectedProfile = BBSDefinitions.Instance.BBSDataArray.Where(bbs => bbs.Name == _currentProfile.BBS).FirstOrDefault();
                //Name = currentProfile.Name;
                TNC = _currentProfile.TNC;
                BBS = _currentProfile.BBS;
                DefaultTo = _currentProfile.SendTo;
                if (_currentProfile.TNC.Contains(PublicData.EMail))
                {
                    // Update SMTP data if using Email for sending
                    TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList.Where(tnc => tnc.Name.Contains(PublicData.EMail)).FirstOrDefault();
                    int index = EmailAccountArray.Instance.GetSelectedIndexFromEmailUserName(tncDevice.MailUserName);
                    TNCSettingsViewModel.Instance.MailAccountSelectedIndex = index;
                }
                if (!string.IsNullOrEmpty(DefaultTo) && (DefaultTo.Contains("PKTMON") || DefaultTo.Contains("PKTTUE")))
                {
                    DateTime netTime;
                    if (DefaultTo.Contains("PKTMON"))
                    {
                        netTime = GetNextNetDate("PKTMON");
                    }
                    else if (DefaultTo.Contains("PKTTUE"))
                    {
                        netTime = GetNextNetDate("PKTTUE");
                    }
                    else
                    {
                        return;
                    }
                    IdentityViewModel instance = IdentityViewModel.Instance;
                    DefaultSubject = $"Practice {instance.UserCallsign}, {instance.UserFirstName}, {instance.UserCity}, " +
                                     $"{netTime.Month:d2}/{netTime.Day:d2}/{netTime.Year:d4}";
                }
                else
                {
                    DefaultSubject = CurrentProfile.Subject;
                }
                DefaultMessage = CurrentProfile.Message;

                Utilities.SetApplicationTitle();

                ResetChangedProperty();
            }
        }

        //private string name;
        //public string Name
        //{
        //    get => name;
        //    set => SetProperty(ref name, value);
        //}

        private ObservableCollection<TNCDevice> _TNCDeviceListSource;
        public ObservableCollection<TNCDevice> TNCDeviceListSource
        {
            get => new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
            set => SetProperty(ref _TNCDeviceListSource, value);
        }

        private void UpdateProfileSaveButton<T>(T savedProperty, T newProperty)
        {
            bool changed = !Equals(savedProperty, newProperty);
            IsAppBarSaveEnabled = SaveEnabled(changed);
        }

        //private string _ProfileName;
        //public string ProfileName
        //{
        //    get => _ProfileName;
        //    set => Set(ref _ProfileName, value);
        //}

        private string _tnc;
        public string TNC
        {
            get => _tnc;
            set
            {
                if (value is null)
                    return;

                SetProperty(ref _tnc, value);

                if (_tnc.Contains(PublicData.EMail))
                {
                    BBS = "";
                }
                else
                {
                    BBS = CurrentProfile?.BBS;
                }
                UpdateProfileSaveButton(_SavedProfile.TNC, _tnc);
            }
        }

        private TNCDevice _currentTNC;
        public TNCDevice TNCFromSelectedProfile
        {
            get
            {
                if (_currentTNC is null)
                {
                    ProfileSelectedIndex = Utilities.GetProperty(nameof(ProfileSelectedIndex));
                }
                return _currentTNC;
            }
            set
            {
                //Set(ref currentTNC, value);
                _currentTNC = value;
                if (_currentTNC != null && _currentTNC.Name.Contains(PublicData.EMail))
                {
                    BBS = "";
                }
                //else
                //{
                //    BBS = CurrentProfile.BBS;
                //}
            }
        }

        public ObservableCollection<BBSData> BBSDataCollection => new ObservableCollection<BBSData>(BBSDefinitions.Instance.BBSDataArray);
        //{
            //get => new ObservableCollection<BBSData>(BBSDefinitions.Instance.BBSDataList);
            //{
            //    ObservableCollection<BBSData> bbsDataCollection = new ObservableCollection<BBSData>(BBSDefinitions.Instance.BBSDataList);
            //    BBSData bbsData = new BBSData();
            //    bbsData.Name = "Get from Address Book";
            //    bbsDataCollection.Add(bbsData);
            //    return bbsDataCollection;
            //}
        //}

        private string bbsSelectedValue;
        public string BBS
        {
            get => bbsSelectedValue;
            set
            {
                SetProperty(ref bbsSelectedValue, value);

                //bool changed = CurrentProfile.BBS != bbsSelectedValue;
                //IsAppBarSaveEnabled = SaveEnabled(changed);
                UpdateProfileSaveButton(_SavedProfile.BBS, bbsSelectedValue);
            }
        }

        private BBSData _currentBBS;
        public BBSData BBSFromSelectedProfile
        {
            get => _currentBBS;
            set
            {
                SetProperty(ref _currentBBS, value);
                BBSDescription = _currentBBS?.Description;
                BBSFrequency1 = _currentBBS?.Frequency1;
                BBSFrequency2 = _currentBBS?.Frequency2;
                BBSFrequency3 = _currentBBS?.Frequency3;

                bool changed = BBSFromSelectedProfile?.Name != _currentProfile?.BBS;            
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string _bbsDescription;
        public string BBSDescription
        {
            get => _bbsDescription;
            set => SetProperty(ref _bbsDescription, value);
        }
        
        private string _bbsFrequency1;
        public string BBSFrequency1
        {
            get => _bbsFrequency1;
            set => SetProperty(ref _bbsFrequency1, value);
        }

        private string _bbsFrequency2;
        public string BBSFrequency2
        {
            get => _bbsFrequency2;
            set => SetProperty(ref _bbsFrequency2, value);
        }

        private string _bbsFrequency3;
        public string BBSFrequency3
        {
            get => _bbsFrequency3;
            set => SetProperty(ref _bbsFrequency3, value);
        }

        private string _defaultTo;
        public string DefaultTo
        {
            get => _defaultTo;
            set
            {
                SetProperty(ref _defaultTo, value);

                bool changed = CurrentProfile.SendTo != _defaultTo;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string _defaultSubject;
        public string DefaultSubject
        {
            get => _defaultSubject;
            set
            {
                SetProperty(ref _defaultSubject, value);

                UpdateProfileSaveButton(_SavedProfile.Subject, _defaultSubject);
            }
        }

        private string _defaultMessage;
        public string DefaultMessage
        {
            get => _defaultMessage;
            set
            {
                SetProperty(ref _defaultMessage, value);

                //bool changed = CurrentProfile.Message != defaultMessage;
                //IsAppBarSaveEnabled = SaveEnabled(changed);
                UpdateProfileSaveButton(_SavedProfile.Message, _defaultMessage);
            }
        }
    
        private bool _displayProfileOnStart;
        public bool DisplayProfileOnStart
        {
            get => GetProperty(ref _displayProfileOnStart);
            set => SetPropertyPrivate(ref _displayProfileOnStart, value, true);
        }

        private bool _isDrillTraffic = false;
        public bool IsDrillTraffic
        {
            get => _isDrillTraffic;
            set => SetProperty(ref _isDrillTraffic, value);
        }

        private int _firstMessageNumber;
        public int FirstMessageNumber
        {
            get
            {
                GetProperty(ref _firstMessageNumber);
                int messageNo = Utilities.FindHighestUsedMesageNumber();
                if (messageNo > _firstMessageNumber)
                {
                    _firstMessageNumber = messageNo + 1;
                }
                //bool found = App.Properties.TryGetValue("MessageNumber", out object first);
                //if (!found)
                //{
                //    //App.Properties["MessageNumber"] = 100;
                //    FirstMessageNumber = Utilities.FindHighestUsedMesageNumber() + 1;
                //}
                //firstMessageNumber = Convert.ToInt32(App.Properties["MessageNumber"]);
                return _firstMessageNumber;
            }
            set
            {
                //Utilities.MarkMessageNumberAsUsed(value);
                SetPropertyPrivate(ref _firstMessageNumber, value, true);
            }
        }

        private string _areaString = "XSCPERM, XSCEVENT";
        public string AreaString
        {
            get => GetProperty(ref _areaString);
            set => SetPropertyPrivate(ref _areaString, value, true);
        }

        private bool _sendReceipt;
        public bool SendReceipt
        {
            get => GetProperty(ref _sendReceipt);
            set => SetPropertyPrivate(ref _sendReceipt, value, true);
        }

        private bool _forceReadBulletins;
        public bool ForceReadBulletins
        {
            get => _forceReadBulletins;
            set => SetProperty(ref _forceReadBulletins, value);
        }

        private RelayCommand _ProfileSettingsAddCommand;
        public RelayCommand ProfileSettingsAddCommand => _ProfileSettingsAddCommand ?? (_ProfileSettingsAddCommand = new RelayCommand(ProfileSettingsAdd));
        private void ProfileSettingsAdd()
        {
            ProfileNameVisibility = false;

            IsAppBarSaveEnabled = true;
        }

        private RelayCommand _PacketSettingsSaveCommand;
        public RelayCommand PacketSettingsSaveCommand => _PacketSettingsSaveCommand ?? (_PacketSettingsSaveCommand = new RelayCommand(PacketSettingsSave));

        public async void PacketSettingsSave()
        {
            //int index = comboBoxProfiles.SelectedIndex;
            if (!ProfileNameVisibility)
            {
                Profile newProfile = new Profile()
                {
                    Name = _NewProfileName,
                    //BBS = comboBoxBBS.SelectedValue as string,
                    BBS = BBS,
                    //TNC = comboBoxTNCs.SelectedValue as string,
                    TNC = TNC,
                    SendTo = DefaultTo,
                    Subject = DefaultSubject,
                    Message = DefaultMessage,

                };
                if (newProfile.TNC.Contains(PublicData.EMail))
                {
                    //comboBoxBBS.SelectedIndex = -1;
                    BBS = "";
                    newProfile.BBS = "";
                }
                ProfileArray.Instance.ProfileList.Add(newProfile);
                ObservableProfileCollection.Add(newProfile);
                await ProfileArray.Instance.SaveAsync();

                ProfileSelectedIndex = ProfileArray.Instance.ProfileList.Count - 1;

                //comboBoxProfiles.Visibility = Visibility.Visible;
                //textBoxNewProfileName.Visibility = Visibility.Collapsed;
                ProfileNameVisibility = true;
                //index = ProfileArray.Instance.ProfileList.Count - 1;
            }
            else
            {
                //Profile profile = comboBoxProfiles.Items[ProfileSelectedIndex] as Profile;
                Profile profile = CurrentProfile;

                profile.Name = CurrentProfile.Name;
                profile.BBS = BBS;
                profile.TNC = TNC;
                profile.SendTo = DefaultTo;
                profile.Subject = DefaultSubject;
                profile.Message = DefaultMessage;

                //ProfileArray.Instance.ProfileList[ProfileArray.Instance.ProfileList.IndexOf(profile)] = profile;
                ProfileArray.Instance.ProfileList[ProfileSelectedIndex] = profile;
            }
            //if (ProfileArray.Instance.ProfileList[ProfileSelectedIndex].TNC.Contains(SharedData.EMail))
            //{

            //    comboBoxBBS.SelectedIndex = -1;
            //    (comboBoxProfiles.Items[comboBoxProfiles.SelectedIndex] as Profile).BBS = "";
            //}

            await ProfileArray.Instance.SaveAsync();

            Utilities.SetApplicationTitle();

            //_PacketSettingsViewmodel.ResetChangedProperty();
            IsAppBarSaveEnabled = false;
            //comboBoxProfiles.SelectedIndex = index;
        }

        private RelayCommand _ProfileSettingsDeleteCommand;
        public RelayCommand ProfileSettingsDeleteCommand => _ProfileSettingsDeleteCommand ?? (_ProfileSettingsDeleteCommand = new RelayCommand(ProfileSettingsDelete));
        private void ProfileSettingsDelete()
        {
            ProfileArray.Instance.ProfileList.Remove(CurrentProfile);
            ObservableProfileCollection.Remove(CurrentProfile);

            ProfileSelectedIndex = Math.Max(ProfileSelectedIndex - 1, 0);
            IsAppBarSaveEnabled = true;
        }

    }
}
