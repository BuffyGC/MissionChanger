using System.Runtime.InteropServices;
using System.Text;

namespace MissionChanger.Classes
{
    internal class NativeMethods
    {
        #region INI

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        public static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        #endregion

    }
}
