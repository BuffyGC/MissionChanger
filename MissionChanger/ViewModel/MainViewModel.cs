using MissionChanger.Classes;
using MissionChanger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MissionChanger.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public string BaseTitle { get => baseTitle; set => SetField(ref baseTitle, value); }
        private string baseTitle;

        public string CommunityFolder { get => communityFolder; set => SetField(ref communityFolder, value); }
        private string communityFolder;

        public RelayCommand CommandSelectPath { get; private set; }
        public RelayCommand CommandLoadAddOns { get; private set; }
        public RelayCommand CommandSetAircraft { get; private set; }

        public MissionViewModel MissionViewModel { get => missionViewModel; set => SetField(ref missionViewModel, value); }
        private MissionViewModel missionViewModel;

        public AircraftsViewModel AircraftsViewModel  { get => aircraftsViewModel; set => SetField(ref aircraftsViewModel, value); }
        private AircraftsViewModel aircraftsViewModel;

        public MainViewModel()
        {
            var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            //object[] attribs = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTrademarkAttribute), true);

#if DEBUG
            baseTitle = string.Format("{0} {1} Developer proudly presented by {2}", 
                versionInfo.ProductName, 
                versionInfo.ProductVersion, 
                versionInfo.LegalTrademarks);
#else
            baseTitle = string.Format("{0} {1} proudly presented by {2}", 
                versionInfo.ProductName, 
                versionInfo.ProductVersion, 
                versionInfo.LegalTrademarks);
#endif
            CommunityFolder = CommFolderDetector.GetCommFolder();
            MissionViewModel = new MissionViewModel();
            AircraftsViewModel = new AircraftsViewModel();

            CommandSelectPath = new RelayCommand(OnSelectPath);
            CommandLoadAddOns = new RelayCommand(OnLoadAddons);
            CommandSetAircraft = new RelayCommand(OnSetAircraft);

            MissionViewModel.PropertyChanged += Mission_PropertyChanged;
        }

        private void Mission_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MissionViewModel.SelectedMission) && MissionViewModel.SelectedMission != null)
            {
                Aircraft aircraft = AircraftsViewModel.Aircrafts.FirstOrDefault(a => a.Name.Equals(MissionViewModel.SelectedMission.Aircraft, StringComparison.OrdinalIgnoreCase));

                if (aircraft != null)
                    AircraftsViewModel.SelectedAircraft = aircraft;
            }
        }

        private void OnLoadAddons(object obj)
        {
            try
            {
                UIServices.SetBusyState();
                ReadAircrafts();
                ReadMissions();
            }
            catch (Exception ex)
            {
                MessageBox.Show("something went wrong reading the community or official folder" +
                    Environment.NewLine +
                    ex.Message);
            }
        }

        private void ReadAircrafts()
        {
            string p = communityFolder.Replace("Community", "");

            AircraftsViewModel.LoadAircrafts(p);
        }


        private void ReadMissions()
        {
            MissionViewModel.LoadMissions(communityFolder);
        }


        private void OnSetAircraft(object obj)
        {
            try
            {
                MissionViewModel.SetAircraft(AircraftsViewModel.SelectedAircraft);

            }
            catch (Exception ex)
            {
                MessageBox.Show("something went wrong setting the aircraft" +
                    Environment.NewLine +
                    ex.Message);

            }
        }


        private void OnSelectPath(object obj)
        {
            try
            {
                System.Windows.Forms.FolderBrowserDialog openFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog
                {
                    ShowNewFolderButton = false,
                    Description = "Add-On-Path",
                    SelectedPath = CommunityFolder,
                    RootFolder = Environment.SpecialFolder.MyComputer
                };

                System.Windows.Forms.DialogResult dlgr = openFolderBrowserDialog.ShowDialog();

                if (dlgr == System.Windows.Forms.DialogResult.OK)
                {
                    CommunityFolder = openFolderBrowserDialog.SelectedPath;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("something went wrong selecting the community folder" +
                    Environment.NewLine +
                    ex.Message);

            }
        }
    }
}
