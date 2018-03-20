// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Implements SavedGameDataStorer using PlayerPrefs.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class PlayerPrefsSavedGameDataStorer : SavedGameDataStorer
    {

        [Tooltip("Save games under this PlayerPrefs key")]
        [SerializeField]
        private string m_playerPrefsKeyBase = "Save";

        [Tooltip("Log debug info.")]
        [SerializeField]
        private bool m_debug = false;

        public string playerPrefsKeyBase
        {
            get { return m_playerPrefsKeyBase; }
            set { m_playerPrefsKeyBase = value; }
        }

        public bool debug
        {
            get { return m_debug && Debug.isDebugBuild; }
        }

        public string GetPlayerPrefsKey(int slotNumber)
        {
            return m_playerPrefsKeyBase + slotNumber;
        }

        public override bool HasDataInSlot(int slotNumber)
        {
            return PlayerPrefs.HasKey(GetPlayerPrefsKey(slotNumber));
        }

        public override void StoreSavedGameData(int slotNumber, SavedGameData savedGameData)
        {
            var s = SaveSystem.Serialize(savedGameData);
            if (debug) Debug.Log("Save System: Storing in PlayerPrefs key " + GetPlayerPrefsKey(slotNumber) + ": " + s);
            PlayerPrefs.SetString(GetPlayerPrefsKey(slotNumber), s);
        }

        public override SavedGameData RetrieveSavedGameData(int slotNumber)
        {
            if (debug && HasDataInSlot(slotNumber)) Debug.Log("Save System: Retrieved from PlayerPrefs key " +
                GetPlayerPrefsKey(slotNumber) + ": " + PlayerPrefs.GetString(GetPlayerPrefsKey(slotNumber)));
            return HasDataInSlot(slotNumber) ? SaveSystem.Deserialize<SavedGameData>(PlayerPrefs.GetString(GetPlayerPrefsKey(slotNumber))) : new SavedGameData();
        }

        public override void DeleteSavedGameData(int slotNumber)
        {
            PlayerPrefs.DeleteKey(GetPlayerPrefsKey(slotNumber));
        }

    }

}
