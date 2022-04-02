using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionChanger.Classes
{
    internal class CommFolderDetector
    {
        private static readonly List<string> UserCfgDirs = new List<string>
        {
            // Roaming
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FlightSimulator\UserCfg.opt",
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Flight Simulator\UserCfg.opt",
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft Flight Simulator\UserCfg.opt",
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft FlightSimulator\UserCfg.opt",
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MicrosoftFlightSimulator\UserCfg.opt",
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FlightSimulator_8wekyb3d8bbwe\UserCfg.opt",
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FlightSimulatorKHAlpha_8wekyb3d8bbwe\UserCfg.opt",
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FlightSimulatorFlightSimDisc_8wekyb3d8bbwe\UserCfg.opt",
            
            // Local
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Packages\Microsoft.FlightSimulator_8wekyb3d8bbwe\LocalCache\UserCfg.opt",
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Packages\Microsoft.FlightSimulatorKHAlpha_8wekyb3d8bbwe\LocalCache\UserCfg.opt",
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Packages\Microsoft.KHAlpha_8wekyb3d8bbwe\LocalCache\UserCfg.opt",
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Packages\Microsoft.FlightSimulatorFlightSimDisc_8wekyb3d8bbwe\LocalCache\UserCfg.opt"
        };

        internal static string GetFSBaseFolder()
        {
            string s = string.Empty;

            foreach (string fn in UserCfgDirs)
            {
                if (LongFile.Exists(fn))
                {
                    s = ReadUserCfg(fn);

                    if (s.Length > 0)
                        return s;
                }
            }

            return s;
        }

        private static string ReadUserCfg(string fn)
        {
            string[] UserCfg = LongFile.ReadAllLines(fn);

            for (int i = UserCfg.Count() - 1; i >= 0; i--)
            {
                if (UserCfg[i].Contains("InstalledPackagesPath"))
                {
                    int pFrom = UserCfg[i].IndexOf("\"") + 1;
                    int pTo = UserCfg[i].LastIndexOf("\"");

                    string path = UserCfg[i].Substring(pFrom, pTo - pFrom);

                    if (IsFSBaseDir(path))
                        return path; //+ @"\Community";
                }
            }

            return string.Empty;
        }


        private static readonly string OfficalPath = @"\Official";
        private static readonly string OfficialSteamPath = OfficalPath + @"\Steam\";
        private static readonly string OfficialStorePath = OfficalPath + @"\OneStore\";

        private static readonly List<string> OfficialDirs = new List<string>
        {
            OfficialSteamPath,
            OfficialStorePath
        };

        private static bool IsFSBaseDir(string selectedPath)
        {
            foreach (string officialPath in OfficialDirs)
            {
                if (System.IO.Directory.Exists(selectedPath + officialPath))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
