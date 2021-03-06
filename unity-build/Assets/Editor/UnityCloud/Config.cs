﻿using System;
using UnityEngine;
using UnityEditor;

namespace UnityCloud
{
    public static class Config
    {
        const string MenuPath = "Project/UnityCloudBuildManiest/Export";

        static bool ExportManifestFlag
        {
            get
            {
                var value = EditorUserSettings.GetConfigValue(MenuPath);
                if (string.IsNullOrEmpty(value))
                {
                    value = "true";
                }
                return ToBoolean(value);
            }
            set { EditorUserSettings.SetConfigValue(MenuPath, value.ToString()); }
        }

        [MenuItem(MenuPath, true)]
        public static bool UpdateExportManifestFlag()
        {
            Menu.SetChecked(MenuPath, ExportManifestFlag);
            return !EditorApplication.isCompiling;
        }

        [MenuItem(MenuPath)]
        public static void SwitchExportManifestFlag()
        {
            ExportManifestFlag = !ExportManifestFlag;
        }

        static bool ToBoolean(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }
                if (int.TryParse(value, out int v))
                {
                    return Convert.ToBoolean(v);
                }
                return Convert.ToBoolean(value);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public static bool IsExportManifest()
        {
            // batchmodeであれば環境変数の設定を見る
            if (Application.isBatchMode)
            {
                var exportEnv = GetEnvironmentVariable("EXPORT_UNITY_CLOUD_BUILD_MANIFEST");
                if (!string.IsNullOrEmpty(exportEnv))
                {
                    return ToBoolean(exportEnv);
                }
            }
            return ExportManifestFlag;
        }

        public static string GetEnvironmentVariable(string variable, string defaultValue = null)
        {
            var envs = Environment.GetEnvironmentVariables();
            return envs.Contains(variable) ? (string)envs[variable] : defaultValue;
        }
    }
}
