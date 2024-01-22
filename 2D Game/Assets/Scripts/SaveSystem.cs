using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

namespace Krisnat.Assets.Scripts
{
    public static class SaveSystem
    {
        public static void SavePlayer(Player player)
        {
            BinaryFormatter formatter = new();
            string path = Application.persistentDataPath + "/player.bob";
            FileStream stream = new(path, FileMode.Create);

            PlayerSaveData data = new(player);

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
                Debug.LogError("Save file not found in" + path);
                return null;
            }
        }
    }
}
