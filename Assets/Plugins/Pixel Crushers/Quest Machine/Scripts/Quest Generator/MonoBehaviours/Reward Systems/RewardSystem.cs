// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    public abstract class RewardSystem : MonoBehaviour
    {

        public abstract int DetermineReward(int points, Quest quest);

    }
}
