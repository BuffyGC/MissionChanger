using System;
using System.Linq;
using System.Reflection;
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

        internal static bool ExistsInFolder(string path, string searchPattern)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            return System.IO.Directory.EnumerateFiles(path, searchPattern).Any();
        }

        internal static void Delete(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);
            
            System.IO.File.Delete(path);
        }

        internal static void DeleteIfExists(string path)
        {
            if (LongFile.Exists(path))
                LongFile.Delete(path);
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

        internal static void Move(string sourceFileName, string destFileName, bool overwrite)
        {
            if (sourceFileName.Length >= MAX_PATH)
                sourceFileName = GetWin32LongPath(sourceFileName);

            if (destFileName.Length >= MAX_PATH)
                destFileName = GetWin32LongPath(destFileName);

            if (overwrite && Exists(destFileName))
                Delete(destFileName);

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

            if (Exists(path))
                return System.IO.File.ReadAllLines(path);

            return new string[0];
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

        internal static DateTime GetCreationTime(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            return System.IO.File.GetCreationTime(path);
        }

        internal static void SetCreationTime(string path, DateTime creationTime)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            System.IO.File.SetCreationTime(path, creationTime);
        }

        internal static DateTime GetLastWriteTime(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            return System.IO.File.GetLastWriteTime(path);
        }

        internal static void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            System.IO.File.SetLastWriteTime(path, lastWriteTime);
        }

        internal static string RemoveWin32LongPath(string path)
        {
            if (path.StartsWith(@"\\?\UNC\"))
            {
                path = @"\\" + path.Substring(8);
            }
            else
            if (path.StartsWith(@"\\?\"))
            {
                path = path.Substring(4);
            }

            return path;
        }

        internal static string GetWin32LongPath(string path, bool check = false)
        {
            if (check && path.Length < MAX_PATH)
                return path;

            if (path.StartsWith(@"\\?\"))
                return path;

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


        internal static bool DirectoryExists(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            return System.IO.Directory.Exists(path);
        }

        internal static bool DirectoryIsEmpty(string path)
        {
            if (!DirectoryExists(path))
                return true;

            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);

            var dirs = di.EnumerateDirectories("*", System.IO.SearchOption.AllDirectories);

            if (dirs.Any())
                return false;

            var files = di.EnumerateFiles("*", System.IO.SearchOption.AllDirectories);

            if (files.Any())
                return false;

            return true;
        }


        internal static System.IO.DirectoryInfo CreateDirectory(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            return System.IO.Directory.CreateDirectory(path);
        }


        internal static void DeleteDirectory(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            System.IO.Directory.Delete(path);
        }

        internal static void DeleteDirectory(string path, bool recursive)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            System.IO.Directory.Delete(path, recursive);
        }

        internal static void ForceDeleteDirectory(string path)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            if (!System.IO.Directory.Exists(path))
                return;

            string [] filenames = System.IO.Directory.GetFiles(path);

            foreach (string filename in filenames)
            {
                try
                {
                    string longFileName = GetWin32LongPath(filename);
                    //Logger.WriteFile(MethodBase.GetCurrentMethod(), "ToDelete: ", longFileName, 1);

                    System.IO.File.SetAttributes(longFileName, System.IO.FileAttributes.Normal);
                    LongFile.Delete(longFileName);
                }
                catch(Exception )
                {
                    //Logger.WriteException(MethodBase.GetCurrentMethod(), "Error deleting file:", ex, 0);
                    //Logger.WriteFile(MethodBase.GetCurrentMethod(), "Error deleting file:", GetWin32LongPath(filename, true), 0);
                }
            }

            string[] directories = System.IO.Directory.GetDirectories(path);

            foreach (string directory in directories)
            {
                ForceDeleteDirectory(directory);
            }

            System.IO.Directory.Delete(path, true);
        }

        internal static void RemoveEmptyDirectories(string path, bool inclTopLevelFolder = false)
        {
            if (path.Length >= MAX_PATH)
                path = GetWin32LongPath(path);

            if (!System.IO.Directory.Exists(path))
                return;

            var fi = System.IO.Directory.EnumerateFiles(path);

            if (! fi.Any())
            {
                string[] directories = System.IO.Directory.GetDirectories(path);

                foreach (string directory in directories)
                {
                    RemoveEmptyDirectories(directory, true);
                }
            }

            if (inclTopLevelFolder)
            {
                if (!fi.Any())
                {
                    var di = System.IO.Directory.EnumerateDirectories(path);

                    if (!di.Any())
                        System.IO.Directory.Delete(path);
                }
            }
        }

        internal static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool overwrite = false)
        {
            sourceDirName = LongFile.GetWin32LongPath(sourceDirName);
            destDirName = LongFile.GetWin32LongPath(destDirName);

            // Get the subdirectories for the specified directory.
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new System.IO.DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it.       
            System.IO.Directory.CreateDirectory(destDirName);


            // Get the files in the directory and copy them to the new location.
            string[] files = System.IO.Directory.GetFiles(sourceDirName);

            foreach (string file in files)
            {
                string destFile = System.IO.Path.Combine(destDirName, System.IO.Path.GetFileName(file));

                if (LongFile.Exists(destFile))
                    LongFile.Delete(destFile);

                LongFile.Copy(file, destFile, overwrite);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                string[] dirs = System.IO.Directory.GetDirectories(sourceDirName);

                foreach (string subdir in dirs)
                {
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(subdir);
                    string destPath = System.IO.Path.Combine(destDirName, di.Name);
                    DirectoryCopy(subdir, destPath, copySubDirs, overwrite);
                }
            }
        }


        internal static void DirectoryMove(string sourceDirName, string destDirName, bool overwrite = false, bool removeTopLevelFolder = false)
        {
            sourceDirName = LongFile.GetWin32LongPath(sourceDirName);
            destDirName = LongFile.GetWin32LongPath(destDirName);

            // Get the subdirectories for the specified directory.
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new System.IO.DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it.       
            System.IO.Directory.CreateDirectory(destDirName);


            // Get the files in the directory and copy them to the new location.
            string[] files = System.IO.Directory.GetFiles(sourceDirName);

            foreach (string file in files)
            {
                string destFile = System.IO.Path.Combine(destDirName, System.IO.Path.GetFileName(file));

                if (overwrite && LongFile.Exists(destFile))
                    LongFile.Delete(destFile);

                LongFile.Move(file, destFile, overwrite);
            }

            string[] dirs = System.IO.Directory.GetDirectories(sourceDirName);

            foreach (string subdir in dirs)
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(subdir);
                string destPath = System.IO.Path.Combine(destDirName, di.Name);
                DirectoryMove(subdir, destPath, overwrite, true);
            }

            if (removeTopLevelFolder)
                System.IO.Directory.Delete(sourceDirName);
        }
    }
}
