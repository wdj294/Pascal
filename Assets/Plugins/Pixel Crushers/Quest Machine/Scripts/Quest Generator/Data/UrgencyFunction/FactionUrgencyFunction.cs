// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Returns the faction affinity.
    /// </summary>
    //--- Only used internally: [CreateAssetMenu(menuName = "Quest Machine/Entities/Urgency Functions/Urgency By Faction")]
    public class FactionUrgencyFunction : UrgencyFunction
    {

        public override string typeName { get { return "By Faction"; } }

        public override float Compute(WorldModel worldModel)
        {
            var observerFaction = worldModel.observer.entityType.GetFaction();
            var otherFaction = worldModel.observed.entityType.GetFaction();
            return worldModel.observed.count * -observerFaction.GetAffinity(otherFaction);
        }

    }
}