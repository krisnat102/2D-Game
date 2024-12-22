using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
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
                catch (SerializationException ex)
                {
                    return null;
                }
            }
            else
            {
                Debug.LogWarning("Save file not found in " + path);
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
                    file.Delete(); // Delete each file
                }

                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    dir.Delete(true); // Delete each subdirectory and its contents
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
