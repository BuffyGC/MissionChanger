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

namespace MissionChanger.ViewModel
{
    public class MissionViewModel : BaseViewModel
    {
        public ObservableCollection<Mission> Missions { get => missions; set => SetField(ref missions, value); }
        private ObservableCollection<Mission> missions = new ObservableCollection<Mission>();

        public Mission SelectedMission { get => selectedMission; set => SetField(ref selectedMission, value); }
        private Mission selectedMission;


        public MissionViewModel()
        {
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
                    mission.Name = System.IO.Path.GetFileNameWithoutExtension(fltFile);
                    mission.Aircraft = INI.Read("Sim", "Sim.0");
                    mission.Filename = fltFile;

                    string backupname = GetBackupFilename(fltFile);

                    if (LongFile.Exists(backupname))
                        mission.OriginalAircraft = new INI(backupname).Read("Sim", "Sim.0");

                    Missions.Add(mission);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("something went wrong loading the missions" +
                    Environment.NewLine +
                    ex.Message);
            }

        }

        internal void SetAircraft(Aircraft selectedAircraft)
        {
            if (selectedMission != null && selectedAircraft != null)
            {
                string backupname = GetBackupFilename(selectedMission.Filename);

                if (!LongFile.Exists(backupname))
                    LongFile.Copy(selectedMission.Filename, backupname);

                INI INI = new INI(selectedMission.Filename);
                INI.Write("Sim", selectedAircraft.Name, "Sim.0");

                if (string.IsNullOrEmpty(SelectedMission.OriginalAircraft))
                    SelectedMission.OriginalAircraft = SelectedMission.Aircraft;

                SelectedMission.Aircraft = selectedAircraft.Name;

                if (SelectedMission.Aircraft.Equals(SelectedMission.OriginalAircraft))
                {
                    SelectedMission.OriginalAircraft = string.Empty;

                    if (LongFile.Exists(backupname))
                        LongFile.Delete(backupname);
                }
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
