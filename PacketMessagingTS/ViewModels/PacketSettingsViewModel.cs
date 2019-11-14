using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

using SharedCode;

namespace PacketMessagingTS.ViewModels
{
    public class PacketSettingsViewModel : BaseViewModel
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<PacketSettingsViewModel>();
        private static LogHelper _logHelper = new LogHelper(log);

        Profile _SavedProfile;

        public PacketSettingsViewModel()
        {

        }

        public override void ResetChangedProperty()
        {
            base.ResetChangedProperty();
            IsAppBarSaveEnabled = false;
        }

        //private List<Profile> observableProfileCollection;
        public List<Profile> ObservableProfileCollection
        {
            get => ProfileArray.Instance.ProfileList;
        }

        private int profileSelectedIndex;
        public int ProfileSelectedIndex
        {
            get => GetProperty(ref profileSelectedIndex);
            set
            {
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
                BBSFromSelectedProfile = BBSDefinitions.Instance.BBSDataList.Where(bbs => bbs.Name == currentProfile.BBS).FirstOrDefault();
                //Name = currentProfile.Name;
                TNC = currentProfile.TNC;
                BBS = currentProfile.BBS;
                DefaultTo = currentProfile.SendTo;
                if (currentProfile.TNC.Contains(SharedData.EMail))
                {
                    // Update SMTP data if using Email for sending
                    TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList.Where(tnc => tnc.Name.Contains(SharedData.EMail)).FirstOrDefault();
                    int index = EmailAccountArray.Instance.GetSelectedIndexFromEmailUserName(tncDevice.MailUserName);
                    Singleton<TNCSettingsViewModel>.Instance.MailAccountSelectedIndex = index;
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
                    IdentityViewModel instance = Singleton<IdentityViewModel>.Instance;
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

        private string tnc;
        public string TNC
        {
            get => tnc;
            set
            {
                //if (value is null)
                //    return;

                SetProperty(ref tnc, value);

                //bool changed = ProfileArray.Instance.ProfileList[ProfileSelectedIndex].TNC != tnc;
                //IsAppBarSaveEnabled = SaveEnabled(changed);
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
            }
        }

        public ObservableCollection<BBSData> BBSDataCollection
        {
            get => new ObservableCollection<BBSData>(BBSDefinitions.Instance.BBSDataList);
            //{
            //    ObservableCollection<BBSData> bbsDataCollection = new ObservableCollection<BBSData>(BBSDefinitions.Instance.BBSDataList);
            //    BBSData bbsData = new BBSData();
            //    bbsData.Name = "Get from Address Book";
            //    bbsDataCollection.Add(bbsData);
            //    return bbsDataCollection;
            //}
        }

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
                currentBBS = value;
                BBSDescription = currentBBS?.Description;
                BBSFrequency1 = currentBBS?.Frequency1;
                BBSFrequency2 = currentBBS?.Frequency2;

                bool changed = BBSFromSelectedProfile?.Name != currentProfile.BBS;            
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

                //bool changed = CurrentProfile.Subject != defaultSubject;
                //IsAppBarSaveEnabled = SaveEnabled(changed);
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

        private int firstMessageNumber;
        public int FirstMessageNumber
        {
            get
            {
                bool found = App.Properties.TryGetValue("MessageNumber", out object first);
                if (!found)
                {
                    App.Properties["MessageNumber"] = 100;
                }
                firstMessageNumber = Convert.ToInt32(App.Properties["MessageNumber"]);
                return firstMessageNumber;
            }
            set
            {
                Utilities.MarkMessageNumberAsUsed(value);
                SetProperty(ref firstMessageNumber, value);
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
    }
}
