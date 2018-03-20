// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Actions are tasks that quest givers can ask questers to do to an entity.
    /// </summary>
    public class Action : ScriptableObject
    {

        [Tooltip("This description field is for your internal reference.")]
        [TextArea]
        [SerializeField]
        private string m_description;

        [SerializeField]
        private Motive[] m_motives;

        [Tooltip("The display name of this action.")]
        [SerializeField]
        private StringField m_displayName;

        [Tooltip("Content for each quest node state and UI category (dialogue, journal, HUD).")]
        [SerializeField]
        private ActionText m_actionText = new ActionText();

        [SerializeField]
        private ActionRequirement[] m_requirements;

        [SerializeField]
        private ActionEffect[] m_effects;

        [SerializeField]
        private ActionCompletion m_completion;

        /// <summary>
        /// Description field for your own internal reference.
        /// </summary>
        public string description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// The display name of this action.
        /// </summary>
        public StringField displayName
        {
            get { return m_displayName; }
            set { m_displayName = value; }
        }

        public Motive[] motives
        {
            get { return m_motives; }
            set { m_motives = value; }
        }

        /// <summary>
        /// Content for each quest node state and UI category (dialogue, journal, HUD).
        /// </summary>
        public ActionText actionText
        {
            get { return m_actionText; }
            set { m_actionText = value; }
        }

        public ActionRequirement[] requirements
        {
            get { return m_requirements; }
            set { m_requirements = value; }
        }

        public ActionEffect[] effects
        {
            get { return m_effects; }
            set { m_effects = value; }
        }

        public ActionCompletion completion
        {
            get { return m_completion; }
            set { m_completion = value; }
        }
    }

}