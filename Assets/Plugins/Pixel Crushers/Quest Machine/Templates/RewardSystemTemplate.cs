/* [REMOVE THIS LINE] 

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    // This is a starter template for reward systems. To use it,
    /// make a copy, rename it, and remove the line marked above.
    /// Then fill in your code where indicated below.
    public class RewardSystemTemplate : RewardSystem // Rename this class.
    {

        // The quest generator will call this method to try to use up points
        // by choosing rewards to offer.
        public override int DetermineReward(int points, Quest quest)
        {
            // Example code:

            // Add some UI content to the quest's offerContentList:
            var bodyText = BodyTextQuestContent.CreateInstance<BodyTextQuestContent>();
            bodyText.bodyText = new StringField("Some Reward");
            quest.offerContentList.Add(bodyText);

            // Add an action to the quest's Successful state to send a message to the Message System:
            var action = MessageQuestAction.CreateInstance<MessageQuestAction>();
            action.senderID = new StringField(QuestMachineTags.QUESTGIVERID); // Send from quest giver.
            action.targetID = new StringField(QuestMachineTags.QUESTERID); // Send to quester (player).
            action.message = new StringField("Some Reward Message");
            action.parameter = new StringField();
            action.value = new MessageValue();
            var successInfo = quest.GetStateInfo(QuestState.Successful);
            successInfo.actionList.Add(action);

            return points - 10; // Use up 10 reward points with this reward.
        }

    }
}

/**/