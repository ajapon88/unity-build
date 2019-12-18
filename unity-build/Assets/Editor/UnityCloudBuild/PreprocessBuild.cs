using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

#if !UNITY_CLOUD_BUILD
namespace UnityCloudBuild
{
    public class PreprocessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 100; } }

        public readonly string ManifestPath = "Assets/UnityCloud/Resources/UnityCloudBuildManifest.json.txt";

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("Create UnityCloudBuildManigest");

            var manifest = new UnityCloudBuildManifest();
            manifest.scmCommitId = GetScmCommitId();
            manifest.scmBranch = GetScmBranch();
            manifest.buildNumber = GetEnvironmentVariable("UNITY_BUILD_NUMBER");
            manifest.buildStartTime = GetEnvironmentVariable("UNITY_BUILD_START_TIME", DateTime.Now.ToString());
            manifest.projectId = GetEnvironmentVariable("UNITY_PROJECT_ID", CloudProjectSettings.projectId);
            manifest.bundleId = PlayerSettings.applicationIdentifier;
            manifest.unityVersion = UnityEditorInternal.InternalEditorUtility.GetFullUnityVersion();
            manifest.xcodeVersion = GetXcodeVersion();
            manifest.cloudBuildTargetName = GetEnvironmentVariable("UNITY_CLOUD_BUILD_TARGET_NAME"); // default-web/default-ios/default-android

            var json = JsonUtility.ToJson(manifest, true);
            Debug.LogFormat("UnityCloudBuildManifest\n{0}", json);

            if (!Directory.Exists(Path.GetDirectoryName(ManifestPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ManifestPath));
            }
            File.WriteAllText(ManifestPath, json, Encoding.UTF8);

            AssetDatabase.Refresh();
        }

        public string GetEnvironmentVariable(string variable, string defaultValue = null)
        {
            var envs = Environment.GetEnvironmentVariables();
            return envs.Contains(variable) ? (string)envs[variable] : defaultValue;
        }

        int RunCommand(string command, string args, out string stdout)
        {
            int exitCode = 0;
            stdout = "";
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
            return exitCode;
        }

        string GetScmCommitId()
        {
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
