using UnityEditor;

public static class BuildTargetExtension
{
    public static BuildTargetGroup ToBuildTargetGroup(this BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case BuildTarget.iOS:
                return BuildTargetGroup.iOS;
            case BuildTarget.Android:
                return BuildTargetGroup.Android;
            case BuildTarget.StandaloneOSX:
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
                return BuildTargetGroup.Standalone;
            default:
                throw new System.Exception(string.Format("Invalid BuildTarget: {0}", buildTarget));
        }
    }
}
