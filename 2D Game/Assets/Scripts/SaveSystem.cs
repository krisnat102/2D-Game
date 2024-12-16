using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace Krisnat.Assets.Scripts
{
    public static class SaveSystem
    {
        public static bool HasLoadFile()
        {
            string path = Application.persistentDataPath + "/player.bob";
            return File.Exists(path);
        }

        public static void SavePlayer(Player player)
        {
            BinaryFormatter formatter = new();
            string path = Application.persistentDataPath + "/player.bob";
            FileStream stream = new(path, FileMode.Create);

            PlayerSaveData data = new(player);

            formatter.Serialize(stream, data);
            stream.Close();
        }

        public static void SaveData(PlayerSaveData data)
        {
            BinaryFormatter formatter = new();
            string path = Application.persistentDataPath + "/player.bob";
            FileStream stream = new(path, FileMode.Create);

            formatter.Serialize(stream, data);
            stream.Close();
        }

        public static PlayerSaveData LoadPlayer()
        {
            string path = Application.persistentDataPath + "/player.bob";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new();
                FileStream stream = new(path, FileMode.Open);

                PlayerSaveData data = formatter.Deserialize(stream) as PlayerSaveData;
                stream.Close();

                return data;
            }
            else
            {
                Debug.LogWarning("Save file not found in" + path);
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
