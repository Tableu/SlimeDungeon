using Newtonsoft.Json.Linq;

namespace Systems.Save
{
    public interface ISavable
    {
        /// <summary>
        ///     A unique ID for this component.
        ///     Needs to be unique across all components attached to a game-object,
        ///     does not need to be globally unique.
        /// </summary>
        public string id { get; }

        /// <summary>
        ///     Save the component state into a serializable object
        /// </summary>
        /// <returns>A serializable object</returns>
        object SaveState();
        /// <summary>
        ///     Restore the component state from a JObject representation of the object returned in SaveState
        /// </summary>
        /// <param name="state">The object returned in SaveState, converted to a JObject</param>
        /// <remarks>
        ///     Original save data object can be recovered via state.ToObject call
        /// </remarks>
        void LoadState(JObject state);
    }
}
