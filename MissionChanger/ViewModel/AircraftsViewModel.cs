using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Xml.Serialization;
using MissionChanger.Classes;
using MissionChanger.Model;

namespace MissionChanger.ViewModel
{
    public class AircraftsViewModel : BaseViewModel
    {
        public ObservableCollection<AircraftModel> Aircrafts { get => aircrafts; set => SetField(ref aircrafts, value); }
        private ObservableCollection<AircraftModel> aircrafts = new ObservableCollection<AircraftModel>();

        public CollectionView AircraftsView { get => aircraftsView; set => SetField(ref aircraftsView, value); }
        private CollectionView aircraftsView;

        public string Filter { get => filter; set { if (SetField(ref filter, value)) RefreshFilter(); } }
        private string filter;

        public AircraftModel SelectedAircraft { get => selectedAircraft; set => SetField(ref selectedAircraft, value); }
        private AircraftModel selectedAircraft;

        public bool ShowAll { get => showAll; set => SetField(ref showAll, value); }
        private bool showAll = false;


        public AircraftsViewModel()
        {
            SetViewSource();
        }

        internal static readonly string CommunityFolder = @"Community\";
        internal static readonly string OfficialFolder = @"Official\";

        private void SetViewSource()
        {
            CollectionViewSource aircraftsCollectionViewSource = new CollectionViewSource
            {
                Source = Aircrafts
            };
            AircraftsView = (CollectionView)aircraftsCollectionViewSource.View;
            AircraftsView.Filter = AircraftsFilter;
        }

        private bool AircraftsFilter(object obj)
        {
            if (obj is AircraftModel aircraft)
            {
                if (string.IsNullOrWhiteSpace(Filter))
                    return ShowAll || aircraft.AircraftInstalled == AircraftInstallEnum.Installed;

                if (aircraft == SelectedAircraft)
                    return true;

                if (aircraft.Name.ToUpper().Contains(Filter.ToUpper()))
                    return ShowAll || aircraft.AircraftInstalled == AircraftInstallEnum.Installed;

                if (string.IsNullOrEmpty(aircraft.BaseName) || aircraft.BaseName.StartsWith("["))
                {
                    var livs = Aircrafts.Where(ac => ac.BaseName.Equals(aircraft.Name, StringComparison.OrdinalIgnoreCase));

                    if (livs != null)
                    {
                        foreach (AircraftModel liv in livs)
                        {
                            if (liv == SelectedAircraft)
                                return ShowAll || aircraft.AircraftInstalled == AircraftInstallEnum.Installed;

                            if (liv.Name.ToUpper().Contains(Filter.ToUpper()))
                                return ShowAll || aircraft.AircraftInstalled == AircraftInstallEnum.Installed;
                        }
                    }
                }
            }

            return false;
        }

        internal void RefreshFilter()
        {
            // SetViewSource();
            AircraftsView.Refresh();
        }


        private ObservableCollection<AircraftModel> InitDefaultAircrafts()
        {
            ObservableCollection<AircraftModel> aircrafts = new ObservableCollection<AircraftModel>();

            try
            {
                string Uri = @"https://raw.githubusercontent.com/BuffyGC/BushTripInjector/main/api/v1/aircrafts/aircrafts.xml";
                string xml = HTTP.GetString(Uri);

                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<AircraftModel>), new XmlRootAttribute("BushTripInjector"));
                    aircrafts = (ObservableCollection<AircraftModel>)xs.Deserialize(sr);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong while downloading MSFS protected Aircrafts information. Maybe not all aircrafts will be available for your mission!");
            }



