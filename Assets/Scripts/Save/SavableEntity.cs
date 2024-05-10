using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Systems.Save
{
    /// <summary>
    ///     Handles saving and loading of all ISavable components attached to the gameobject
    /// </summary>
    public class SavableEntity : MonoBehaviour
    {
        /// <summary>
        ///     A unique ID for this gameobject
        /// </summary>
        /// <remarks>
        ///     Used in save files, so must be unique within whole project
        /// </remarks>
        [SerializeField] private string id;
        public string Id => id;

        /// <summary>
        ///     Generates a dictionary containing the save data for each savable component
        /// </summary>
        /// <returns>The save data</returns>
        public Dictionary<string, object> Save()
        {
            var savableComponents = GetComponents<ISavable>();
            var components = new Dictionary<string, object>();
            foreach (var savable in savableComponents)
            {
                components[savable.id] = savable.SaveState();
            }
            return components;
        }

        /// <summary>
        ///     Restores the state of all savable components from save data
        /// </summary>
        /// <param name="savedState">The save dictionary for this component</param>
        public void Load(Dictionary<string, JObject> savedState)
        {
            var savableComponents = GetComponents<ISavable>();
            foreach (var savable in savableComponents)
            {
                savable.LoadState(savedState[savable.id]);
            }
        }
    }
}
