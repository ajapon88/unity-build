using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;

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
        if (BuildTarget == BuildTarget.Android)
        {
            string extension = EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk";
            locationPathName = Path.Combine(locationPathName, $"{Application.productName}{extension}");
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
