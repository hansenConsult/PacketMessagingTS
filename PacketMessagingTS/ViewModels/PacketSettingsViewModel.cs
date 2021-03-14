using System;
using System.Collections.ObjectModel;
using System.Linq;

using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

using SharedCode;
using SharedCode.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class PacketSettingsViewModel : BaseViewModel
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
                Set(ref _ProfileNameVisibility, value);
                NewProfileNameVisibility = !value;
            }
        }

        private bool _NewProfileNameVisibility = false;
        public bool NewProfileNameVisibility
        {
            get => _NewProfileNameVisibility;
            set
            {
                Set(ref _NewProfileNameVisibility, value);
            }
        }

        private string _NewProfileName;
        public string NewProfileName
        {
            get => _NewProfileName;
            set => Set(ref _NewProfileName, value);
        }

        private int profileSelectedIndex;
        public int ProfileSelectedIndex
        {
            get => GetProperty(ref profileSelectedIndex);
            set
            {
                if (ProfileArray.Instance.ProfileList is null)
                {
                    SetProperty(ref profileSelectedIndex, -1, true);
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
                    SetProperty(ref profileSelectedIndex, value, true);
                }
                else
                {
                    _logHelper.Log(LogLevel.Error, $"ProfileSelectedIndex = {value}");
                    SetProperty(ref profileSelectedIndex, 0, true);
                }
                CurrentProfile = ProfileArray.Instance.ProfileList[profileSelectedIndex];
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

        private Profile currentProfile;
        public Profile CurrentProfile
        {
            get
            {
                if (currentProfile is null)
                {
                    ProfileSelectedIndex = Utilities.GetProperty(nameof(ProfileSelectedIndex));
                }
                return currentProfile;
            }
            set
            {
                currentProfile = value;

                TNCFromSelectedProfile = TNCDeviceArray.Instance.TNCDeviceList.Where(tnc => tnc.Name == currentProfile.TNC).FirstOrDefault();
                BBSFromSelectedProfile = BBSDefinitions.Instance.BBSDataArray.Where(bbs => bbs.Name == currentProfile.BBS).FirstOrDefault();
                //Name = currentProfile.Name;
                TNC = currentProfile.TNC;
                BBS = currentProfile.BBS;
                DefaultTo = currentProfile.SendTo;
                if (currentProfile.TNC.Contains(PublicData.EMail))
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
            set => Set(ref _TNCDeviceListSource, value);
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

        private string tnc;
        public string TNC
        {
            get => tnc;
            set
            {
                if (value is null)
                    return;

                SetProperty(ref tnc, value);

                if (tnc.Contains(PublicData.EMail))
                {
                    BBS = "";
                }
                else
                {
                    BBS = CurrentProfile?.BBS;
                }
                UpdateProfileSaveButton(_SavedProfile.TNC, tnc);
            }
        }

        private TNCDevice currentTNC;
        public TNCDevice TNCFromSelectedProfile
        {
            get
            {
                if (currentTNC is null)
                {
                    ProfileSelectedIndex = Utilities.GetProperty(nameof(ProfileSelectedIndex));
                }
                return currentTNC;
            }
            set
            {
                //Set(ref currentTNC, value);
                currentTNC = value;
                if (currentTNC != null && currentTNC.Name.Contains(PublicData.EMail))
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

        private BBSData currentBBS;
        public BBSData BBSFromSelectedProfile
        {
            get => currentBBS;
            set
            {
                Set(ref currentBBS, value);
                BBSDescription = currentBBS?.Description;
                BBSFrequency1 = currentBBS?.Frequency1;
                BBSFrequency2 = currentBBS?.Frequency2;
                BBSFrequency3 = currentBBS?.Frequency3;

                bool changed = BBSFromSelectedProfile?.Name != currentProfile?.BBS;            
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string bbsDescription;
        public string BBSDescription
        {
            get => bbsDescription;
            set => SetProperty(ref bbsDescription, value);
        }
        
        private string bbsFrequency1;
        public string BBSFrequency1
        {
            get => bbsFrequency1;
            set => SetProperty(ref bbsFrequency1, value);
        }

        private string bbsFrequency2;
        public string BBSFrequency2
        {
            get => bbsFrequency2;
            set => SetProperty(ref bbsFrequency2, value);
        }

        private string bbsFrequency3;
        public string BBSFrequency3
        {
            get => bbsFrequency3;
            set => SetProperty(ref bbsFrequency3, value);
        }

        private string defaultTo;
        public string DefaultTo
        {
            get => defaultTo;
            set
            {
                SetProperty(ref defaultTo, value);

                bool changed = CurrentProfile.SendTo != defaultTo;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string defaultSubject;
        public string DefaultSubject
        {
            get => defaultSubject;
            set
            {
                SetProperty(ref defaultSubject, value);

                UpdateProfileSaveButton(_SavedProfile.Subject, defaultSubject);
            }
        }

        private string defaultMessage;
        public string DefaultMessage
        {
            get => defaultMessage;
            set
            {
                SetProperty(ref defaultMessage, value);

                //bool changed = CurrentProfile.Message != defaultMessage;
                //IsAppBarSaveEnabled = SaveEnabled(changed);
                UpdateProfileSaveButton(_SavedProfile.Message, defaultMessage);
            }
        }
    
        private bool displayProfileOnStart;
        public bool DisplayProfileOnStart
        {
            get => GetProperty(ref displayProfileOnStart);
            set => SetProperty(ref displayProfileOnStart, value, true);
        }

        private bool isDrillTraffic = false;
        public bool IsDrillTraffic
        {
            get => isDrillTraffic;
            set => Set(ref isDrillTraffic, value);
        }

        private int firstMessageNumber;
        public int FirstMessageNumber
        {
            get
            {
                GetProperty(ref firstMessageNumber);
                int messageNo = Utilities.FindHighestUsedMesageNumber();
                if (messageNo > firstMessageNumber)
                {
                    firstMessageNumber = messageNo + 1;
                }
                //bool found = App.Properties.TryGetValue("MessageNumber", out object first);
                //if (!found)
                //{
                //    //App.Properties["MessageNumber"] = 100;
                //    FirstMessageNumber = Utilities.FindHighestUsedMesageNumber() + 1;
                //}
                //firstMessageNumber = Convert.ToInt32(App.Properties["MessageNumber"]);
                return firstMessageNumber;
            }
            set
            {
                //Utilities.MarkMessageNumberAsUsed(value);
                SetProperty(ref firstMessageNumber, value, true);
            }
        }

        private string areaString = "XSCPERM, XSCEVENT";
        public string AreaString
        {
            get => GetProperty(ref areaString);
            set => SetProperty(ref areaString, value, true);
        }

        private bool sendReceipt;
        public bool SendReceipt
        {
            get => GetProperty(ref sendReceipt);
            set => SetProperty(ref sendReceipt, value, true);
        }

        private bool forceReadBulletins;
        public bool ForceReadBulletins
        {
            get => forceReadBulletins;
            set => SetProperty(ref forceReadBulletins, value);
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
