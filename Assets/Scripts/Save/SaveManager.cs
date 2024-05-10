using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.Save
{
    /// <summary>
    ///     The class responsible for saving and loading the game.
    /// </summary>
    [CreateAssetMenu(fileName = "SaveManager", menuName = "SaveManager")]
    public class SaveManager : ScriptableObject
    {
        /// <summary>
        ///     The path to save and load to. Automatically set if not manually specified.
        /// </summary>
        [NonSerialized] public string savePath;

        public static string DefaultSavePath => Application.persistentDataPath + "/savedata.save";


        /// <summary>
        ///     Saves the game to a json file. Can be called by right clicking the SaveManager object in the inspector.
        /// </summary>
        [ContextMenu("Save Game")]
        public void Save()
        {
            if (savePath is null || savePath == "")
            {
                savePath = DefaultSavePath;
            }
            var saveData = new JObject();
            if (File.Exists(savePath))
            {
                saveData = JObject.Parse(File.ReadAllText(savePath));
            }

            var savableObjects = FindObjectsOfType<SavableEntity>();
            var saveDict = new Dictionary<string, object>();
            foreach (var entity in savableObjects)
            {
                saveDict[entity.Id] = entity.Save();
            }

            saveData.Merge(JObject.FromObject(saveDict), new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Replace
            });

            File.WriteAllText(savePath, saveData.ToString(Formatting.Indented));
        }

        /// <summary>
        ///     Tries to load a save game using the current savePath.
        /// </summary>
        /// <returns>True if loading succeeded, false otherwise</returns>
        [ContextMenu("Load Game")]
        public bool Load()
        {
            if (savePath is null || savePath == "")
            {
                savePath = DefaultSavePath;
            }
            if (!File.Exists(savePath))
            {
                return false;
            }
            var saveData = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(File.ReadAllText(savePath));
            var savableObjects = FindObjectsOfType<SavableEntity>();
            if (saveData is null)
            {
                return false;
            }
            foreach (var entity in savableObjects)
            {
                if (saveData.ContainsKey(entity.Id))
                {
                    entity.Load(saveData[entity.Id].ToObject<Dictionary<string, JObject>>());
                }
            }
            return true;
        }
    }
}
