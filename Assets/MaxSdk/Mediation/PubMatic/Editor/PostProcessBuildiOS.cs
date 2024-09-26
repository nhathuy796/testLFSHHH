#if UNITY_IOS || UNITY_IPHONE

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;

namespace AppLovinMax.Mediation.PubMatic
{
    public class PostProcessBuildiOS
    {
        // Use a priority of 90 to have AppLovin embed frameworks after Pods are installed (EDM finishes installing Pods at priority 60) and before Firebase Crashlytics runs their scripts (at priority 100).
        [PostProcessBuild(90)]
        public static void MaxPostProcessPbxProject(BuildTarget buildTarget, string buildPath)
        {
            // Check if we should embed the dynamic libraries.
            if (!ShouldEmbedDynamicLibraries(buildPath)) return;

            var projectPath = PBXProject.GetPBXProjectPath(buildPath);
            var project = new PBXProject();
            project.ReadFromFile(projectPath);

#if UNITY_2019_3_OR_NEWER
            var unityMainTargetGuid = project.GetUnityMainTargetGuid();
#else
            var unityMainTargetGuid = project.TargetGuidByName("Unity-iPhone");
#endif

            // Check that the Pods directory exists (it might not if a publisher is building with Generate Podfile setting disabled in EDM).
            var podsDirectory = Path.Combine(buildPath, "Pods");
            if (!Directory.Exists(podsDirectory)) return;

            var dynamicLibraryPath = GetDynamicLibraryPath(podsDirectory, "OMSDK_Pubmatic.xcframework");
            if (string.IsNullOrEmpty(dynamicLibraryPath))
            {
                Debug.LogError("Failed to embed OMSDK_Pubmatic.xcframework.");
                return;
            }

            var fileGuid = project.AddFile(dynamicLibraryPath, dynamicLibraryPath);
            project.AddFileToEmbedFrameworks(unityMainTargetGuid, fileGuid);

            project.WriteToFile(projectPath);
        }

        private static string GetDynamicLibraryPath(string buildPath, string dynamicLibraryName)
        {
            // .xcframework is a directory, not a file.
            var directories = Directory.GetDirectories(buildPath, dynamicLibraryName, SearchOption.AllDirectories);
            if (directories.Length <= 0) return null;

            var dynamicLibraryAbsolutePath = directories[0];
            var index = dynamicLibraryAbsolutePath.LastIndexOf("Pods", StringComparison.Ordinal);
            return dynamicLibraryAbsolutePath.Substring(index);
        }

        /// <summary>
        /// Do not embed the dynamic libraries if the Podfile has a `Unity-iPhone` target and `use_frameworks!` is not present.
        /// NOTE: We generally shouldn't embed if `use_frameworks!` is present, but PubMatic Podspec has a bug that requires us to embed them even when `use_frameworks!` is present.
        /// </summary>
        /// <param name="buildPath">An iOS build path</param>
        /// <returns>Whether or not the dynamic libraries should be embedded.</returns>
        private static bool ShouldEmbedDynamicLibraries(string buildPath)
        {
            var podfilePath = Path.Combine(buildPath, "Podfile");
            if (!File.Exists(podfilePath)) return false;

            // If the Podfile doesn't have a `Unity-iPhone` target, we should embed the dynamic libraries.
            var lines = File.ReadAllLines(podfilePath);
            var containsUnityIphoneTarget = lines.Any(line => line.Contains("target 'Unity-iPhone' do"));
            if (!containsUnityIphoneTarget) return true;

            // If the Podfile has any `use_frameworks!` line, we should embed the dynamic library.
            return lines.Any(line => line.Contains("use_frameworks!"));
        }
    }
}

#endif
