using System;
using UnityEngine;
using UnityEditor;

namespace UnityCloudBuild
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
            return ExportManifestFlag;
        }
    }
}
