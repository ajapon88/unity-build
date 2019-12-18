using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
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

    private BuildTargetGroup BuildTargetGroup
    {
        get { return BuildPipeline.GetBuildTargetGroup(BuildTarget); }
    }

    [MenuItem("Project/Build/ActiveBuildTarget")]
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
        string locationPathName = Path.Combine(Application.dataPath, "../Build/", BuildTarget.ToString());
        if (BuildTarget == BuildTarget.Android)
        {
            string extension = EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk";
            locationPathName = Path.Combine(locationPathName, $"{Application.productName}{extension}");
        }
        var options = new BuildPlayerOptions
        {
            target = BuildTarget,
            targetGroup = BuildTargetGroup,
            locationPathName = locationPathName,
            scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray()
        };

        BuildPipeline.BuildPlayer(options);
    }
}
