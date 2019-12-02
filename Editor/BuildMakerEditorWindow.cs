using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using BuildMaker.Utils;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

namespace BuildMaker.Editor
{
    public class BuildMakerEditorWindow : OdinEditorWindow
    {
        [HideIf("@completedBuilds.Count == 0")]
        [PropertyOrder(0)]
        [TableList]
        public List<CompletedBuildTable> completedBuilds = new List<CompletedBuildTable>();

        [FoldoutGroup("Build Profiles")]
        [PropertyOrder(1)]
        [TableList]
        public List<BuildProfileTable> buildProfiles = new List<BuildProfileTable>();

        [PropertyTooltip("If enabled, this will wipe the build directory before build")]
        [PropertyOrder(4)]
        [FoldoutGroup("Options")]
        public bool eraseDirectory;

        [PropertyTooltip("If enabled, this will play the build after it is complete")]
        [PropertyOrder(5)]
        [FoldoutGroup("Options")]
        public bool playAfterBuild;

        [PropertyTooltip("If enabled, this will open up our Build directory after the build is complete")]
        [PropertyOrder(6)]
        [FoldoutGroup("Options")]
        public bool openFolder;

        [PropertyTooltip("If enabled, this will terminate the same process that we are about to build if it is still running.")]
        [PropertyOrder(7)]
        [FoldoutGroup("Options")]
        public bool killOldProcesses;

        [PropertyTooltip("If enabled, this will build all assets marked as Addressable.")]
        [PropertyOrder(3)]
        [FoldoutGroup("Addressable Asset Settings")]
        public bool buildAddressables;

        [FoldoutGroup("Build Profiles")]
        [PropertyOrder(2)]
        [Button]
        public void AddBuildProfile()
        {
            buildProfiles.Add(new BuildProfileTable());
        }

        [PropertyTooltip("LEEEEEERRRRRROOOOOYYYYY JJJJJJEEEEENNNNKKKKIIIIINNNNSSSS!!!")]
        [PropertyOrder(5)]
        [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        private void Build()
        {
            Debug.Log("Build started!");

            for (int i = 0; i < buildProfiles.Count; i++)
            {
                if(buildProfiles[i].Enabled)
                    Debug.Log("Active build profiles: " + i + buildProfiles[i].ScriptableObject.name);
            }

            if (buildAddressables)
            {
                BuildAddressables();
            }

            ExecuteBuild();
        }

        private void BuildAddressables()
        {
            Debug.Log("Building addressables...");
            AddressableAssetSettings.CleanPlayerContent(
                AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log("Addressable build complete!");
        }

        private void ExecuteBuild()
        {
            foreach (BuildProfileTable profile in buildProfiles)
            {
                if (!profile.Enabled)
                {
                    continue;
                }

                if (!PreBuildChecks.Check(profile))
                {
                    BuildFailed();
                    return;
                }

                // We want to shut down the process if its already running
                if (killOldProcesses)
                {
                    CheckIfProcessRunning(profile);
                }

                if (eraseDirectory)
                {
                    FileUtil.DeleteFileOrDirectory(profile.ScriptableObject.directory);
                }

                string buildFullLocation = profile.ScriptableObject.directory +
                                           profile.ScriptableObject.executableName +
                                           SetExecutableExtension.Get(profile.ScriptableObject.platform);

                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
                {
                    locationPathName = buildFullLocation,
                    //options = profile.ScriptableObject.buildOptions,
                    scenes = profile.ScriptableObject.sceneDirectories,
                    //target = ConvertToUnityBuildTarget.Convert(profile.ScriptableObject.platform)
                    target = BuildTarget.StandaloneWindows64
                };

                BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
                BuildSummary summary = buildReport.summary;

                if (summary.result == BuildResult.Succeeded)
                {
                    BuildSucceeded(summary, buildFullLocation, profile.ScriptableObject.args);
                    AddCompletedBuild(profile);
                }
            }
        }

        private void BuildSucceeded(BuildSummary summary, string buildFullLocation, string args)
        {
            Debug.Log("Build " + summary.result + "!");

            //TODO: If doing multiple builds, only the last one gets launched after builds completed
            if (playAfterBuild)
            {
                Process proc = new Process();
                proc.StartInfo.FileName = buildFullLocation;
                proc.StartInfo.Arguments = args;
                proc.Start();
            }

            if (openFolder)
            {
                EditorUtility.RevealInFinder(buildFullLocation);
            }
        }

        private void AddCompletedBuild(BuildProfileTable profile)
        {
            CompletedBuildTable newItem = new CompletedBuildTable {BuildProfile = profile.ScriptableObject};
            newItem.timestamp = DateTime.Now.ToString(CultureInfo.CurrentCulture);

            // If we build this build profile already, replace it in the list of CompletedBuilds
            foreach (CompletedBuildTable build in completedBuilds)
            {
                if (build.BuildProfile.name == newItem.BuildProfile.name)
                {
                    completedBuilds.Remove(build);
                    break;
                }
            }

            completedBuilds.Add(newItem);
        }

        private void CheckIfProcessRunning(BuildProfileTable profile)
        {
            Process[] processes = Process.GetProcessesByName(profile.ScriptableObject.executableName);

            if (processes.Length > 0)
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    Debug.LogWarning("Process " + processes[i].ProcessName + " was killed because it may prevent a build from running");
                    processes[i].Kill();
                }
            }
        }

        private void BuildFailed()
        {
            Debug.LogError("Build failed!");
        }

        [MenuItem("Build/Build Maker")]
        private static void OpenWindow()
        {
            GetWindow<BuildMakerEditorWindow>().Show();
        }
    }
}
