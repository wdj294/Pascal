// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    public class PlanStep
    {
        public Fact fact;
        public Action action;
        public int requiredCounterValue;

        public PlanStep(Fact fact, Action action, int requiredCounterValue = 1)
        {
            this.fact = fact;
            this.action = action;
            this.requiredCounterValue = requiredCounterValue;
            this.requiredCounterValue = fact.count; //[TODO]
        }

        public override string ToString()
        {
            return ((action != null) ? action.name : "(no-action)") + ":" + ((fact != null && fact.entityType != null) ? fact.entityType.name : "(no-entity)");
        }
    }

}
