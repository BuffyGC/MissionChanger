using MissionChanger.Classes;
using MissionChanger.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public string FSBaseFolder { get => _FSBaseFolder; set => SetField(ref _FSBaseFolder, value); }
        private string _FSBaseFolder;

        public RelayCommand CommandSelectPath { get; private set; }
        public RelayCommand CommandLoadAddOns { get; private set; }
        public RelayCommand CommandSaveMissions { get; private set; }
        public RelayCommand CommandOpenInExplorer { get; private set; }

        public MissionViewModel MissionViewModel { get => missionViewModel; set => SetField(ref missionViewModel, value); }
        private MissionViewModel missionViewModel;


        public AircraftsViewModel AircraftsViewModel  { get => aircraftsViewModel; set => SetField(ref aircraftsViewModel, value); }
        private AircraftsViewModel aircraftsViewModel;

        public bool Loaded { get => loaded; set => SetField(ref loaded, value); }
        private bool loaded = false;
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
            FSBaseFolder = CommFolderDetector.GetFSBaseFolder();

            CommandSelectPath = new RelayCommand(OnSelectPath);
            CommandLoadAddOns = new RelayCommand(OnLoadAddons);
            CommandSaveMissions = new RelayCommand(OnSaveMissions);
            CommandOpenInExplorer = new RelayCommand(OnOpenInExplorer);

            MissionViewModel = new MissionViewModel();
            AircraftsViewModel = new AircraftsViewModel();

            MissionViewModel.PropertyChanged += Mission_PropertyChanged;
            AircraftsViewModel.PropertyChanged += AircraftsViewModel_PropertyChanged;

            if (string.IsNullOrWhiteSpace(FSBaseFolder))
                CommandSelectPath.Execute(null);

            //if (!string.IsNullOrWhiteSpace(FSBaseFolder))
            //    CommandLoadAddOns.Execute(null);
        }

        private void OnOpenInExplorer(object obj)
        {
            if (MissionViewModel.SelectedMission != null)
            {
                string path = "explorer.exe";
                string file = MissionViewModel.SelectedMission.Filename;

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = path;

                if (!string.IsNullOrEmpty(file))
                {
                    psi.Arguments = $"/select, \"{file}\"";
                }

                Process.Start(psi);
            }
        }

        private void AircraftsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AircraftsViewModel.SelectedAircraft) 
                && AircraftsViewModel.SelectedAircraft != null)
            {
                if (MissionViewModel.SelectedMission != null)
                    MissionViewModel.SelectedMission.Aircraft = AircraftsViewModel.SelectedAircraft.Name;
            }
            else
            if (e.PropertyName == nameof(AircraftsViewModel.ShowAll))
            {
                AircraftsViewModel.RefreshFilter();
            }
        }

        private void Mission_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MissionViewModel.SelectedMission) && MissionViewModel.SelectedMission != null)
            {
                AircraftModel aircraft = AircraftsViewModel.Aircrafts.FirstOrDefault(a => a.Name.Equals(MissionViewModel.SelectedMission.Aircraft, StringComparison.OrdinalIgnoreCase));

                AircraftsViewModel.SelectedAircraft = aircraft;
                AircraftsViewModel.RefreshFilter();
            }
        }

        private void OnLoadAddons(object obj)
        {
            try
            {
                UIServices.SetBusyState();

                MissionViewModel.PropertyChanged -= Mission_PropertyChanged;
                AircraftsViewModel.PropertyChanged -= AircraftsViewModel_PropertyChanged;

                MissionViewModel = new MissionViewModel();
                AircraftsViewModel = new AircraftsViewModel();

                MissionViewModel.PropertyChanged += Mission_PropertyChanged;
                AircraftsViewModel.PropertyChanged += AircraftsViewModel_PropertyChanged;

                ReadAircrafts();
                ReadMissions();

                Loaded = MissionViewModel != null && MissionViewModel.Missions.Count() > 0 && AircraftsViewModel != null && AircraftsViewModel.Aircrafts.Count > 0;
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
            string p = FSBaseFolder.Replace("Community", "");

            AircraftsViewModel.LoadAircrafts(p);
        }


        private void ReadMissions()
        {
            MissionViewModel.LoadMissions(FSBaseFolder);
        }


        private void OnSaveMissions(object obj)
        {
            try
            {
                MissionViewModel.SaveMissions();

            }
            catch (Exception ex)
            {
                MessageBox.Show("something went wrong ssaving your missions" +
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
                    SelectedPath = FSBaseFolder,
                    RootFolder = Environment.SpecialFolder.MyComputer
                };

                System.Windows.Forms.DialogResult dlgr = openFolderBrowserDialog.ShowDialog();

                if (dlgr == System.Windows.Forms.DialogResult.OK)
                {
                    FSBaseFolder = openFolderBrowserDialog.SelectedPath;
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
