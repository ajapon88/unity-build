using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityCloudBuild
{
    [System.Serializable]
    public class UnityCloudBuildManifest
    {
        public string scmCommitId = null;
        public string scmBranch = null;
        public string buildNumber = null;
        public string buildStartTime = null;
        public string bundleId = null;
        public string projectId = null;
        public string unityVersion = null;
        public string xcodeVersion = null;
        public string cloudBuildTargetName = null;
    }
}