// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Auto-saves when the game closes and auto-loads when the game opens.
    /// Useful for mobile games.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class AutoSaveLoad : MonoBehaviour
    {

        public int saveSlotNumber = 1;

        [Tooltip("Load the saved game when this component starts.")]
        public bool loadOnStart = true;

        [Tooltip("Save when the player quits the app.")]
        public bool saveOnQuit = true;

        [Tooltip("Save when the player pauses or minimizes the app; tick this for mobile builds.")]
        public bool saveOnPause = true;

        [Tooltip("Save when the app loses focus.")]
        public bool saveOnLoseFocus = false;

        /// <summary>
        /// When starting, load the game.
        /// </summary>
        private void Start()
        {
            if (loadOnStart && SaveSystem.HasSavedGameInSlot(saveSlotNumber))
            {
                SaveSystem.LoadFromSlot(saveSlotNumber);
            }
        }

        /// <summary>
        /// When quitting, save the game.
        /// </summary>
        private void OnApplicationQuit()
        {
            if (saveOnQuit)
            {
                SaveSystem.SaveToSlot(saveSlotNumber);
            }
        }

        /// <summary>
        /// When app is paused (e.g., minimized) and saveOnPause is true, save game.
        /// </summary>
        /// <param name="paused">True indicates game is being paused.</param>
        private void OnApplicationPause(bool paused)
        {
            if (paused && saveOnPause)
            {
                SaveSystem.SaveToSlot(saveSlotNumber);
            }
        }

        /// <summary>
        /// When app loses focus and saveOnLoseFocus is true, save the game.
        /// </summary>
        /// <param name="focusStatus">False indicates game is losing focus.</param>
        void OnApplicationFocus(bool focusStatus)
        {
            if (saveOnLoseFocus && focusStatus == false)
            {
                SaveSystem.SaveToSlot(saveSlotNumber);
            }
        }

        /// <summary>
        /// Clears the saved game data and restarts the game at a specified scene.
        /// </summary>
        /// <param name="startingSceneName"></param>
        public void Restart(string startingSceneName)
        {
            SaveSystem.DeleteSavedGameInSlot(saveSlotNumber);
            SaveSystem.RestartGame(startingSceneName);
        }

    }

}
