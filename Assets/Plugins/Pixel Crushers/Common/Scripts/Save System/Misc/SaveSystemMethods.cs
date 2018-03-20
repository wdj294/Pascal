// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Provides inspector-selectable methods to control SaveSystem.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class SaveSystemMethods : MonoBehaviour
    {

        /// <summary>
        /// Saves the current game in the specified slot.
        /// </summary>
        /// <param name="slotNumber"></param>
        public void SaveSlot(int slotNumber)
        {
            SaveSystem.SaveToSlot(slotNumber);
        }

        /// <summary>
        /// Loads the game previously-saved in the specified slot.
        /// </summary>
        /// <param name="slotNumber"></param>
        public void LoadFromSlot(int slotNumber)
        {
            SaveSystem.LoadFromSlot(slotNumber);
        }

        /// <summary>
        /// Changes scenes. You can optionally specify a player spawnpoint by 
        /// adding '@' and the spawnpoint GameObject name.
        /// </summary>
        /// <param name="sceneNameAndSpawnpoint"></param>
        public void LoadScene(string sceneNameAndSpawnpoint)
        {
            SaveSystem.LoadScene(sceneNameAndSpawnpoint);
        }

        /// <summary>
        /// Resets all saved game data and restarts the game at the specified scene.
        /// </summary>
        /// <param name="startingSceneName"></param>
        public void RestartGame(string startingSceneName)
        {
            SaveSystem.RestartGame(startingSceneName);
        }

    }

}
