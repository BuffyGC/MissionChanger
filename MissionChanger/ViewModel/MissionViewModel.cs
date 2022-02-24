using MissionChanger.Classes;
using MissionChanger.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;

namespace MissionChanger.ViewModel
{
    public class MissionViewModel : BaseViewModel
    {
        public ObservableCollection<Mission> Missions { get => missions; set => SetField(ref missions, value); }
        private ObservableCollection<Mission> missions = new ObservableCollection<Mission>();

        public Mission SelectedMission { get => selectedMission; set => SetField2(ref selectedMission, value, nameof(IsMissionSelected)); }
        private Mission selectedMission;

        public bool IsMissionSelected => SelectedMission != null;

        public RelayCommand<Mission> SelectedItemChanged { get; private set; } = null;


        public MissionViewModel()
        {
            SelectedItemChanged = new RelayCommand<Mission>( c => SelectedMission = c );
        }

        internal void LoadMissions(string communityFolder)
        {
            try
            {
                IEnumerable<string> fltFiles = System.IO.Directory.EnumerateFiles(LongFile.GetWin32LongPath(communityFolder), "*.flt", System.IO.SearchOption.AllDirectories);

                foreach (string fltFile in fltFiles)
                {
                    INI INI = new INI(fltFile);

                    Mission mission = new Mission();
                    mission.MissionType = INI.Read("MissionType", "Main");

                    if (mission.MissionType.Equals("BushTrip", StringComparison.OrdinalIgnoreCase))
                    {
                        mission.Name = Path.GetFileNameWithoutExtension(fltFile);
                        mission.Manifest = GetManifest(fltFile);
                        mission.IsProtected = mission.Manifest.total_package_size?.Length > 0;

                        mission.HasWeatherFile = LongFile.Exists(Path.Combine(Path.GetDirectoryName(fltFile), "Weather.WPR"));

                        mission.Title = mission.Manifest != null ? $"{mission.Manifest.title} ({mission.Name})" : mission.Name;
                        mission.Aircraft = INI.Read("Sim", "Sim.0");
                        mission.Filename = LongFile.RemoveWin32LongPath(fltFile);
                        mission.DateTime = ReadDateTime(INI);

                        ReadWeather(INI, mission);

                        string backupname = GetBackupFilename(fltFile);

                        if (LongFile.Exists(backupname))
                            mission.OriginalAircraft = new INI(backupname).Read("Sim", "Sim.0");
                        else
                            mission.OriginalAircraft = mission.Aircraft;

                        AddSavedMissions(mission);

                        if (!string.IsNullOrWhiteSpace(mission.MissionType) && !string.IsNullOrWhiteSpace(mission.Aircraft))
                        {
                            mission.PropertyChanged += Mission_PropertyChanged;
                            Missions.Add(mission);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("something went wrong loading the missions" +
                    Environment.NewLine +
                    ex.Message);
            }

        }

        private void Mission_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (! e.PropertyName.Equals(nameof(Mission.IsChanged)))
                (sender as Mission).IsChanged = true;
        }

        private Manifest GetManifest(string fltFile)
        {
            DirectoryInfo di = Directory.GetParent(Path.GetDirectoryName(fltFile));

            while (di != null)
            {
                string manifest_filename = Path.Combine(di.FullName, "manifest.json");
                if (LongFile.Exists(manifest_filename))
                {
                    string jsonData = LongFile.ReadAllText(manifest_filename);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.MaxJsonLength = jsonData.Length + 10;

                    Manifest mf = js.Deserialize<Manifest>(jsonData);

                    return mf;
                }

                di = di.Parent;
            }

            return null;
        }

        private static string SavedMissonsPath()
        {
            string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string RoamingAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);


            List<string> sl = new List<string>()
            {
                LocalAppData + @"\Packages\Microsoft.FlightSimulator_8wekyb3d8bbwe\LocalState\MISSIONS\ACTIVITIES\",

                RoamingAppData + @"\Microsoft Flight Simulator\MISSIONS\ACTIVITIES\"
            };

            foreach (string d in sl)
                if (Directory.Exists(d))
                    return d;

            return string.Empty;
        }

        private void AddSavedMissions(Mission theMission)
        {
            try
            {
                string savedMissionPath = SavedMissonsPath();

                if (!string.IsNullOrWhiteSpace(savedMissionPath))
                {
                    savedMissionPath = LongFile.GetWin32LongPath(Path.Combine(savedMissionPath, theMission.Name + "_SAVE"));

                    if (Directory.Exists(savedMissionPath))
                    {
                        IEnumerable<string> fltFiles = System.IO.Directory.EnumerateFiles(savedMissionPath, "*.flt", System.IO.SearchOption.AllDirectories);

                        foreach (string fltFile in fltFiles)
                        {
                            INI INI = new INI(fltFile);

                            string name = fltFile.Replace(savedMissionPath, string.Empty);

                            if (name.IndexOf("BCKP", StringComparison.OrdinalIgnoreCase) == -1)
                            {
                                string file = Path.GetFileName(name);
                                name = name.Replace(file, string.Empty);
                                name = name.Replace("\\", string.Empty);

                                Mission savedMission = new Mission();
                                savedMission.IsProtected = theMission.IsProtected;
                                savedMission.HasWeatherFile = theMission.HasWeatherFile;

                                savedMission.MissionType = INI.Read("MissionType", "Main");
                                savedMission.Name = name;    // TODO
                                savedMission.Title = name;
                                savedMission.Aircraft = INI.Read("Sim", "Sim.0");
                                savedMission.Filename = LongFile.RemoveWin32LongPath(fltFile);
                                savedMission.DateTime = ReadDateTime(INI);

                                savedMission.LostGPS = !INI.KeyExists("WpInfo0", "GPS_Engine");

                                ReadWeather(INI, savedMission);

                                string backupname = GetBackupFilename(fltFile);

                                if (LongFile.Exists(backupname))
                                    savedMission.OriginalAircraft = new INI(backupname).Read("Sim", "Sim.0");
                                else
                                    savedMission.OriginalAircraft = savedMission.Aircraft;

                                if (!string.IsNullOrWhiteSpace(savedMission.MissionType) && !string.IsNullOrWhiteSpace(savedMission.Aircraft))
                                {
                                    savedMission.PropertyChanged += Mission_PropertyChanged;
                                    theMission.SavedMissions.Add(savedMission);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void ReadWeather(INI INI, Mission mission)
        {
            mission.UseWeatherFile = INI.ReadDefault(false, "UseWeatherFile", "Weather");
            mission.UseLiveWeather = INI.ReadDefault(false, "UseLiveWeather", "Weather");
            mission.WeatherPresetFile = INI.ReadDefault("", "WeatherPresetFile", "Weather");
            mission.WeatherCanBeLive = INI.ReadDefault(false, "WeatherCanBeLive", "Weather");
        }

        private DateTime ReadDateTime(INI INI)
        {
            int year = INI.ReadDefault(2021, "Year", "DateTimeSeason");
            int days = INI.ReadDefault(1, "Day", "DateTimeSeason");
            int hours = INI.ReadDefault(8, "Hours", "DateTimeSeason");
            int minutes = INI.ReadDefault(0, "Minutes", "DateTimeSeason");
            int seconds = INI.ReadDefault(0, "Seconds", "DateTimeSeason");

            DateTime dt = new DateTime(year, 1, 1, hours, minutes, seconds);
            dt = dt.AddDays(days -1);

            return dt;
        }

        internal void SaveMissions()
        {
            foreach (Mission mission in Missions)
            {
                if (mission.IsChanged)
                    SaveMission(mission);

                foreach (Mission savedMission in mission.SavedMissions)
                {
                    if (savedMission.IsChanged)
                        SaveMission(savedMission);
                }
            }
        }

        private void SaveMission(Mission mission)
        {
            if (mission.IsChanged)
            {
                string backupname = GetBackupFilename(mission.Filename);

                if (!LongFile.Exists(backupname))
                    LongFile.Copy(selectedMission.Filename, backupname);

                INI INI = new INI(LongFile.GetWin32LongPath(selectedMission.Filename));
                INI.Write("Sim", mission.Aircraft, "Sim.0");

                INI.Write("Year", mission.DateTime.Year, "DateTimeSeason");
                INI.Write("Day", mission.DateTime.DayOfYear, "DateTimeSeason");
                INI.Write("Hours", mission.DateTime.Hour, "DateTimeSeason");
                INI.Write("Minutes", mission.DateTime.Minute, "DateTimeSeason");
                INI.Write("Seconds", mission.DateTime.Second, "DateTimeSeason");

                if (mission.UseWeatherFile)
                {
                    INI.Write("UseWeatherFile", true, "Weather");
                    INI.DeleteKey("UseLiveWeather", "Weather");
                    INI.DeleteKey("WeatherPresetFile", "Weather");
                    INI.DeleteKey("WeatherCanBeLive", "Weather");
                }
                else
                if (mission.UseLiveWeather)
                {
                    INI.Write("UseWeatherFile", false, "Weather");
                    INI.Write("UseLiveWeather", true, "Weather");
                    INI.Write("WeatherPresetFile", "", "Weather");
                    INI.Write("WeatherCanBeLive", true, "Weather");
                }
                else
                {
                    INI.Write("UseWeatherFile", false, "Weather");
                    INI.Write("UseLiveWeather", false, "Weather");
                    INI.Write("WeatherPresetFile", mission.WeatherPresetFile, "Weather");
                    INI.Write("WeatherCanBeLive", false, "Weather");
                }

                ReadWeather(INI, mission);

                mission.IsChanged = false;
            }
        }

        string GetBackupFilename(string fullname)
        {
            string path = Path.GetDirectoryName(fullname);
            string filename = Path.GetFileNameWithoutExtension(fullname);

            string backupname = Path.Combine(path, filename + ".bak");

            return backupname;
        }
    }
}
