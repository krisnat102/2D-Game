using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Krisnat.Assets.Scripts
{
    public static class SaveSystem
    {
        private static readonly object fileLock = new();

        public static bool HasLoadFile()
        {
            string path = Application.persistentDataPath + "/player.bob";
            return File.Exists(path);
        }

        public static void SavePlayer(Player player)
        {
            lock (fileLock)
            {
                string path = Application.persistentDataPath + "/player.bob";

                PlayerSaveData data = new(player);
                BinaryFormatter formatter = new();

                using (FileStream stream = new(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(stream, data);
                }
            }
        }

        public static void SaveData(PlayerSaveData data)
        {
            string path = Application.persistentDataPath + "/player.bob";
            BinaryFormatter formatter = new();

            using (FileStream stream = new(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, data);
            }
        }


        public static PlayerSaveData LoadPlayer()
        {
            string path = Application.persistentDataPath + "/player.bob";

            if (File.Exists(path))
            {
                FileInfo fileInfo = new(path);

                if (fileInfo.Length == 0) // Check if the file is empty
                {
                    return null;
                }

                try
                {
                    BinaryFormatter formatter = new();

                    using (FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        return formatter.Deserialize(stream) as PlayerSaveData;
                    }
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #if UNITY_EDITOR
        [MenuItem("Tools/DeleteSaveFile")]
#endif
        public static void DeleteAllSaveFiles()
        {
            string path = Application.persistentDataPath;

            if (Directory.Exists(path))
            {
                DirectoryInfo directory = new DirectoryInfo(path);

                foreach (FileInfo file in directory.GetFiles())
                {
                    try
                    {
                        file.Attributes = FileAttributes.Normal; // Remove read-only attribute
                        file.Delete();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to delete file {file.Name}: {ex.Message}");
                    }
                }

                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    try
                    {
                        dir.Delete(true);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to delete directory {dir.Name}: {ex.Message}");
                    }
                }

                Debug.Log("All save files deleted successfully.");
            }
            else
            {
                Debug.LogWarning("Persistent data path directory does not exist.");
            }
        }

    }
}
