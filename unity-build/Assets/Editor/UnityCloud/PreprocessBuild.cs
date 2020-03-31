using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

#if !UNITY_CLOUD_BUILD
namespace UnityCloud
{
    public class PreprocessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 100; } }

        public static readonly string ManifestPath = "Assets/UnityCloud/Resources/UnityCloudBuildManifest.json.txt";

        public void OnPreprocessBuild(BuildReport report)
        {
            bool isExportManifest = Config.IsExportManifest();
            if (Application.isBatchMode)
            {
                if (System.Environment.GetCommandLineArgs().Any(arg => arg == "-no-export-unity-cloud-build-manifest"))
                {
                    isExportManifest = false;
                }
            }
#if NO_EXPORT_UNITY_CLOUD_BUILD_MANIFEST
            isExportManifest = false;
#endif
            if (isExportManifest)
            {
                Debug.Log("Create UnityCloudBuildManigest");

                var manifest = new UnityCloudBuildManifest();
                manifest.scmCommitId = Config.GetEnvironmentVariable("UNITY_SCM_COMMIT_ID") ?? GetScmCommitId();
                manifest.scmBranch = Config.GetEnvironmentVariable("UNITY_SCM_BRANCH") ?? GetScmBranch();
                manifest.buildNumber = Config.GetEnvironmentVariable("UNITY_BUILD_NUMBER");
                manifest.buildStartTime = Config.GetEnvironmentVariable("UNITY_BUILD_START_TIME") ?? report.summary.buildStartedAt.ToLocalTime().ToString("G");
                manifest.projectId = Config.GetEnvironmentVariable("UNITY_PROJECT_ID", CloudProjectSettings.projectId);
                manifest.bundleId = PlayerSettings.applicationIdentifier;
                manifest.unityVersion = UnityEditorInternal.InternalEditorUtility.GetFullUnityVersion();
                manifest.xcodeVersion = Config.GetEnvironmentVariable("UNITY_XCODE_VERSION") ?? GetXcodeVersion();
                manifest.cloudBuildTargetName = Config.GetEnvironmentVariable("UNITY_CLOUD_BUILD_TARGET_NAME"); // default-web/default-ios/default-android

                var json = JsonUtility.ToJson(manifest, true);
                Debug.LogFormat("UnityCloudBuildManifest\n{0}", json);

                if (!Directory.Exists(Path.GetDirectoryName(ManifestPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(ManifestPath));
                }
                File.WriteAllText(ManifestPath, json, Encoding.UTF8);
            }
            else if (File.Exists(ManifestPath))
            {
                File.Delete(ManifestPath);
            }
            AssetDatabase.Refresh();
        }

        static int RunCommand(string command, string args, out string stdout)
        {
            int exitCode = 0;
            stdout = "";
            try
            {
                using (var p = new System.Diagnostics.Process())
                {
                    p.StartInfo.FileName = command;
                    p.StartInfo.Arguments = args;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardInput = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.EnableRaisingEvents = true;

                    p.Start();

                    stdout = p.StandardOutput.ReadToEnd().Trim();

                    p.WaitForExit();
                    exitCode = p.ExitCode;
                    p.Close();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                exitCode = 128;
            }
            return exitCode;
        }

        static bool IsCommandExists(string command)
        {
            var os = Environment.OSVersion;
            string checkCommand;
            switch (os.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    checkCommand = "where";
                    break;
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    checkCommand = "type";
                    break;
                default:
                    Debug.LogWarningFormat("Invalid platform: {0}", os.Platform);
                    return false;
            }

            return RunCommand(checkCommand, command, out _) == 0;
        }

        string GetScmCommitId()
        {
            if (!IsCommandExists("git"))
            {
                return "";
            }
            string stdout;
            var exitCode = RunCommand("git", "rev-parse HEAD", out stdout);
            if (exitCode == 0)
            {
                return stdout;
            }
            Debug.LogErrorFormat("Get ScmCommitId Failed({0})", exitCode);
            return "";
        }

        string GetScmBranch()
        {
            if (!IsCommandExists("git"))
            {
                return "";
            }
            string stdout;
            var exitCode = RunCommand("git", "symbolic-ref --short HEAD", out stdout);
            if (exitCode == 0)
            {
                return stdout;
            }
            Debug.LogErrorFormat("Get ScmBranch Failed({0})", exitCode);
            return "";
        }

        public string GetXcodeVersion()
        {
            if (!IsCommandExists("xcodebuild"))
            {
                return "";
            }
            string stdout;
            var exitCode = RunCommand("xcodebuild", "-version", out stdout);
            if (exitCode == 0)
            {
                Regex reg = new Regex(@"^\s*Xcode\s+(?<xcode>[\d\.]+)\s+Build version\s+(?<build_version>[\d\w]+)\s*");
                Match match = reg.Match(stdout.Trim());
                return match.Success ? string.Format("{0}({1})", match.Groups["xcode"].Value, match.Groups["build_version"].Value) : "";
            }
            Debug.LogErrorFormat("Get Xcode Version Failed({0})", exitCode);
            return "";
        }
    }
}
#endif
