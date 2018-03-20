// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// This reward system sends a message with number of user-definable "things".
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class MessageRewardSystem : RewardSystem
    {

        [Tooltip("Consume reward points when determining reward.")]
        public bool consumePoints = true;

        [Tooltip("How the number passed with the message scales according to the points.")]
        public AnimationCurve pointsCurve = new AnimationCurve(new Keyframe(1, 1), new Keyframe(100, 100));

        public StringField thing = new StringField("Coins");
        public StringField target = new StringField(QuestMachineTags.QUESTERID);
        public StringField message = new StringField("Get");
        public StringField parameter = new StringField("Coin");

        public override int DetermineReward(int points, Quest quest)
        {
            var val = (int) pointsCurve.Evaluate(points);

            if (!StringField.IsNullOrEmpty(thing))
            {
                var bodyText = BodyTextQuestContent.CreateInstance<BodyTextQuestContent>();
                bodyText.bodyText = new StringField(val + " " + thing);
                quest.offerContentList.Add(bodyText);
            }

            var xpAction = MessageQuestAction.CreateInstance<MessageQuestAction>();
            xpAction.senderID = new StringField(QuestMachineTags.QUESTGIVERID);
            xpAction.targetID = target;
            xpAction.message = message;
            xpAction.parameter = parameter;
            xpAction.value.valueType = MessageValueType.Int;
            xpAction.value.intValue = val;
            var successInfo = quest.GetStateInfo(QuestState.Successful);
            successInfo.actionList.Add(xpAction);

            return consumePoints ? (points - val) : points;
        }

    }
}