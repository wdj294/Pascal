// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// A ScriptableObject that holds a list of quest assets.
    /// </summary>
    //---  No [CreateAssetMenu]. Hide from asset menu; use wrapper class instead.
    public class QuestDatabase : ScriptableObject
    {

        [Tooltip("This description field is for your internal reference. Not seen by the player.")]
        [TextArea]
        [SerializeField]
        private string m_description;

        [Tooltip("Quests to include in the database.")]
        [SerializeField]
        private List<Quest> m_questAssets = new List<Quest>();

        /// <summary>
        /// This description field is for your internal reference. Not seen by the player.
        /// </summary>
        public string description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// Quest assets to include in the database.
        /// </summary>
        public List<Quest> questAssets { get { return m_questAssets; } }

    }
}
