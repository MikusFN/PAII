using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    /// <summary>
    /// This class is used just to display two build options in the menu. So its less work each time you want to rebuild.
    /// </summary>
    public class BuildAutomaton
    {
        private const string ANDROID_SCENE_FOLDER = "/Android/Scenes/";
        private const string WINDOWS_SCENE_FOLDER = "/PC/Scenes/";

        private static string WindowsSaveDirectory = null;
        private static string AndroidSaveDirectory = null;

        [MenuItem("Build/Windows Build")]
        public static void WindowsBuild()
        {
            if (WindowsSaveDirectory == null)
            {
                ChangeWindowsBuildDirectory();
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = GetAllFileNamesFromDirectory(WINDOWS_SCENE_FOLDER),
                locationPathName = WindowsSaveDirectory + "/PC.exe",
                target = BuildTarget.StandaloneWindows,
                options = BuildOptions.Development
            };
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
        [MenuItem("Build/Windows Build and Run")]
        public static void WindowsBuildAndRun()
        {
            WindowsBuild();
            var proc = new Process();
            proc.StartInfo.FileName = WindowsSaveDirectory + "/PC.exe";
            proc.Start();
        }
        [MenuItem("Build/Change Windows Build Directory")]
        public static void ChangeWindowsBuildDirectory()
        {
            WindowsSaveDirectory = EditorUtility.SaveFolderPanel("Choose Location to build the board", "", "");
        }
        [MenuItem("Build/Mac Build and Run")]
        public static void MacBuildAndRun(){
            if (WindowsSaveDirectory == null)
            {
                ChangeWindowsBuildDirectory();
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = GetAllFileNamesFromDirectory(WINDOWS_SCENE_FOLDER),
                locationPathName = WindowsSaveDirectory + "/MacBuild",
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.AutoRunPlayer
            };
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }


        //THESE FUNCTIONS ARE NOT WORKING PROPERLY, PLEASE USE THE UNITY BUILD.
        [MenuItem("Build/Android Build")]
        private static void AndroidBuild()
        {
            if (AndroidSaveDirectory == null)
            {
                ChangeAndroidBuildDirectory();
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = GetAllFileNamesFromDirectory(ANDROID_SCENE_FOLDER),
                locationPathName = AndroidSaveDirectory + "/build.apk", //????
                target = BuildTarget.Android,
                options = BuildOptions.AutoRunPlayer
            };
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
        [MenuItem("Build/Change Android Build Directory")]
        private static void ChangeAndroidBuildDirectory()
        {
            AndroidSaveDirectory = EditorUtility.SaveFolderPanel("Choose Location to build the game", "", "");
        }

        private static string[] GetAllFileNamesFromDirectory(string directory)
        {
            DirectoryInfo d = new DirectoryInfo(Application.dataPath + directory);
            FileInfo[] Files = d.GetFiles("*.unity");
            string[] scenes = new string[Files.Length];
            for (int i = 0; i < scenes.Length; i++)
                scenes[i] = ("Assets" + directory + Files[i].Name);
            return scenes;
        }
    }
}
