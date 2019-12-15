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

    [MenuItem("Project/Build/Player")]
    public static void Build()
    {
        new BuildPlayer().Execute();
    }

    public static void Build_iOS()
    {
        new BuildPlayer()
        {
            BuildTarget = BuildTarget.iOS,
        }.Execute();
    }

    public static void Build_Android()
    {
        new BuildPlayer()
        {
            BuildTarget = BuildTarget.Android,
        }.Execute();
    }

    private void Execute()
    {
        string locationPathName = System.IO.Path.Combine(Application.dataPath, "../Build/", BuildTarget.ToString());
        if (BuildTarget == BuildTarget.Android)
        {
            string extension = EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk";
            locationPathName = System.IO.Path.Combine(locationPathName, $"{Application.productName}{extension}");
        }
        var options = new BuildPlayerOptions
        {
            target = BuildTarget,
            targetGroup = BuildTarget.ToBuildTargetGroup(),
            locationPathName = locationPathName,
            scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray()
        };

        BuildPipeline.BuildPlayer(options);
    }
}
