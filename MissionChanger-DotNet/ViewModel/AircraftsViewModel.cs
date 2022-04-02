using MissionChanger.Classes;
using MissionChanger.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void LoadAircrafts(string path)
        {
            if (Aircrafts.Count > 0)
                return;

            if (Directory.Exists(LongFile.GetWin32LongPath(path)))
            {
                string[] files1 = Directory.GetFiles(LongFile.GetWin32LongPath(Path.Combine(path, CommunityFolder)), "Aircraft.cfg", SearchOption.AllDirectories);
                string[] files2 = Directory.GetFiles(LongFile.GetWin32LongPath(Path.Combine(path, OfficialFolder)), "Aircraft.cfg", SearchOption.AllDirectories);

                string[] files = files1.Concat(files2).ToArray();

                int i = 0;

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
                                        Aircrafts.Add(new Aircraft() { Name = s[0] });
                                    }
                                }
                            }
                        }
                    }
                }

                Aircrafts = new ObservableCollection<Aircraft>(Aircrafts.OrderBy(p => p.Name));
            }
        }
    }
}