            return aircrafts;
        }

        public void LoadAircrafts(string path)
        {
            if (Aircrafts.Count > 0)
                return;

            Aircrafts = InitDefaultAircrafts();

            if (Directory.Exists(LongFile.GetWin32LongPath(path)))
            {
                string baseFolder = CommFolderDetector.GetFSBaseFolder();

                string offPath =
                    LongFile.DirectoryExists(Path.Combine(baseFolder, OfficialFolder))
                    ? Path.Combine(baseFolder, OfficialFolder)
                    : Path.Combine(path, OfficialFolder);

                string commPath =
                    LongFile.DirectoryExists(Path.Combine(baseFolder, CommunityFolder))
                    ? Path.Combine(baseFolder, CommunityFolder)
                    : Path.Combine(path, CommunityFolder);

                string[] files1 = LongFile.DirectoryExists(offPath)
                    ? Directory.GetFiles(LongFile.GetWin32LongPath(offPath), "Aircraft.cfg", SearchOption.AllDirectories)
                    : new string[0];

                string[] files2 = LongFile.DirectoryExists(commPath)
                    ? Directory.GetFiles(LongFile.GetWin32LongPath(commPath), "Aircraft.cfg", SearchOption.AllDirectories)
                    : new string[0];

                string[] files3 = !baseFolder.Equals(path) && LongFile.DirectoryExists(path)
                    ? Directory.GetFiles(LongFile.GetWin32LongPath(path), "Aircraft.cfg", SearchOption.AllDirectories)
                    : new string[0];

                string[] fs_basepathes =
                    LongFile.DirectoryExists(offPath)
                    ? Directory.GetDirectories(LongFile.GetWin32LongPath(offPath), "fs-base", SearchOption.AllDirectories)
                    : new string[0];

                string[] files = files1.Concat(files2).Concat(files3).ToArray();


                int i = 0;

                List<AircraftModel> AircraftList = Aircrafts.ToList();
                List<AircraftModel> tempAircrafts = new List<AircraftModel>();

                foreach (string f in files)
                {
                    if (f.ToLower().Contains("fs-devmode"))
                        continue;

                    i++;


                    INI INI = new INI(f);

                    List<string> chapters = INI.GetAllChapters("FLTSIM.");

                    foreach (string chapter in chapters)
                    {
                        string isUserSelectable = INI.Read("isUserSelectable", chapter).Trim();

                        if (!isUserSelectable.StartsWith("0"))
                        {
                            string name = INI.Read("title", chapter);

                            if (name?.Length > 0)
                            {
                                string[] s = name.Split(new[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);

                                if (name.ToUpper().Contains("JUST"))
                                { }

                                if (s.Length > 0)
                                {
                                    if (s[0].Contains("SDK"))
                                    {

                                    }

                                    AircraftModel m = Aircrafts.FirstOrDefault(p => p.Name.Equals(s[0], StringComparison.OrdinalIgnoreCase));

                                    if (m == null)
                                    {
                                        AircraftSourceTypeEnum aircraftSourceType = AircraftSourceTypeEnum.Unknown;
                                        string sourcePath = string.Empty;
                                        string altSourcePath = string.Empty;
                                        string baseName = string.Empty;
                                        int index = -1;

                                        string base_container = INI.Read("base_container", "VARIATION").Replace(@"\\", @"\");

                                        if (!string.IsNullOrEmpty(base_container))
                                        {
                                            AircraftModel bm = AircraftList.FirstOrDefault(
                                                p => p.SourcePath.Equals(base_container, StringComparison.OrdinalIgnoreCase)
                                            );

                                            if (bm != null)
                                            {
                                                aircraftSourceType = bm.SourceType;
                                                baseName = bm.Name;

                                                index = AircraftList.IndexOf(bm);
                                            }
                                            else
                                            {
                                                // ..\Asobo_103Solo_SmallWheels
                                                baseName = $"[{base_container.Replace(@"..\", "")}] not installed";
                                            }
                                        }
                                        else
                                        {
                                            sourcePath = @"..\" + Directory.GetParent(f).Name;
                                            altSourcePath = GetBasePath(f);
                                        }



                                        if (f.StartsWith(LongFile.GetWin32LongPath(Path.Combine(path, OfficialFolder)), StringComparison.OrdinalIgnoreCase)
                                            || f.StartsWith(LongFile.GetWin32LongPath(offPath), StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (aircraftSourceType == AircraftSourceTypeEnum.Unknown)
                                                aircraftSourceType = AircraftSourceTypeEnum.Official;
                                        }
                                        else
                                        if (!baseName.StartsWith("["))
                                        {
                                            if (f.StartsWith(LongFile.GetWin32LongPath(Path.Combine(path, CommunityFolder)), StringComparison.OrdinalIgnoreCase)
                                                || f.StartsWith(LongFile.GetWin32LongPath(commPath), StringComparison.OrdinalIgnoreCase))
                                                aircraftSourceType = AircraftSourceTypeEnum.Community;
                                        }

                                        AircraftModel aircraft = new AircraftModel()
                                        {
                                            Name = s[0],
                                            BaseName = baseName,
                                            SourcePath = sourcePath,
                                            AltSourcePath = altSourcePath,
                                            SourceType = aircraftSourceType,
                                        };

                                        if (index != -1)
                                        {
                                            tempAircrafts.Add(aircraft);
                                        }
                                        else
                                        {
                                            AircraftModel am = AircraftList.FirstOrDefault(a => aircraft.Name.CompareTo(a.Name) < 0);

                                            if (am != null)
                                                index = AircraftList.IndexOf(am);

                                            if (index != -1)
                                                AircraftList.Insert(index, aircraft);
                                            else
                                                AircraftList.Add(aircraft);
                                        }
                                    }
                                    else
                                    { }
                                }
                            }
                        }
                    }
                }

                tempAircrafts = tempAircrafts.OrderBy(a => a.BaseName).ThenByDescending(a => a.Name).ToList();

                foreach (AircraftModel aircraft in tempAircrafts)
                {
                    int index = -1;

                    AircraftModel bm = AircraftList.FirstOrDefault(a => a.Name.Equals(aircraft.BaseName));

                    if (bm != null)
                        index = AircraftList.IndexOf(bm);

                    if (index != -1)
                        AircraftList.Insert(index + 1, aircraft);
                    else
                        AircraftList.Add(aircraft);
                }

                tempAircrafts.Clear();

                if (fs_basepathes.Length > 0)
                {
                    string fs_base_path = fs_basepathes[0].Replace("fs-base", "");

                    foreach (AircraftModel ai in AircraftList.Where(a => !string.IsNullOrEmpty(a.AltSourcePath)))
                    {
                        if (ai.Name.ToUpper().Contains("JUST"))
                        { }

                        bool exists =
                            LongFile.Exists(Path.Combine(fs_base_path, ai.AltSourcePath, "manifest.json")) ||
                            LongFile.Exists(Path.Combine(commPath, ai.AltSourcePath, "manifest.json"));

                        if (!exists && !string.IsNullOrEmpty(ai.AltSourcePath2))
                        {
                            exists =
                                LongFile.Exists(Path.Combine(fs_base_path, ai.AltSourcePath2, "manifest.json")) ||
                                LongFile.Exists(Path.Combine(commPath, ai.AltSourcePath2, "manifest.json"));
                        }

                        if (exists)
                            ai.AircraftInstalled = AircraftInstallEnum.Installed;
                        else
                            ai.AircraftInstalled = AircraftInstallEnum.NotInstalled;
                    }
                    foreach (AircraftModel ai in AircraftList.Where(a => string.IsNullOrEmpty(a.AltSourcePath)))
                    {
                        if (ai.Name.ToUpper().Contains("JUST"))
                        { }

                        AircraftModel baseModel = AircraftList.Where(a => a.Name.Equals(ai.BaseName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (baseModel != null)
                        {
                            if (baseModel.AircraftInstalled == AircraftInstallEnum.Installed)
                                ai.AircraftInstalled = AircraftInstallEnum.Installed;
                            else
                            {
                                if (ai.SourceType == AircraftSourceTypeEnum.Community)
                                    ai.AircraftInstalled = AircraftInstallEnum.BaseNotInst;
                                else
                                    ai.AircraftInstalled = AircraftInstallEnum.NotInstalled;
                            }
                        }
                        else
                            ai.AircraftInstalled = AircraftInstallEnum.BaseNotInst;
                    }

                    // Aircrafts = new ObservableCollection<AircraftModel>(AircraftList.Where(a => a.AircraftInstalled == AircraftInstallEnum.Installed));
                    Aircrafts = new ObservableCollection<AircraftModel>(AircraftList);
                }
                else
                    Aircrafts = new ObservableCollection<AircraftModel>(AircraftList);

                SetViewSource();
            }
        }


        private string GetBasePath(string file)
        {
            DirectoryInfo di = Directory.GetParent(Path.GetDirectoryName(file));

            while (di != null)
            {
                string manifest_filename = Path.Combine(di.FullName, "manifest.json");
                if (LongFile.Exists(manifest_filename))
                {
                    return di.Name;
                }

                di = di.Parent;
            }

            return null;
        }
    }
}
