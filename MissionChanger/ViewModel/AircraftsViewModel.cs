using MissionChanger.Classes;
using MissionChanger.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace MissionChanger.ViewModel
{
    public class AircraftsViewModel : BaseViewModel
    {
        public ObservableCollection<Aircraft> Aircrafts { get => aircrafts; set => SetField(ref aircrafts, value); }
        private ObservableCollection<Aircraft> aircrafts = new ObservableCollection<Aircraft>();

        public Aircraft SelectedAircraft { get => selectedAircraft; set => SetField(ref selectedAircraft, value); }
        private Aircraft selectedAircraft;

        public AircraftsViewModel()
        {
        }

        internal static readonly string CommunityFolder = @"Community\";
        internal static readonly string OfficialFolder = @"Official\";

        private ObservableCollection<Aircraft> InitDefaultAircrafts()
        {
            ObservableCollection<Aircraft> aircrafts = new ObservableCollection<Aircraft>();

            try
            {
                string Uri = @"https://raw.githubusercontent.com/BuffyGC/BushTripInjector/main/api/v1/aircrafts/aircrafts.xml";
                string xml = HTTP.GetString(Uri);

                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Aircraft>), new XmlRootAttribute("BushTripInjector"));
                    aircrafts = (ObservableCollection<Aircraft>)xs.Deserialize(sr);
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
                string[] files1 = Directory.GetFiles(LongFile.GetWin32LongPath(Path.Combine(path, OfficialFolder)), "Aircraft.cfg", SearchOption.AllDirectories);
                string[] files2 = Directory.GetFiles(LongFile.GetWin32LongPath(Path.Combine(path, CommunityFolder)), "Aircraft.cfg", SearchOption.AllDirectories);

                string[] files = files1.Concat(files2).ToArray();

                int i = 0;

                List<Aircraft> AircraftList = Aircrafts.ToList();
                List<Aircraft> tempAircrafts = new List<Aircraft>();

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

                                if (s.Length > 0)
                                {
                                    if (s[0].Contains("SDK"))
                                    {

                                    }

                                    Aircraft m = Aircrafts.FirstOrDefault(p => p.Name.Equals(s[0], StringComparison.OrdinalIgnoreCase));

                                    if (m == null)
                                    {
                                        AircraftSourceTypeEnum aircraftSourceType = AircraftSourceTypeEnum.Unknown;
                                        string sourcePath = string.Empty;
                                        string baseName = string.Empty;
                                        int index = -1;

                                        string base_container = INI.Read("base_container", "VARIATION").Replace(@"\\", @"\");

                                        if (!string.IsNullOrEmpty(base_container))
                                        {
                                            Aircraft bm = AircraftList.FirstOrDefault(
                                                p => p.SourcePath.Equals(base_container, StringComparison.OrdinalIgnoreCase)
                                                  || p.AltSourcePath.Equals(base_container, StringComparison.OrdinalIgnoreCase)
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

                                            string altPath = GetBasePath(f);

                                        }


                                        if (!baseName.StartsWith("["))
                                        {

                                            if (f.StartsWith(LongFile.GetWin32LongPath(Path.Combine(path, OfficialFolder)), StringComparison.OrdinalIgnoreCase))
                                            {
                                                if (aircraftSourceType == AircraftSourceTypeEnum.Unknown)
                                                    aircraftSourceType = AircraftSourceTypeEnum.Official;
                                            }
                                            else
                                            if (f.StartsWith(LongFile.GetWin32LongPath(Path.Combine(path, CommunityFolder)), StringComparison.OrdinalIgnoreCase))
                                                aircraftSourceType = AircraftSourceTypeEnum.Community;
                                        }

                                        Aircraft aircraft = new Aircraft()
                                        {
                                            Name = s[0],
                                            BaseName = baseName,
                                            SourcePath = sourcePath,
                                            SourceType = aircraftSourceType,
                                        };

                                        if (index != -1)
                                        {
                                            tempAircrafts.Add(aircraft);
                                        }
                                        else
                                        {
                                            Aircraft am = AircraftList.FirstOrDefault(a => aircraft.Name.CompareTo(a.Name) < 0);

                                            if (am != null)
                                                index = AircraftList.IndexOf(am);

                                            if (index != -1)
                                                AircraftList.Insert(index, aircraft);
                                            else
                                                AircraftList.Add(aircraft);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                tempAircrafts = tempAircrafts.OrderBy(a => a.BaseName).ThenByDescending(a => a.Name).ToList();

                foreach (Aircraft aircraft in tempAircrafts)
                {
                    int index = -1;

                    Aircraft bm = AircraftList.FirstOrDefault(a => a.Name.Equals(aircraft.BaseName));

                    if (bm != null)
                        index = AircraftList.IndexOf(bm);

                    if (index != -1)
                        AircraftList.Insert(index + 1, aircraft);
                    else
                        AircraftList.Add(aircraft);
                }

                tempAircrafts.Clear();

                Aircrafts = new ObservableCollection<Aircraft>(AircraftList);
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
