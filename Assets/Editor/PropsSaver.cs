using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PropsSaver : EditorWindow
    {
        public string WindowText { get; set; }
        public ResourcesImporter.Settings SettingName { get; set; }
        
        private string InputPath { get; set; }
 
        void OnGUI()
        {
            EditorGUIUtility.labelWidth = 420;
            EditorGUIUtility.fieldWidth = 420;
            
            InputPath = EditorGUILayout.TextField(WindowText, InputPath);
 
            if (GUILayout.Button("Set!"))
            {
                if (string.IsNullOrWhiteSpace(InputPath) || !Directory.Exists(InputPath))
                {
                    EditorUtility.DisplayDialog("Oh", $"Bad path: \"{InputPath}\"", "ok");
                    return;   
                }
                
                InputPath.SaveToSetting(SettingName);
                EditorUtility.DisplayDialog("Success", $"Path set successfully: \"{InputPath}\"", "ok");
                Close();
            }
 
            if (GUILayout.Button("Abort"))
                Close();
        }


        public new void ShowUtility()
        {
            InputPath = SettingName.LoadFromSettings();
            base.ShowUtility();
        }
 
        [MenuItem("CoreProject/Set CoreProject LayoutAssets folder")]
        static void SetCoreProjectLayoutAssetsFolder()
        {
            var window = CreateInstance<PropsSaver>();
            window.WindowText = "Please set path to LayoutAssets folder in you target project";
            window.SettingName = ResourcesImporter.Settings.PathToCoreProjectAsstes;
            window.ShowUtility();
        }
        
    }
}