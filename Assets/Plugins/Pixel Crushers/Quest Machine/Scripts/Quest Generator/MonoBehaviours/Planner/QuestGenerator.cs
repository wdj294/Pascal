// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    public delegate void GeneratedQuestDelegate(Quest quest);

    /// <summary>
    /// Class to procedurally generate quests.
    /// </summary>
    public class QuestGenerator
    {

        #region Fields & Properties

        public static int maxSimultaneousPlanners = 5;
        public static int maxGoalActionChecksPerFrame = 100;
        public static int maxStepsPerFrame = 100;
        public static bool detailedDebug = false;

        private QuestEntity entity { get; set; }
        private StringField group { get; set; }
        private DomainType domainType { get; set; }
        private WorldModel worldModel { get; set; }
        private bool requireReturnToComplete { get; set; }
        private List<QuestContent> rewardsUIContents { get; set; }
        private List<RewardSystem> rewardSystems { get; set; }
        private string[] ignoreList { get; set; }
        private Coroutine coroutine { get; set; }
        private bool cancel { get; set; }

        private PlanStep goal { get; set; }
        private List<PlanStep> masterStepList = new List<PlanStep>();
        private Plan plan { get; set; }

        #endregion

        #region Entry Points

        public void GenerateQuest(QuestEntity entity, StringField group, DomainType domainType, WorldModel worldModel, bool requireReturnToComplete,
            List<QuestContent> rewardsUIContents, List<RewardSystem> rewardSystems, List<Quest> existingQuests, GeneratedQuestDelegate generatedQuest)
        {
            if (entity == null || domainType == null || worldModel == null) return;
            coroutine = entity.StartCoroutine(GenerateQuestCoroutine(entity, group, domainType, worldModel, requireReturnToComplete, 
                rewardsUIContents, rewardSystems, existingQuests, generatedQuest));
        }

        public void CancelGeneration()
        {
            cancel = true;
        }

        private IEnumerator GenerateQuestCoroutine(QuestEntity entity, StringField group, DomainType domainType, WorldModel worldModel, bool requireReturnToComplete,
            List<QuestContent> rewardsUIContents, List<RewardSystem> rewardSystems, List<Quest> existingQuests, GeneratedQuestDelegate generatedQuest)
        {
            this.cancel = false;
            this.entity = entity;
            this.group = group;
            this.domainType = domainType;
            this.worldModel = worldModel;
            this.requireReturnToComplete = requireReturnToComplete;
            this.rewardsUIContents = rewardsUIContents;
            this.rewardSystems = rewardSystems;
            this.ignoreList = GenerateIgnoreList(existingQuests);
            masterStepList = new List<PlanStep>();
            goal = null;
            plan = null;
            Quest quest = null;
            worldModel.observer = new Fact(domainType, entity.entityType, 1);
            yield return DetermineGoal();
            if (!(cancel || goal == null))
            {
                yield return GeneratePlan();
                if (!(cancel || plan == null))
                {
                    BackfillMinimumCounterValues();
                    if (detailedDebug) LogPlan(plan);
                    quest = ConvertPlanToQuest();
                }
            }
            generatedQuest(quest);
        }

        private string[] GenerateIgnoreList(List<Quest> existingQuests)
        {
            if (existingQuests == null || existingQuests.Count == 0) return null;
            var list = new List<string>();
            for (int i = 0; i < existingQuests.Count; i++)
            {
                if (existingQuests[i] == null) continue;
                var entityTypeName = existingQuests[i].goalEntityTypeName;
                if (string.IsNullOrEmpty(entityTypeName) || list.Contains(entityTypeName)) continue;
                list.Add(entityTypeName);
            }
            return list.ToArray();
        }

        #endregion

        #region Determine Goal

        private IEnumerator DetermineGoal()
        {
            // Search world model for most urgent fact:
            Fact[] mostUrgentFacts;
            worldModel.ComputeUrgency(UrgentFactSelectionMode.mostUrgent, out mostUrgentFacts, ignoreList);
            var mostUrgentFact = (mostUrgentFacts.Length > 0) ? mostUrgentFacts[0] : null;
            if (mostUrgentFact == null) yield break;
            if (QuestMachine.debug || detailedDebug) Debug.Log("Quest Machine: [Generator] " + entity.displayName + ": Most urgent fact: " + mostUrgentFact.count + " " + 
                mostUrgentFact.entityType.name + " in " + mostUrgentFact.domainType.name, entity);

            // Choose goal action to perform on that fact:
            float bestUrgency = Mathf.Infinity;
            Action bestAction = null;
            var actions = GetEntityActions(mostUrgentFact.entityType);
            if (actions == null) yield break;
            int numChecks = 0;
            for (int i = 0; i < actions.Count; i++)
            {
                numChecks++;
                if (numChecks > maxGoalActionChecksPerFrame)
                {
                    numChecks = 0;
                    yield return null;
                }
                var action = actions[i];
                if (action == null) continue;
                var wm = new WorldModel(worldModel);
                wm.ApplyAction(mostUrgentFact, action);
                Fact newMostUrgentFact;
                var newUrgency = wm.ComputeUrgency(out newMostUrgentFact);
                var bestMotive = ChooseBestMotive(action.motives);
                var weightedUrgency = (bestMotive != null) ? (newUrgency - (GetDriveAlignment(bestMotive.driveValues) * newUrgency)) : newUrgency;
                if (weightedUrgency < bestUrgency) // Select goal action based on resulting urgency weighted by how well the motive aligns with the giver's drives.
                {
                    bestUrgency = weightedUrgency;
                    bestAction = action;
                }
            }
            if (bestAction == null) yield break;
            goal = new PlanStep(mostUrgentFact, bestAction, mostUrgentFact.count);
            if (QuestMachine.debug || detailedDebug) Debug.Log("Quest Machine: [Generator] " + entity.displayName + ": Goal: " + bestAction.name + " " + 
                mostUrgentFact.count + " " + mostUrgentFact.entityType.name, entity);
        }

        #endregion

        #region Motives

        private Motive ChooseBestMotive(Motive[] motives)
        {
            if (motives == null) return null;
            Motive bestMotive = null;
            float bestAlignment = -Mathf.Infinity;
            for (int i = 0; i < motives.Length; i++)
            {
                var motive = motives[i];
                if (motive == null) continue;
                var alignment = GetDriveAlignment(motive.driveValues);

                if (detailedDebug) Debug.Log("Quest Machine: [Generator] Motive Alignment: entity=" + ((entity != null) ? entity.name : "null") + 
                    " motive=" + motive.text + " alignment=" + alignment, entity);

                if (alignment > bestAlignment)
                {
                    bestAlignment = alignment;
                    bestMotive = motive;
                }
            }
            return bestMotive;
        }

        private float GetDriveAlignment(DriveValue[] driveValues)
        {
            if (driveValues == null) return 0;
            float totalAlignment = 0;
            int count = 0;
            for (int i = 0; i < driveValues.Length; i++)
            {
                var driveValue = driveValues[i];
                if (driveValue == null || driveValue.drive == null) continue;
                var entityDriveValue = LookupEntityDriveValue(driveValue.drive);
                if (entityDriveValue == null) continue;
                float difference = Mathf.Abs(driveValue.value - entityDriveValue.value);
                var alignment = (200f - difference) / 200f;
                totalAlignment += alignment;
                count++;
            }
            return (count == 0) ? 0 : (totalAlignment / (float)count);
        }

        private DriveValue LookupEntityDriveValue(Drive drive)
        {
            if (drive == null || entity == null || entity.entityType == null || entity.entityType.driveValues == null) return null;
            for (int i = 0; i < entity.entityType.driveValues.Count; i++)
            {
                var driveValue = entity.entityType.driveValues[i];
                if (driveValue == null) continue;
                if (driveValue.drive == drive) return driveValue;
            }
            return null;
        }

        #endregion

        #region Generate Plan

        private IEnumerator GeneratePlan()
        {
            yield return BFS(worldModel, goal);
        }

        // Currently use a BFS rather than A* because it produces better results since there's no good heuristic yet.
        private IEnumerator BFS(WorldModel initialWorldModel, PlanStep goal)
        {
            yield return null;

            var Q = new Queue<Plan>();

            // Queue the initial state:
            Q.Enqueue(new Plan(null, null, initialWorldModel));

            int numStepsChecked = 0;
            int safeguard = 0;
            while (Q.Count > 0 && safeguard < 1000)
            {
                safeguard++;

                numStepsChecked++;
                if (numStepsChecked > maxStepsPerFrame)
                {
                    numStepsChecked = 0;
                    yield return null;
                }

                var current = Q.Dequeue();

                //--- For debugging:
                //var indent = string.Empty;
                //for (int i = 0; i < current.steps.Count; i++) { indent += "        "; }
                //var lastStep = (current.steps.Count > 0) ? current.steps[current.steps.Count - 1] : null;
                //Debug.Log(indent + "Goal met (lastStep=" + lastStep + ")? " + current.worldModel.AreRequirementsMet(goal.action.requirements));

                // If the current state meets the goal requirements, return a finished plan:
                if (current.worldModel.AreRequirementsMet(goal.action.requirements))
                {
                    plan = new Plan(current, goal, null);
                    yield break;
                }

                // Otherwise queue up the actions that are valid in the current state:
                var lastStep = (current.steps.Count > 0) ? current.steps[current.steps.Count - 1] : null;
                foreach (var fact in current.worldModel.facts)
                {
                    var actions = GetEntityActions(fact.entityType);
                    if (actions == null) continue;
                    foreach (var action in actions)
                    {
                        if (fact == null || fact.entityType == null || action == null) continue;
                        if (lastStep != null && fact == lastStep.fact && action == lastStep.action) continue; // Don't repeat last action.
                        if (!current.worldModel.AreRequirementsMet(action.requirements)) continue; // If not valid, don't queue it.

                        //---Debug.Log(indent + action.name + " " + fact.entityType.name);

                        var newWorldModel = new WorldModel(current.worldModel);
                        newWorldModel.ApplyAction(fact, action);
                        var newPlan = new Plan(current, GetStep(fact, action), newWorldModel);
                        Q.Enqueue(newPlan);
                    }
                }
            }
            if (QuestMachine.debug || detailedDebug) Debug.Log("Quest Machine: [Generator] Could not create quest. Exceeded safeguard while generating plan to " + 
                goal.action.name + " " + goal.fact.entityType.name + ".", entity);
        }

        private List<Action> GetEntityActions(EntityType entityType)
        {
            // Nonrecursively gather a list of all actions on the entity and its parents:
            if (entityType == null) return null;
            if (entityType.parents == null || entityType.parents.Count == 0) return entityType.actions;
            var actions = new List<Action>();
            var processed = new List<EntityType>();
            var Q = new Queue<EntityType>();
            Q.Enqueue(entityType);
            int safeguard = 0;
            while (Q.Count > 0 && safeguard < 1000)
            {
                safeguard++;
                var et = Q.Dequeue();
                if (et == null) continue;
                processed.Add(et);
                if (et.parents != null)
                {
                    // Add parents to queue to check for actions:
                    for (int i = 0; i < et.parents.Count; i++)
                    {
                        var parent = et.parents[i];
                        if (parent != null && !processed.Contains(parent)) Q.Enqueue(parent);
                    }
                }
                if (et.actions != null)
                {
                    // Add actions to list:
                    for (int i = 0; i < et.actions.Count; i++)
                    {
                        var action = et.actions[i];
                        if (action != null && !actions.Contains(action)) actions.Add(action);
                    }
                }
            }
            return actions;
        }

        private PlanStep GetStep(Fact fact, Action action)
        {
            foreach (var step in masterStepList)
            {
                if (step.fact == fact && step.action == action) return step;
            }
            var newStep = new PlanStep(fact, action);
            masterStepList.Add(newStep);
            return newStep;
        }

        /// <summary>
        /// For each counter type (e.g., numApplesPicked, numOrcsKilled), find the 
        /// highest required value. Then set all required values for that counter
        /// type to that value.
        /// </summary>
        private void BackfillMinimumCounterValues()
        {
            var requiredCounterValue = new Dictionary<string, int>();
            for (int i = 0; i < plan.steps.Count; i++)
            {
                var step = plan.steps[i];
                if (step.action.completion.mode != ActionCompletion.Mode.Counter) continue;
                var stepCounterName = StringField.GetStringValue(step.action.completion.baseCounterName);
                var stepRequiredValue = step.action.completion.requiredValue;
                if (!requiredCounterValue.ContainsKey(stepCounterName))
                {
                    requiredCounterValue.Add(stepCounterName, stepRequiredValue);
                }
                else
                {
                    requiredCounterValue[stepCounterName] = Mathf.Max(requiredCounterValue[stepCounterName], stepRequiredValue);
                }
            }
            for (int i = 0; i < plan.steps.Count; i++)
            {
                var step = plan.steps[i];
                if (step.action.completion.mode != ActionCompletion.Mode.Counter) continue;
                var stepCounterName = StringField.GetStringValue(step.action.completion.baseCounterName);
                step.action.completion.requiredValue = requiredCounterValue[stepCounterName];
            }
        }

        private void LogPlan(Plan plan)
        {
            var s = "Quest Machine: [Generator] Plan (" + plan.steps.Count + " steps):\n";
            foreach (var planStep in plan.steps)
            {
                if (planStep.fact != null)
                {
                    s += "   " + planStep.ToString() + "\n";
                }
                else
                {
                    s += "   (null)\n";
                }
            }
            Debug.Log(s);
        }

        #endregion

        #region Convert Plan To Quest

        private Quest ConvertPlanToQuest()
        {
            // Build title:
            var mainTargetEntity = goal.fact.entityType.name;
            var mainTargetDescriptor = goal.fact.entityType.GetDescriptor(goal.fact.count);
            var title = goal.action.displayName + " " + mainTargetDescriptor;
            var questID = title + " " + System.Guid.NewGuid();
            var domainName = StringField.GetStringValue(goal.fact.domainType.displayName);

            // Start QuestBuilder:
            var questBuilder = new QuestBuilder(title, questID, title);
            questBuilder.quest.isTrackable = true;
            questBuilder.quest.showInTrackHUD = true;
            questBuilder.quest.icon = goal.fact.entityType.image;
            questBuilder.quest.group = new StringField(group);
            questBuilder.quest.goalEntityTypeName = goal.fact.entityType.name;

            // Offer motive:
            var motiveText = (goal.action.motives.Length > 0) ? StringField.GetStringValue(goal.action.motives[0].text) : StringField.GetStringValue(goal.action.actionText.activeText.dialogueText);
            motiveText = ReplaceStepTags(motiveText, mainTargetEntity, mainTargetDescriptor, domainName, string.Empty, 0);
            questBuilder.AddOfferContents(questBuilder.CreateTitleContent(), questBuilder.CreateBodyContent(motiveText));

            // Offer rewards:
            questBuilder.AddOfferContents(QuestContent.CloneList<QuestContent>(rewardsUIContents).ToArray());
            if (detailedDebug) Debug.Log("Quest Machine: [Generator] Checking " + rewardSystems.Count + " reward systems for " + goal.fact.count + " " + goal.fact.entityType.name +
                " (level " + goal.fact.entityType.level + ") on " + entity.name, entity);
            var pointsRemaining = goal.fact.entityType.level * goal.fact.count;
            foreach (var rewardSystem in rewardSystems)
            {
                if (rewardSystem == null) continue;
                pointsRemaining = rewardSystem.DetermineReward(pointsRemaining, questBuilder.quest);
                if (pointsRemaining <= 0) break;
            }

            // Quest heading:
            var hasSuccessfulDialogueText = !StringField.IsNullOrEmpty(goal.action.actionText.completedText.dialogueText);
            var hasSuccessfulJournalText = !StringField.IsNullOrEmpty(goal.action.actionText.completedText.journalText);
            AddQuestHeading(questBuilder, QuestContentCategory.Dialogue, hasSuccessfulDialogueText);
            AddQuestHeading(questBuilder, QuestContentCategory.Journal, hasSuccessfulJournalText);
            AddQuestHeading(questBuilder, QuestContentCategory.HUD, false);

            // Successful text (shown in journal / when talking again about successful quest):
            var successful = questBuilder.quest.GetStateInfo(QuestState.Successful);
            if (hasSuccessfulDialogueText)
            {
                var dlgText = questBuilder.CreateBodyContent(ReplaceStepTags(goal.action.actionText.completedText.dialogueText.value, mainTargetEntity, mainTargetDescriptor, domainName, string.Empty, 0));
                questBuilder.AddContents(successful.GetContentList(QuestContentCategory.Dialogue), dlgText);
            }
            if (hasSuccessfulJournalText)
            {
                var jrlText = questBuilder.CreateBodyContent(ReplaceStepTags(goal.action.actionText.completedText.journalText.value, mainTargetEntity, mainTargetDescriptor, domainName, string.Empty, 0));
                questBuilder.AddContents(successful.GetContentList(QuestContentCategory.Journal), jrlText);
            }

            // Add steps:
            var previousNode = questBuilder.GetStartNode();
            var counterNames = new HashSet<string>();
            for (int i = 0; i < plan.steps.Count; i++)
            {
                var step = plan.steps[i];

                // Create next condition node:
                var targetEntity = step.fact.entityType.name;
                var targetDescriptor = step.fact.entityType.GetDescriptor(step.fact.count);
                var id = (i + 1).ToString();
                var internalName = step.action.displayName + " " + targetDescriptor;
                var conditionNode = questBuilder.AddConditionNode(previousNode, id, internalName, ConditionCountMode.All);
                previousNode = conditionNode;

                // Variables for node text tag replacement:
                var counterName = string.Empty;
                int requiredCounterValue = 0;

                var completion = step.action.completion;
                if (completion.mode == ActionCompletion.Mode.Counter)
                {
                    // Setup counter condition:
                    if (completion.mode != ActionCompletion.Mode.Counter) continue;
                    counterName = goal.fact.entityType.pluralDisplayName.value + completion.baseCounterName.value;
                    if (!counterNames.Contains(counterName))
                    {
                        var counter = questBuilder.AddCounter(counterName, completion.initialValue, completion.minValue, completion.maxValue, false, completion.updateMode);
                        foreach (var messageEvent in completion.messageEventList)
                        {
                            var counterMessageEvent = new QuestCounterMessageEvent(messageEvent.targetID, messageEvent.message,
                                new StringField(StringField.GetStringValue(messageEvent.parameter).Replace("{TARGETENTITY}", targetEntity)),
                                messageEvent.operation, messageEvent.literalValue);
                            counter.messageEventList.Add(counterMessageEvent);
                        }
                    }
                    counterName = goal.fact.entityType.pluralDisplayName.value + completion.baseCounterName.value;
                    requiredCounterValue = Mathf.Min(step.requiredCounterValue, step.fact.count);
                    questBuilder.AddCounterCondition(conditionNode, counterName, CounterValueConditionMode.AtLeast, requiredCounterValue);
                    // Consider: Add action to reset counter to zero in case future nodes repeat the same counter?
                }
                else
                {
                    // Setup message condition:
                    questBuilder.AddMessageCondition(conditionNode, QuestMessageParticipant.Any, completion.senderID, QuestMessageParticipant.Any, completion.targetID,
                        completion.message, new StringField(StringField.GetStringValue(completion.parameter).Replace("{TARGETENTITY}", targetEntity)));
                }

                // Text for condition node's Active state:
                var activeState = conditionNode.stateInfoList[(int)QuestNodeState.Active];
                var taskText = ReplaceStepTags(step.action.actionText.activeText.dialogueText.value, targetEntity, targetDescriptor, domainName, counterName, requiredCounterValue);
                var bodyText = questBuilder.CreateBodyContent(taskText);
                var dialogueList = activeState.categorizedContentList[(int)QuestContentCategory.Dialogue];
                dialogueList.contentList.Add(bodyText);

                var jrlText = ReplaceStepTags(step.action.actionText.activeText.journalText.value, targetEntity, targetDescriptor, domainName, counterName, requiredCounterValue);
                var jrlbodyText = questBuilder.CreateBodyContent(jrlText);
                var journalList = activeState.categorizedContentList[(int)QuestContentCategory.Journal];
                journalList.contentList.Add(jrlbodyText);

                var hudText = ReplaceStepTags(step.action.actionText.activeText.hudText.value, targetEntity, targetDescriptor, domainName, counterName, requiredCounterValue);
                var hudbodyText = questBuilder.CreateBodyContent(hudText);
                var hudList = activeState.categorizedContentList[(int)QuestContentCategory.HUD];
                hudList.contentList.Add(hudbodyText);

                // Action when active:
                if (!StringField.IsNullOrEmpty(step.action.actionText.activeText.alertText))
                {
                    var alertAction = questBuilder.CreateAlertAction(ReplaceStepTags(step.action.actionText.activeText.alertText.value, targetEntity, targetDescriptor, domainName, counterName, requiredCounterValue));
                    activeState.actionList.Add(alertAction);
                }
            }

            // Add "return to giver" node:
            if (requireReturnToComplete)
            {
                var questGiver = entity.GetComponent<QuestGiver>();
                var giverID = (questGiver != null) ? questGiver.id : ((entity != null) ? entity.displayName : null);
                var returnNode = questBuilder.AddDiscussQuestNode(previousNode, QuestMessageParticipant.QuestGiver, giverID);
                returnNode.id = new StringField("Return");
                previousNode = returnNode;

                QuestStateInfo.ValidateStateInfoListCount(returnNode.stateInfoList);

                // Text when active:
                var stateInfo = returnNode.stateInfoList[(int)QuestNodeState.Active];
                QuestStateInfo.ValidateCategorizedContentListCount(stateInfo.categorizedContentList);
                var successText = ReplaceStepTags(StringField.GetStringValue(goal.action.actionText.successText), mainTargetEntity, mainTargetDescriptor, domainName, string.Empty, 0);
                var bodyText = questBuilder.CreateBodyContent(successText);
                var dialogueList = returnNode.stateInfoList[(int)QuestNodeState.Active].categorizedContentList[(int)QuestContentCategory.Dialogue];
                dialogueList.contentList.Add(bodyText);

                var jrlText = "{Return to} " + questGiver.displayName;
                var jrlBodyText = questBuilder.CreateBodyContent(jrlText);
                var journalList = returnNode.stateInfoList[(int)QuestNodeState.Active].categorizedContentList[(int)QuestContentCategory.Journal];
                journalList.contentList.Add(jrlBodyText);

                var hudText = "{Return to} " + questGiver.displayName;
                var hudBodyText = questBuilder.CreateBodyContent(hudText);
                var hudList = returnNode.stateInfoList[(int)QuestNodeState.Active].categorizedContentList[(int)QuestContentCategory.HUD];
                hudList.contentList.Add(hudBodyText);

                // Alert when active:
                var actionList = returnNode.GetStateInfo(QuestNodeState.Active).actionList;
                var alertAction = questBuilder.CreateAlertAction(hudText);
                actionList.Add(alertAction);

                // Indicator when active:
                var indicatorAction = questBuilder.CreateSetIndicatorAction(questBuilder.quest.id, entity.id, QuestIndicatorState.Talk);
                actionList.Add(indicatorAction);

                // Indicator when true:
                indicatorAction = questBuilder.CreateSetIndicatorAction(questBuilder.quest.id, entity.id, QuestIndicatorState.None);
                actionList = returnNode.GetStateInfo(QuestNodeState.True).actionList;
                actionList.Add(indicatorAction);
            }

            // Success node:
            questBuilder.AddSuccessNode(previousNode);

            return questBuilder.ToQuest();
        }

        private void AddQuestHeading(QuestBuilder questBuilder, QuestContentCategory category, bool addToSuccessfulList)
        {
            // Add to Active state and, if not HUD, to Successful state.
            questBuilder.AddContents(questBuilder.quest.stateInfoList[(int)QuestState.Active].categorizedContentList[(int)category], questBuilder.CreateTitleContent());
            if (addToSuccessfulList && category != QuestContentCategory.HUD)
            {
                questBuilder.AddContents(questBuilder.quest.stateInfoList[(int)QuestState.Successful].categorizedContentList[(int)category], questBuilder.CreateTitleContent());
            }
        }

        // Replace special tags that are specific to generated quests:
        private string ReplaceStepTags(string s, string targetEntity, string targetDescriptor, string domainName, string counterName, int counterValue)
        {
            return s.Replace("{#COUNTERNAME}", "{#" + counterName + "}").
                    Replace("{#COUNTERGOAL}", counterValue.ToString()).
                    Replace("{TARGETENTITY}", targetEntity).
                    Replace("{TARGETDESCRIPTOR}", targetDescriptor).
                    Replace("{DOMAIN}", domainName);
        }

        #endregion

    }

}