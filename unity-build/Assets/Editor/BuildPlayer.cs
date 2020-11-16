using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildPlayer
{
    private BuildTarget buildTarget;

    private BuildTarget BuildTarget
    {
        get
        {
            if (buildTarget == default(BuildTarget))
            {
                buildTarget = EditorUserBuildSettings.activeBuildTarget;
            }
            return buildTarget;
        }
        set
        {
            buildTarget = value;
        }
    }

    public static void Build()
    {
        new BuildPlayer().Execute();
    }

    [MenuItem("Project/Build/iOS")]
    public static void Build_iOS()
    {
        new BuildPlayer()
        {
            BuildTarget = BuildTarget.iOS,
        }.Execute();
    }

    [MenuItem("Project/Build/Android")]
    public static void Build_Android()
    {
        new BuildPlayer()
        {
            BuildTarget = BuildTarget.Android,
        }.Execute();
    }

    private void Execute()
    {
        string locationPathName = Path.GetFullPath(Path.Combine(Application.dataPath, "../Build/", BuildTarget.ToString()));
        bool isBuildTest = System.Environment.GetCommandLineArgs().Any(arg => arg == "-buildTest");
        switch (BuildTarget)
        {
            case BuildTarget.Android:
                string extension = EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk";
                locationPathName = Path.Combine(locationPathName, $"{Application.productName}{extension}");
                break;
            case BuildTarget.iOS:
                if (isBuildTest)
                {
                    PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
                }
                break;
        }
        var options = new BuildPlayerOptions
        {
            target = BuildTarget,
            locationPathName = locationPathName,
            scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray()
        };

        var buildReport = BuildPipeline.BuildPlayer(options);
        if (buildReport.summary.result != BuildResult.Succeeded)
        {
            throw new UnityEditor.Build.BuildFailedException(string.Format("Build Error!!!\nTotalErrors: {0}", buildReport.summary.totalErrors));
        }
        Debug.LogFormat("Build Succeeded!!\nOutputPath: {0}", buildReport.summary.outputPath);
    }
}
