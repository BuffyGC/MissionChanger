using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MissionChanger.Classes
{
    internal class LongFile
    {
        internal const int MAX_PATH = 260;

        internal static bool Exists(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            return System.IO.File.Exists(path);
        }

        internal static void Delete(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            System.IO.File.Delete(path);
        }

        internal static void Copy(string sourceFileName, string destFileName)
        {
            Copy(sourceFileName, destFileName, false);
        }

        internal static void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            if (sourceFileName.Length >= MAX_PATH)
                sourceFileName = GetWin32LongPath(sourceFileName);

            if (destFileName.Length >= MAX_PATH)
                destFileName = GetWin32LongPath(destFileName);

            System.IO.File.Copy(sourceFileName, destFileName, overwrite);
        }

        internal static void Move(string sourceFileName, string destFileName)
        {
            if (sourceFileName.Length >= MAX_PATH)
                sourceFileName = GetWin32LongPath(sourceFileName);

            if (destFileName.Length >= MAX_PATH)
                destFileName = GetWin32LongPath(destFileName);

            System.IO.File.Move(sourceFileName, destFileName);
        }

        internal static byte[] ReadAllBytes(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            return System.IO.File.ReadAllBytes(path);
        }

        internal static string[] ReadAllLines(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            return System.IO.File.ReadAllLines(path);
        }

        internal static string ReadAllText(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            return System.IO.File.ReadAllText(path);
        }

        internal static System.IO.StreamWriter AppendText(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            return System.IO.File.AppendText(path);
        }



        internal static void WriteAllLines(string path, string[] contents)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            System.IO.File.WriteAllLines(path, contents);

        }

        internal static void WriteAllText(string path, string contents)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            System.IO.File.WriteAllText(path, contents);
        }

        internal static void WriteAllText(string path, string contents, Encoding encoding)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            System.IO.File.WriteAllText(path, contents, encoding);
        }

        internal static void WriteAllBytes(string path, byte[] bytes)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            System.IO.File.WriteAllBytes(path, bytes);
        }



        internal static System.IO.FileStream Create(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            return System.IO.File.Create(path);
        }

        internal static DateTime GetLastWriteTime(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            return System.IO.File.GetLastWriteTime(path);

        }

        internal static string RemoveWin32LongPath(string path)
        {
            if (path.StartsWith(@"\\?\UNC\"))
                path = path.Remove(0, 8);
            else
            if (path.StartsWith(@"\\?\"))
                path = path.Remove(0, 4);

            return path;
        }

        internal static string GetWin32LongPath(string path, bool check = false)
        {
            if (check && path.Length < MAX_PATH)
                return path;

            if (path.StartsWith(@"\\?\")) return path;

            if (path.StartsWith("\\"))
            {
                path = @"\\?\UNC\" + path.Substring(2);
            }
            else if (path.Contains(":"))
            {
                path = @"\\?\" + path;
            }
            else
            {
                var currdir = Environment.CurrentDirectory;
                path = Combine(currdir, path);
                while (path.Contains("\\.\\")) path = path.Replace("\\.\\", "\\");
                path = @"\\?\" + path;
            }
            return path.TrimEnd('.'); 
        }

        private static string Combine(string path1, string path2)
        {
            return path1.TrimEnd('\\') + "\\" + path2.TrimStart('\\').TrimEnd('.'); ;
        }


        internal static string ReplaceIllegalFilenameChars(string filename, string replacement = "")
        {
            string regexSearch = new string(System.IO.Path.GetInvalidFileNameChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            string res = r.Replace(filename, replacement);

            return res;
        }

        internal static string GetMD5(string filename)
        {
            if (Exists(filename))
            {
                using (MD5 md5 = MD5.Create())
                {
                    using (System.IO.FileStream stream = System.IO.File.OpenRead(GetWin32LongPath(filename, true)))
                    {
                        byte[] hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }

            return string.Empty;
        }

    }
}
