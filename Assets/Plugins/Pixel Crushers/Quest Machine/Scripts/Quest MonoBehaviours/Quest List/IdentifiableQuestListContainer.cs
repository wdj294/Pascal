// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// A quest list with an ID.
    /// </summary>
    [AddComponentMenu("")] // Just a base class.
    public class IdentifiableQuestListContainer : QuestListContainer
    {

        #region Serialized Fields

        [Tooltip("The ID that uniquely identifies this entity. If unassigned, uses the Quest Entity's Display Name if present.")]
        [SerializeField]
        protected StringField m_id = new StringField();

        [Tooltip("The name shown in UIs. If blank, uses the Quest Entity's Display Name if present.")]
        [SerializeField]
        protected StringField m_displayName = new StringField();

        [Tooltip("The image shown in UIs. If unassigned, uses the Quest Entity's Image if present.")]
        [SerializeField]
        protected Sprite m_image;

        #endregion

        #region Property Accessors to Serialized Fields

        private StringField m_fallbackID = null; // Use this ID (QuestEntity's) if id QuestGiver's ID isn't set.

        /// <summary>
        /// The ID that uniquely identifies this quest giver. When the quester (e.g., player) accepts
        /// a quest from this quest giver, the quester's instance will have a reference to this ID so
        /// the quester knows who gave the quest.
        /// </summary>
        public StringField id
        {
            get
            {
                if (!StringField.IsNullOrEmpty(m_id)) return m_id;
                if (StringField.IsNullOrEmpty(m_fallbackID))
                {
                    var entity = GetComponentInChildren<QuestEntity>();
                    m_fallbackID = (entity != null) ? entity.displayName : new StringField(name);
                }
                return m_fallbackID;
            }
            set
            {
                m_id = value;
            }
        }

        /// <summary>
        /// The name shown in UIs. If blank, uses the Quest Entity's Display Name if present.
        /// </summary>
        public StringField displayName
        {
            get { return m_displayName; }
            set { m_displayName = value; }
        }

        /// <summary>
        /// The image shown in UIs. If blank, uses the Quest Entity's Image if present.
        /// </summary>
        public Sprite image
        {
            get { return m_image; }
            set { m_image = value; }
        }

        #endregion

    }

}
