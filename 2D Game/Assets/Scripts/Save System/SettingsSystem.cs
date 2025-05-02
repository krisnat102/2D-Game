using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    using UnityEngine;
    using System.IO;

    public static class SettingsSystem
    {
        private static readonly string savePath = Application.persistentDataPath + "/settings.json";

        public static void SaveSettings(SettingsData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
        }

        public static SettingsData LoadSettings()
        {
            if (!File.Exists(savePath))
            {
                var defaultSettings = new SettingsData();
                SaveSettings(defaultSettings);
                return defaultSettings;
            }

            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<SettingsData>(json);
        }
    }

}
