// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    public enum UrgentFactionSelectionCriterion
    {
        SameAsGlobalSetting,
        Weighted,
        WeightedSquared
    }

    /// <summary>
    /// Specifies the method to use to select the most urgent fact.
    /// The quest generator will generate a quest to address this fact.
    /// </summary>
    public struct UrgentFactSelectionMode
    {
        [SerializeField]
        private UrgentFactionSelectionCriterion m_criterion;

        [SerializeField]
        private int m_max;

        public UrgentFactionSelectionCriterion criterion
        {
            get { return m_criterion; }
            set { m_criterion = value; }
        }

        public int max
        {
            get { return m_max; }
            set { m_max = value; }
        }

        public UrgentFactSelectionMode(UrgentFactionSelectionCriterion criterion, int max)
        {
            m_criterion = criterion;
            m_max = max;
        }

        public static UrgentFactSelectionMode mostUrgent = new UrgentFactSelectionMode(UrgentFactionSelectionCriterion.Weighted, 1);
    }

}