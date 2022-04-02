using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace MissionChanger.Classes
{
    internal class INI
    {
        private readonly string Path;
        private readonly string EXE = Assembly.GetExecutingAssembly().GetName().Name;



        public INI(string IniPath = null)
        {
            Path = new System.IO.FileInfo(IniPath ?? EXE + ".ini").FullName;

            if (Path.Contains("KITD"))
                Path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\MissionChanger\MissionChanger.ini";
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(2048);
            NativeMethods.GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 2048, Path);
            return RetVal.ToString();
        }

        public string ReadDefault(string Default, string Key, string Section = null)
        {
            if (KeyExists(Key, Section))
                return Read(Key, Section);

            return Default;
        }

        public bool ReadDefault(bool Default, string Key, string Section = null)
        {
            if (KeyExists(Key, Section))
                return Read(Key, Section).Equals("true", StringComparison.OrdinalIgnoreCase);

            return Default;
        }

        public int ReadDefault(int Default, string Key, string Section = null)
        {
            if (KeyExists(Key, Section))
            {
                string s = Read(Key, Section);

                if (int.TryParse(s, out int r))
                    return r;
            }

            return Default;
        }

        public double ReadDefault(double Default, string Key, string Section = null)
        {
            if (KeyExists(Key, Section))
            {
                string s = Read(Key, Section);

                if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double r))
                    return r;
            }

            return Default;
        }
        public DateTime ReadDefault(DateTime Default, string Key, string Section = null)
        {
            if (KeyExists(Key, Section))
            {
                string s = Read(Key, Section);

                if (DateTime.TryParse(s, out DateTime d))
                    return d;
            }

            return Default;
        }

        public void Write(string Key, bool Value, string Section = null)
        {
            NativeMethods.WritePrivateProfileString(Section ?? EXE, Key, Value.ToString(), Path);
        }

        public void Write(string Key, int Value, string Section = null)
        {
            NativeMethods.WritePrivateProfileString(Section ?? EXE, Key, Value.ToString(), Path);
        }

        public void Write(string Key, string Value, string Section = null)
        {
            NativeMethods.WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }

        internal List<string> GetAllChapters(string prefix = null)
        {
            List<string> result = new List<string>();

            string[] strings = LongFile.ReadAllLines(Path);

            foreach (string s in strings)
            {
                if (s.StartsWith("["))
                {
                    string s1 = s.Substring(1);

                    int p = s1.IndexOf(']');

                    s1 = s1.Substring(0, p);

                    if (prefix == null || s1.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        result.Add(s1);
                }
            }

            return result;
        }

        internal List<string> GetAllEntries(string section)
        {
            List<string> entries = new List<string>();

            string[] strings = LongFile.ReadAllLines(Path);

            bool inSection = false;

            foreach (string s in strings)
            {
                if (s.StartsWith("["))
                {
                    if (inSection)
                    {
                        inSection = false;
                        break;  // next section
                    }
                    else
                    {
                        string s1 = s.Substring(1);

                        int p = s1.IndexOf(']');

                        s1 = s1.Substring(0, p);

                        if (section.Equals(s1, StringComparison.OrdinalIgnoreCase))
                        {
                            inSection = true;
                        }
                    }
                }
                else
                {
                    if (inSection && !s.StartsWith(";"))
                    {
                        string[] entry = s.Split('=');

                        if (entry.Length > 0 && !string.IsNullOrWhiteSpace(entry[0]))
                            entries.Add(entry[0]);
                    }
                }
            }


            return entries;
        }
    }
}
