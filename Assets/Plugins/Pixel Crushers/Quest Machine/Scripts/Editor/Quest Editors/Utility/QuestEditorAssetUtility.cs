// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Utility methods used by the custom editors to manage assets, namely to
    /// save a quest ScriptableObject and all of its sub-ScriptableObjects to
    /// an asset file.
    /// </summary>
    public static class QuestEditorAssetUtility
    {

        private static string s_lastDirectory = "Assets";

        public static Quest SaveQuestAsAsset(Quest quest, string filePath, bool select = false)
        {
            if (filePath.StartsWith(Application.dataPath))
            {
                filePath = "Assets" + filePath.Substring(Application.dataPath.Length);
            }
            var questAsset = quest.Clone();
            questAsset.isInstance = false;
            AssetDatabase.CreateAsset(questAsset, filePath);
            SaveQuestSubassets(questAsset);
            AssetDatabase.SaveAssets();
            if (select)
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = questAsset;
            }
            return questAsset;
        }

        private static void SaveQuestSubassets(Quest questAsset)
        {
            SaveConditionSetSubassets(questAsset, questAsset.autostartConditionSet);
            SaveConditionSetSubassets(questAsset, questAsset.offerConditionSet);
            SaveUIContentListSubassets(questAsset, questAsset.offerConditionsUnmetContentList);
            SaveUIContentListSubassets(questAsset, questAsset.offerContentList);
            SaveStateInfoListSubassets(questAsset, questAsset.stateInfoList);
            SaveNodeListSubassets(questAsset, questAsset.nodeList);
        }

        private static void SaveQuestSubassetsList<T>(Quest questAsset, List<T> subassets) where T : QuestSubasset
        {
            if (subassets == null) return;
            for (int i = 0; i < subassets.Count; i++)
            {
                AssetUtility.AddToAsset(subassets[i], questAsset);
            }
        }

        private static void SaveConditionSetSubassets(Quest questAsset, QuestConditionSet conditionSet)
        {
            if (conditionSet == null || conditionSet.conditionList == null) return;
            SaveQuestSubassetsList(questAsset, conditionSet.conditionList);
        }

        private static void SaveUIContentListSubassets(Quest questAsset, List<QuestContent> uiContentList)
        {
            SaveQuestSubassetsList(questAsset, uiContentList);
        }

        private static void SaveActionListSubassets(Quest questAsset, List<QuestAction> actions)
        {
            SaveQuestSubassetsList(questAsset, actions);
        }

        private static void SaveStateInfoListSubassets(Quest questAsset, List<QuestStateInfo> stateInfoList)
        {
            if (stateInfoList == null) return;
            for (int i = 0; i < stateInfoList.Count; i++)
            {
                SaveActionListSubassets(questAsset, stateInfoList[i].actionList);
                if (stateInfoList[i].categorizedContentList == null) continue;
                for (int j = 0; j < stateInfoList[i].categorizedContentList.Count; j++)
                {
                    SaveUIContentListSubassets(questAsset, stateInfoList[i].categorizedContentList[j].contentList);
                }
            }
        }

        private static void SaveNodeListSubassets(Quest questAsset, List<QuestNode> nodes)
        {
            if (nodes == null) return;
            for (int i = 0; i < nodes.Count; i++)
            {
                SaveConditionSetSubassets(questAsset, nodes[i].conditionSet);
                SaveStateInfoListSubassets(questAsset, nodes[i].stateInfoList);
            }
        }

        public static Quest CreateNewQuestAssetFromDialog()
        {
            var filePath = EditorUtility.SaveFilePanel("Save Quest As", s_lastDirectory, string.Empty, "asset");
            if (string.IsNullOrEmpty(filePath)) return null;
            s_lastDirectory = System.IO.Path.GetDirectoryName(filePath);
            var questWrapperType = QuestEditorUtility.GetWrapperType(typeof(Quest));
            if (questWrapperType == null)
            {
                Debug.LogError("Quest Machine: Internal error. Can't access Quest type!");
                return null;
            }
            var quest = ScriptableObjectUtility.CreateScriptableObject(questWrapperType) as Quest;
            if (quest == null)
            {
                Debug.LogError("Quest Machine: Internal error. Can't create Quest object!");
                return null;
            }
            var filename = System.IO.Path.GetFileNameWithoutExtension(filePath);
            quest.id.value = filename.Replace(" ", string.Empty);
            quest.title.value = filename;
            return SaveQuestAsAsset(quest, filePath, false);
        }



    }

}