using System;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;

namespace Editor
{
    public static class ResourcesImporter
    {
        public const string settingsPath = "Assets/settings.ini";
        
        public enum Settings
        {
            PathToCoreProjectAsstes
        }

        public static void SaveToSetting(this string value, Settings settingName)
        {
            var ini = new IniFile();
            ini["core"][settingName.ToString()] = value;
            ini.Save(settingsPath);
        }

        public static string LoadFromSettings(this Settings settingName)
        {
            var ini = new IniFile();
            ini.Load(settingsPath);
            return ini["core"][settingName.ToString()].Value;
        }
        
        [MenuItem("CoreProject/ImportAssets")]
        public static void ImportCoreProjectAssets()
        {
            var path = Settings.PathToCoreProjectAsstes.LoadFromSettings();
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                EditorUtility.DisplayDialog("Oh", $"Bad path: \"{path}\". Please set path in \"CoreProject/Set CoreProject LayoutAssets folder\"", "ok");
                return;
            }
            
            
            CopyDirectory(path, "Assets/Resources/Layouts", 0, Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Count(x => !Path.GetExtension(x).Equals(".meta", StringComparison.OrdinalIgnoreCase)));
            AssetDatabase.Refresh();
        }


        private static void CopyDirectory(string srcdir, string desdir, int currentProgress, int allCount)
        {
            string folderName = srcdir.Substring(srcdir.LastIndexOf("\\")+1);
            string desfolderdir = desdir +"\\"+ folderName;
            if (desdir.LastIndexOf("\\") == (desdir.Length - 1))
            {
                desfolderdir = desdir + folderName;
            }
            var filenames = Directory.EnumerateFiles(srcdir, "*", SearchOption.TopDirectoryOnly).Where(x => !Path.GetExtension(x).Equals(".meta", StringComparison.OrdinalIgnoreCase));
            var directories = Directory.EnumerateDirectories(srcdir, "*");
            foreach (var directory in directories)
            {
                if (Directory.Exists(directory)) {              
                    string currentdir = desfolderdir + "\\" + directory.Substring(directory.LastIndexOf("\\") + 1);
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }
                    CopyDirectory(directory, desfolderdir, currentProgress, allCount);
                }
            }
            foreach (string file in filenames) {
                
                string srcfileName = file.Substring(file.LastIndexOf("\\")+1);
                srcfileName = desfolderdir + "\\" + srcfileName;
                if (!Directory.Exists(desfolderdir))
                {
                    Directory.CreateDirectory(desfolderdir);
                }
                File.Copy(file, srcfileName, true);
                currentProgress++;
                if (allCount != 0)
                {
                    float proc = (float)currentProgress / allCount;
                    EditorUtility.DisplayProgressBar("Copying assets from core project", (int)(proc * 100) + "%", proc);    
                }
                
            }
            EditorUtility.ClearProgressBar();

        }
    }
}