// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;

namespace PixelCrushers
{

    /// <summary>
    /// Manages localization settings.
    /// </summary>
    public class UILocalizationManager : MonoBehaviour
    {

        [Tooltip("The PlayerPrefs key to store the player's selected language code.")]
        [SerializeField]
        private string m_currentLanguagePlayerPrefsKey = "Language";

        [Tooltip("Overrides the global text table.")]
        [SerializeField]
        private TextTable m_textTable = null;

        private string m_currentLanguage = string.Empty;

        public static UILocalizationManager instance { get; private set; }

        /// <summary>
        /// The PlayerPrefs key to store the player's selected language code.
        /// </summary>
        public string currentLanguagePlayerPrefsKey
        {
            get { return m_currentLanguagePlayerPrefsKey; }
            set { m_currentLanguagePlayerPrefsKey = value; }
        }

        /// <summary>
        /// Overrides the global text table.
        /// </summary>
        public TextTable textTable
        {
            get { return (m_textTable != null) ? m_textTable : GlobalTextTable.textTable; }
            set { m_textTable = value; }
        }

        public string currentLanguage
        {
            get
            {
                return m_currentLanguage;
            }
            set
            {
                m_currentLanguage = value;
                UpdateUIs(value);
            }
        }

        private void Awake()
        {
            instance = this;
            if (!string.IsNullOrEmpty(currentLanguagePlayerPrefsKey) && PlayerPrefs.HasKey(currentLanguagePlayerPrefsKey))
            {
                currentLanguage = PlayerPrefs.GetString(currentLanguagePlayerPrefsKey);
            }
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame(); // Wait for Text components to start.
            var languageCode = string.Empty;
            UpdateUIs(languageCode);
        }

        /// <summary>
        /// Updates the current language and all localized UIs.
        /// </summary>
        /// <param name="language">Language code defined in your Text Table.</param>
        public void UpdateUIs(string language)
        {
            m_currentLanguage = language;
            if (!string.IsNullOrEmpty(currentLanguagePlayerPrefsKey))
            {
                PlayerPrefs.SetString(currentLanguagePlayerPrefsKey, language);
            }
            var localizeUIs = FindObjectsOfType<LocalizeUI>();
            for (int i = 0; i < localizeUIs.Length; i++)
            {
                localizeUIs[i].UpdateText();
            }
        }

    }
}
