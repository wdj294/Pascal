// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Entity types are abstract definitions of entities. Every entity in the game world has an entity type that defines
    /// its attributes such as its faction with other entities and actions that can be performed on it. The quest generator
    /// needs to work with abstract entity types because actual instances of the entities may not exist in the world at
    /// the time a quest is being generated.
    /// </summary>
    public class EntityType : ScriptableObject
    {

        [Tooltip("Description of this entity type.")]
        [TextArea]
        [SerializeField]
        private string m_description;

        [Tooltip("There is only one entity of this type.")]
        [SerializeField]
        private bool m_isUnique;

        [Tooltip("The display name of this entity type.")]
        [SerializeField]
        private StringField m_displayName;

        [Tooltip("The plural display name for multiples of this entity type.")]
        [SerializeField]
        private StringField m_pluralDisplayName;

        [Tooltip("The entity type's image may be shown in UIs.")]
        [SerializeField]
        private Sprite m_image;

        [Tooltip("The entity type's level, used to determine quest difficulty and rewards.")]
        [SerializeField]
        private int m_level = 1;

        [Tooltip("The faction that this entity type belongs to.")]
        [SerializeField]
        private Faction m_faction;

        [Tooltip("The entity type's parent types.")]
        [SerializeField]
        private List<EntityType> m_parents;

        [Tooltip("Functions that quest generators use to determine how urgently they must generate a quest about this entity.")]
        [SerializeField]
        private List<UrgencyFunction> m_urgencyFunctions;

        [Tooltip("Actions that can be performed on this entity type.")]
        [SerializeField]
        private List<Action> m_actions;

        [Tooltip("This entity type's drives. Used by quest generators to decide on targets and actions.")]
        [SerializeField]
        private List<DriveValue> m_driveValues;

        [NonSerialized]
        private StringField m_internalAssetName = null;

        [NonSerialized]
        private StringField m_internalPluralDisplayName = null;

        /// <summary>
        /// Description of this entity type.
        /// </summary>
        public string description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// True if there is only one entity of this type.
        /// </summary>
        public bool isUnique
        {
            get { return m_isUnique; }
            set { m_isUnique = value; }
        }

        /// <summary>
        /// The display name of this entity type.
        /// </summary>
        public StringField displayName
        {
            get
            {
                if (!StringField.IsNullOrEmpty(m_displayName))
                {
                    return m_displayName;
                }
                else
                {
                    if (StringField.IsNullOrEmpty(m_internalAssetName)) m_internalAssetName = new StringField(name);
                    return m_internalAssetName;
                }
            }
            set { m_displayName = value; }
        }

        /// <summary>
        /// The plural display name for multiples of this entity type.
        /// </summary>
        public StringField pluralDisplayName
        {
            get
            {
                if (!StringField.IsNullOrEmpty(m_pluralDisplayName))
                {
                    return m_pluralDisplayName;
                }
                else
                {
                    if (StringField.IsNullOrEmpty(m_internalPluralDisplayName))
                    {
                        var plural = displayName.ToString();
                        plural += plural.EndsWith("s") ? "es" : "s";
                        m_internalPluralDisplayName = new StringField(plural);
                    }
                    return m_internalPluralDisplayName;
                }
            }
            set { m_pluralDisplayName = value; }
        }

        /// <summary>
        /// The entity type's image may be shown in UIs.
        /// </summary>
        public Sprite image
        {
            get { return m_image; }
            set { m_image = value; }
        }

        /// <summary>
        /// The entity type's level, used to determine quest difficulty and rewards.
        /// </summary>
        public int level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        /// <summary>
        /// The faction that this entity type belongs to.
        /// </summary>
        public Faction faction
        {
            get { return m_faction; }
            set { m_faction = value; }
        }

        /// <summary>
        /// The entity type's parent types.
        /// </summary>
        public List<EntityType> parents
        {
            get { return m_parents; }
            set { m_parents = value; }
        }

        /// <summary>
        /// Functions that quest generators use to determine how urgently they must generate a quest about this entity.
        /// </summary>
        public List<UrgencyFunction> urgencyFunctions
        {
            get { return m_urgencyFunctions; }
            set { m_urgencyFunctions = value; }
        }

        /// <summary>
        /// Actions that can be performed on this entity type.
        /// </summary>
        public List<Action> actions
        {
            get { return m_actions; }
            set { m_actions = value; }
        }

        /// <summary>
        /// This entity type's drives. Used by quest generators to decide on targets and actions.
        /// </summary>
        public List<DriveValue> driveValues
        {
            get { return m_driveValues; }
            set { m_driveValues = value; }
        }

        /// <summary>
        /// Returns a text descriptor for a specified number of this entity type.
        /// </summary>
        /// <param name="count">The number of entities.</param>
        public string GetDescriptor(int count)
        {
            if (isUnique || count == 1) return StringField.GetStringValue(displayName);
            return count + " " + pluralDisplayName;
        }

        /// <summary>
        /// Looks up this entity type's faction.
        /// </summary>
        /// <returns>This entity type's faction, or its parent's faction if unassigned.</returns>
        public Faction GetFaction()
        {
            if (faction != null) return faction;
            foreach (var parent in parents)
            {
                var result = parent.GetFaction();
                if (result != null) return result;
            }
            return null;
        }

    }
}
