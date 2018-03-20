// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// This class draws the GUI canvas for a Quest in the Quest Editor window.
    /// </summary>
    public class QuestCanvasGUI
    {

        #region Private Fields

        private const float CanvasWidth = 10000;
        private const float CanvasHeight = 10000;

        private float m_panX = 0;
        private float m_panY = 0;
        private float m_zoom = 1;
        private Vector2 m_mousePos;
        private Vector2 m_prevMousePos;
        private bool m_connecting = false;
        private bool m_dragging = false;

        private Quest m_quest;
        private SerializedObject m_questSerializedObject;
        private SerializedProperty m_nodeListProperty;

        #endregion

        #region Setup

        public void AssignQuest(Quest quest)
        {
            m_quest = quest;
            m_questSerializedObject = (quest != null) ? new SerializedObject(quest) : null;
            m_nodeListProperty = (m_questSerializedObject != null) ? m_questSerializedObject.FindProperty("m_nodeList") : null;
        }

        public bool IsQuestAssigned()
        {
            return m_questSerializedObject != null && m_questSerializedObject.targetObject != null;
        }

        #endregion

        #region High Level Drawing

        public virtual void Draw(Rect position)
        {
            if (!AreReferencesValid()) return;
            m_questSerializedObject.Update();
            DrawQuestTitle();
            DrawQuestCanvas();
            m_questSerializedObject.ApplyModifiedProperties();
            DrawGearMenu(position);
        }

        protected bool AreReferencesValid()
        {
            UnityEngine.Assertions.Assert.IsNotNull(m_questSerializedObject, "Quest Machine: Internal error - m_questSerializedObject is null.");
            if (m_questSerializedObject == null) return false;
            UnityEngine.Assertions.Assert.IsNotNull(m_questSerializedObject.targetObject, "Quest Machine: Internal error - m_questSerializedObject target object is null.");
            if (m_questSerializedObject.targetObject == null) return false;
            UnityEngine.Assertions.Assert.IsNotNull(m_nodeListProperty, "Quest Machine: Internal error - m_nodeList property is null.");
            if (m_nodeListProperty == null) return false;
            UnityEngine.Assertions.Assert.IsNotNull(QuestEditorWindow.instance, "Quest Machine: Internal error - QuestEditorWindow.instance is null.");
            if (QuestEditorWindow.instance == null) return false;
            return true;
        }

        private void DrawQuestTitle()
        {
            var titleProperty = m_questSerializedObject.FindProperty("m_title");
            UnityEngine.Assertions.Assert.IsNotNull(titleProperty, "Quest Machine: Internal error - m_title property is null.");
            if (titleProperty == null) return;
            var displayName = StringFieldDrawer.GetStringFieldValue(titleProperty);
            if (m_quest.isInstance) displayName += " (runtime)";
            EditorGUILayout.LabelField(displayName, QuestEditorStyles.questNameGUIStyle);
        }

        private void DrawQuestCanvas()
        {
            var screenCoordsArea = new Rect(m_panX, m_panY, CanvasWidth, CanvasHeight);
            EditorGUIZoomArea.Begin(m_zoom, screenCoordsArea);
            try
            {
                HandleInput();
                DrawConnectionArrows();
                DrawNodes();
            }
            finally
            {
                EditorGUIZoomArea.End();
            }
        }

        #endregion

        #region Draw Content

        private void DrawConnectionArrows()
        {
            ValidateStartNode();
            for (int i = 0; i < m_nodeListProperty.arraySize; i++)
            {
                var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(i);
                if (nodeProperty == null) continue;
                var nodeRect = GetCanvasRect(nodeProperty);
                var parentRect = new Rect(nodeRect.xMin, nodeRect.yMax - EditorGUIUtility.singleLineHeight, nodeRect.width, EditorGUIUtility.singleLineHeight);
                var childIndexListProperty = nodeProperty.FindPropertyRelative("m_childIndexList");
                DrawConnectionsToChildren(parentRect, childIndexListProperty);
            }
        }

        private void DrawConnectionsToChildren(Rect sourceRect, SerializedProperty childIndexListProperty)
        {
            UnityEngine.Assertions.Assert.IsNotNull(childIndexListProperty, "Quest Machine: Internal error - m_childIndexList property is null.");
            if (childIndexListProperty == null) return;
            for (int i = 0; i < childIndexListProperty.arraySize; i++)
            {
                var childIndex = childIndexListProperty.GetArrayElementAtIndex(i).intValue;
                if (0 <= childIndex && childIndex < m_nodeListProperty.arraySize)
                {
                    var destNodeRect = GetCanvasRect(m_nodeListProperty.GetArrayElementAtIndex(childIndex));
                    var destRect = new Rect(destNodeRect.x, destNodeRect.y, destNodeRect.width, EditorGUIUtility.singleLineHeight);
                    DrawNodeCurve(sourceRect, destRect, QuestEditorStyles.ConnectorColor);
                }
            }
        }

        private void ValidateStartNode()
        {
            if (m_nodeListProperty.arraySize > 0) return;
            m_questSerializedObject.ApplyModifiedProperties();
            var quest = m_questSerializedObject.targetObject as Quest;
            if (quest != null) quest.Initialize();
            m_questSerializedObject.Update();
        }

        private void DrawNodes()
        {
            QuestEditorWindow.instance.BeginWindows();
            try
            {
                for (int i = 0; i < m_nodeListProperty.arraySize; i++)
                {
                    DrawNode(i, m_nodeListProperty.GetArrayElementAtIndex(i));
                }
            }
            finally
            {
                QuestEditorWindow.instance.EndWindows();
            }
        }

        private void DrawNode(int nodeIndex, SerializedProperty nodeProperty)
        {
            if (nodeProperty == null) return;
            var stateProperty = nodeProperty.FindPropertyRelative("m_state");
            UnityEngine.Assertions.Assert.IsNotNull(stateProperty, "Quest Machine: Internal error - quest node m_state property is null.");
            if (stateProperty == null) return;
            var nodeState = (QuestNodeState)stateProperty.enumValueIndex;
            var prevContentColor = GUI.contentColor;
            var prevBkColor = GUI.backgroundColor;
            switch (nodeState)
            {
                case QuestNodeState.Inactive:
                    // Use default color; don't set GUI.backgroundColor.
                    break;
                case QuestNodeState.Active:
                    GUI.backgroundColor = QuestEditorStyles.ActiveNodeColor;
                    break;
                case QuestNodeState.True:
                    GUI.backgroundColor = QuestEditorStyles.TrueNodeColor;
                    break;
            }
            var nodeRectProperty = GetCanvasRectProperty(nodeProperty);
            if (nodeRectProperty != null)
            {
                nodeRectProperty.rectValue = GUI.Window(nodeIndex, nodeRectProperty.rectValue, DrawNodeWindow, string.Empty, QuestEditorStyles.questNodeWindowGUIStyle);
            }
            GUI.contentColor = prevContentColor;
            GUI.backgroundColor = prevBkColor;
        }

        private void DrawNodeWindow(int id)
        {
            UnityEngine.Assertions.Assert.IsTrue(0 <= id && id < m_nodeListProperty.arraySize, "Quest Machine: Internal error - node ID is outside m_nodeList range.");
            if (!(0 <= id && id < m_nodeListProperty.arraySize)) return;
            try
            {
                var rect = EditorGUILayout.GetControlRect();
                var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(id);
                UnityEngine.Assertions.Assert.IsNotNull(nodeProperty, "Quest Machine: Internal error - m_nodeList[id] property is null.");
                if (nodeProperty == null) return;
                var canvasRect = GetCanvasRect(nodeProperty);
                var nodeType = GetNodeType(nodeProperty);
                var text = GetNodeText(nodeProperty);
                var barWidth = rect.width + (EditorGUIUtility.isProSkin ? 5 : 7); // A bit of pixel fine-tuning.
                var barRect = new Rect(rect.x - 3, rect.y - 3, barWidth, QuestEditorStyles.nodeBarHeight);
                var textRect = new Rect(barRect.x - 1, barRect.y - 1, barRect.width, barRect.height);
                GUI.Label(barRect, text, QuestEditorStyles.GetNodeBarGUIStyle(nodeType));
                GUI.Label(textRect, text, QuestEditorStyles.nodeTextGUIStyle);
                if (nodeType != QuestNodeType.Start)
                {
                    GUI.DrawTexture(new Rect(rect.x + (rect.width / 2) - (QuestEditorStyles.connectorImage.width / 2), rect.y - 16, QuestEditorStyles.connectorImage.width, QuestEditorStyles.connectorImage.height), QuestEditorStyles.connectorImage);
                }
                if (nodeType != QuestNodeType.Success && nodeType != QuestNodeType.Failure)
                {
                    GUI.DrawTexture(new Rect(rect.x + (rect.width / 2) - (QuestEditorStyles.connectorImage.width / 2), rect.y + canvasRect.height - 23 - QuestEditorStyles.connectorImage.height, QuestEditorStyles.connectorImage.width, QuestEditorStyles.connectorImage.height), QuestEditorStyles.connectorImage);
                }
            }
            finally
            {
                if (!m_connecting) GUI.DragWindow();
            }
        }

        private string GetNodeText(SerializedProperty nodeProperty)
        {
            if (nodeProperty == null) return string.Empty;
            var internalNameProperty = nodeProperty.FindPropertyRelative("m_internalName");
            UnityEngine.Assertions.Assert.IsNotNull(internalNameProperty, "Quest Machine: Internal error - m_internalName property is null.");
            if (internalNameProperty == null) return string.Empty;
            var text = StringFieldDrawer.GetStringFieldValue(internalNameProperty);
            if (string.IsNullOrEmpty(text))
            {
                var idProperty = nodeProperty.FindPropertyRelative("m_id");
                UnityEngine.Assertions.Assert.IsNotNull(idProperty, "Quest Machine: Internal error - m_id property is null.");
                if (idProperty == null) return string.Empty;
                text = StringFieldDrawer.GetStringFieldValue(idProperty);
            }
            return text;
        }

        private QuestNodeType GetNodeType(SerializedProperty nodeProperty)
        {
            if (nodeProperty == null) return QuestNodeType.Start;
            var nodeTypeProperty = nodeProperty.FindPropertyRelative("m_nodeType");
            UnityEngine.Assertions.Assert.IsNotNull(nodeTypeProperty, "Quest Machine: Internal error - m_nodeType property is null.");
            if (nodeTypeProperty == null) return QuestNodeType.Start;
            return (QuestNodeType)nodeTypeProperty.enumValueIndex;
        }

        private SerializedProperty GetCanvasRectProperty(SerializedProperty nodeProperty)
        {
            if (nodeProperty == null) return null;
            var canvasRectProperty = nodeProperty.FindPropertyRelative("m_canvasRect");
            UnityEngine.Assertions.Assert.IsNotNull(canvasRectProperty, "Quest Machine: Internal error - m_canvasRect property is null.");
            if (canvasRectProperty == null) return null;
            if (canvasRectProperty.rectValue.width < QuestEditorStyles.nodeWidth)
            {
                var nodeType = GetNodeType(nodeProperty);
                var isShortNode = (nodeType == QuestNodeType.Success || nodeType == QuestNodeType.Failure);
                var nodeHeight = isShortNode ? QuestEditorStyles.shortNodeHeight : QuestEditorStyles.nodeHeight;
                canvasRectProperty.rectValue = new Rect(canvasRectProperty.rectValue.x, canvasRectProperty.rectValue.y, QuestEditorStyles.nodeWidth, nodeHeight);
            }
            return canvasRectProperty;
        }

        private Rect GetCanvasRect(SerializedProperty parentProperty)
        {
            var canvasRectProperty = GetCanvasRectProperty(parentProperty);
            return (canvasRectProperty != null) ? canvasRectProperty.rectValue : new Rect(50, 30, QuestEditorStyles.nodeWidth, QuestEditorStyles.nodeHeight);
        }

        #endregion

        #region Handle Input

        private void HandleInput()
        {
            m_prevMousePos = m_mousePos;
            m_mousePos = Event.current.mousePosition;
            
            var inputClickNotConnecting = (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.alt && !m_connecting);
            var inputEndConnecting = (Event.current.type == EventType.MouseUp && Event.current.button == 0 && m_connecting);
            var inputCancelConnecting = (Event.current.type == EventType.MouseDown && Event.current.button == 1 && m_connecting);
            var inputBeginDragging = (Event.current.type == EventType.MouseDown && ((Event.current.button == 2) || (Event.current.button == 0 && Event.current.alt)));
            var inputEndDragging = (Event.current.type == EventType.MouseUp && ((Event.current.button == 2) || (Event.current.button == 0)) && m_dragging);
            var inputContextMenu = ((Event.current.type == EventType.MouseDown) && ((Event.current.button == 1) || (Event.current.button == 0 && Event.current.control)));
            var inputWheelZoom = (Event.current.type == EventType.ScrollWheel && !QuestEditorPrefs.zoomLock);
            var deleteKey = (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete && QuestEditorWindow.selectedNodeListIndex != -1);

            if (inputCancelConnecting)
            {
                m_connecting = false;
            }
            else if (inputContextMenu)
            {
                ShowContextMenu();
                Event.current.Use();
            }
            else if (inputEndConnecting)
            {
                EndConnection();
                Event.current.Use();
            }
            else if (inputClickNotConnecting)
            {
                ClickOnCanvas();
            }
            else if (inputBeginDragging)
            {
                m_dragging = true;
            }
            else if (inputEndDragging)
            {
                m_dragging = false;
            }
            else if (inputWheelZoom)
            {
                ZoomWithWheel();
                Event.current.Use();
            }
            else if (deleteKey)
            {
                DeleteNode(QuestEditorWindow.selectedNodeListIndex);
            }
            if (m_dragging)
            {
                PanWithMouse();
            }
            if (m_connecting && QuestEditorWindow.selectedNodeListIndex != -1)
            {
                DrawInProgressConnectorLine();
            }
        }

        private void ZoomWithWheel()
        {
            m_zoom -= Event.current.delta.y / 100f;
        }

        private void PanWithMouse()
        {
            var dX = (m_mousePos.x - m_prevMousePos.x) * m_zoom;
            var dY = (m_mousePos.y - m_prevMousePos.y) * m_zoom;
            if (Mathf.Abs(dX) > 0 || Mathf.Abs(dY) > 0)
            {
                m_panX += dX;
                m_panY += dY;
                QuestEditorWindow.RepaintNow();
            }
        }

        private void ClickOnCanvas()
        {
            QuestEditorWindow.selectedNodeListIndex = -1;
            for (int i = 0; i < m_nodeListProperty.arraySize; i++)
            {
                var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(i);
                var nodeRect = GetCanvasRect(nodeProperty);
                if (nodeRect.Contains(m_mousePos))
                {
                    QuestEditorWindow.selectedNodeListIndex = i;
                    var connectRect = new Rect(nodeRect.x, nodeRect.y + nodeRect.height - QuestEditorStyles.connectorImage.height - 8, nodeRect.width, QuestEditorStyles.connectorImage.height + 8);
                    if (connectRect.Contains(m_mousePos)) m_connecting = true;
                    break;
                }
            }
            QuestEditorWindow.SetSelectionToQuest();
            QuestEditorWindow.RepaintCurrentEditorNow();
        }

        private void DrawInProgressConnectorLine()
        {
            Rect mouseRect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 10, 10);
            var selectedNodeProperty = m_nodeListProperty.GetArrayElementAtIndex(QuestEditorWindow.selectedNodeListIndex);
            var selectedNodeRect = GetCanvasRect(selectedNodeProperty);
            var sourceRect = new Rect(selectedNodeRect.x, selectedNodeRect.y + selectedNodeRect.height - EditorGUIUtility.singleLineHeight, selectedNodeRect.width, EditorGUIUtility.singleLineHeight);
            DrawNodeCurve(sourceRect, mouseRect, QuestEditorStyles.NewConnectorColor);
            QuestEditorWindow.RepaintNow();
        }

        private void EndConnection()
        {
            int clickedIndex = -1;
            for (int i = 0; i < m_nodeListProperty.arraySize; i++)
            {
                var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(i);
                var nodeRect = GetCanvasRect(nodeProperty);
                if (nodeRect.Contains(m_mousePos))
                {
                    clickedIndex = i;
                    break;
                }
            }
            if (clickedIndex != -1 && clickedIndex != QuestEditorWindow.selectedNodeListIndex)
            {
                AddConnection(QuestEditorWindow.selectedNodeListIndex, clickedIndex);
            }
            m_connecting = false;
        }

        private void AddConnection(int sourceIndex, int destIndex)
        {
            if (!(0 <= sourceIndex && sourceIndex < m_nodeListProperty.arraySize && 0 <= destIndex && destIndex < m_nodeListProperty.arraySize)) return;
            var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(sourceIndex);
            UnityEngine.Assertions.Assert.IsNotNull(nodeProperty, "Quest Machine: Internal error - node property is null in AddConnection().");
            if (nodeProperty == null) return;
            var childIndexListProperty = nodeProperty.FindPropertyRelative("m_childIndexList");
            UnityEngine.Assertions.Assert.IsNotNull(childIndexListProperty, "Quest Machine: Internal error - m_childIndexList property is null in AddConnection().");
            if (childIndexListProperty == null) return;
            for (int i = 0; i < childIndexListProperty.arraySize; i++)
            {
                if (childIndexListProperty.GetArrayElementAtIndex(i).intValue == destIndex)
                {
                    return; // Don't allow duplicates.
                }
            }
            childIndexListProperty.arraySize++;
            childIndexListProperty.GetArrayElementAtIndex(childIndexListProperty.arraySize - 1).intValue = destIndex;
        }

        private void ClearConnections(int sourceIndex)
        {
            ClearConnections(m_nodeListProperty.GetArrayElementAtIndex(sourceIndex));
        }

        private void ClearConnections(SerializedProperty nodeProperty)
        {
            if (nodeProperty == null) return;
            var childIndexListProperty = nodeProperty.FindPropertyRelative("m_childIndexList");
            UnityEngine.Assertions.Assert.IsNotNull(childIndexListProperty, "Quest Machine: Internal error - m_childIndexList property is null in ClearConnections().");
            if (childIndexListProperty == null) return;
            childIndexListProperty.arraySize = 0;
        }

        private void ShowContextMenu()
        {
            int clickedIndex = -1;
            m_connecting = false;
            for (int i = 0; i < m_nodeListProperty.arraySize; i++)
            {
                var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(i);
                var nodeRect = GetCanvasRect(nodeProperty);
                if (nodeRect.Contains(m_mousePos))
                {
                    clickedIndex = i;
                    QuestEditorWindow.selectedNodeListIndex = i;
                    break;
                }
            }
            GenericMenu menu = new GenericMenu();
            if (Application.isPlaying && m_quest.isInstance)
            {
                menu.AddItem(new GUIContent("Set State/Inactive"), false, ContextCallback, new CallbackArgs(CallbackType.SetState, clickedIndex, QuestNodeState.Inactive));
                menu.AddItem(new GUIContent("Set State/Active"), false, ContextCallback, new CallbackArgs(CallbackType.SetState, clickedIndex, QuestNodeState.Active));
                menu.AddItem(new GUIContent("Set State/True"), false, ContextCallback, new CallbackArgs(CallbackType.SetState, clickedIndex, QuestNodeState.True));
            }
            else
            {
                if (clickedIndex >= 0)
                {
                    menu.AddItem(new GUIContent("Clear Connections"), false, ContextCallback, new CallbackArgs(CallbackType.ClearConnections, clickedIndex));
                    menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, new CallbackArgs(CallbackType.Delete, clickedIndex));
                    menu.AddSeparator("");
                }
                menu.AddItem(new GUIContent("New Node/Passthrough"), false, ContextCallback, new CallbackArgs(CallbackType.Add, QuestNodeType.Passthrough, Event.current.mousePosition, clickedIndex));
                menu.AddItem(new GUIContent("New Node/Condition"), false, ContextCallback, new CallbackArgs(CallbackType.Add, QuestNodeType.Condition, Event.current.mousePosition, clickedIndex));
                menu.AddItem(new GUIContent("New Node/Success"), false, ContextCallback, new CallbackArgs(CallbackType.Add, QuestNodeType.Success, Event.current.mousePosition, clickedIndex));
                menu.AddItem(new GUIContent("New Node/Failure"), false, ContextCallback, new CallbackArgs(CallbackType.Add, QuestNodeType.Failure, Event.current.mousePosition, clickedIndex));
            }
            menu.ShowAsContext();
        }

        private enum CallbackType { Add, Delete, ClearConnections, SetState }

        private struct CallbackArgs
        {
            public CallbackType callbackType;
            public int clickedIndex;
            public QuestNodeType questNodeType;
            public Vector2 mousePosition;
            public QuestNodeState newState;

            public CallbackArgs(CallbackType callbackType, int clickedIndex)
            {
                this.callbackType = callbackType;
                this.clickedIndex = clickedIndex;
                this.questNodeType = QuestNodeType.Success;
                this.mousePosition = Vector2.zero;
                this.newState = QuestNodeState.Inactive;
            }

            public CallbackArgs(CallbackType callbackType, QuestNodeType questNodeType, Vector2 mousePosition, int clickedIndex)
            {
                this.callbackType = callbackType;
                this.clickedIndex = clickedIndex;
                this.questNodeType = questNodeType;
                this.mousePosition = mousePosition;
                this.newState = QuestNodeState.Inactive;
            }

            public CallbackArgs(CallbackType callbackType, int clickedIndex, QuestNodeState newState)
            {
                this.callbackType = callbackType;
                this.clickedIndex = -1;
                this.questNodeType = QuestNodeType.Success;
                this.mousePosition = Vector2.zero;
                this.newState = newState;
            }
        }

        void ContextCallback(object obj)
        {
            if (obj == null || obj.GetType() != typeof(CallbackArgs) || m_nodeListProperty == null) return;

            var args = (CallbackArgs)obj;

            m_nodeListProperty.serializedObject.Update();

            switch (args.callbackType)
            {
                case CallbackType.Add:
                    AddNode(args.questNodeType, args.mousePosition, args.clickedIndex);
                    break;
                case CallbackType.ClearConnections:
                    ClearConnections(args.clickedIndex);
                    break;
                case CallbackType.Delete:
                    DeleteNode(args.clickedIndex);
                    break;
                case CallbackType.SetState:
                    var quest = m_questSerializedObject.targetObject as Quest;
                    if (quest != null && quest.nodeList != null && 0 <= QuestEditorWindow.selectedNodeListIndex && QuestEditorWindow.selectedNodeListIndex < quest.nodeList.Count)
                    {
                        var node = quest.nodeList[QuestEditorWindow.selectedNodeListIndex];
                        if (node == null) break;
                        node.SetState(args.newState, Application.isPlaying);
                    }
                    break;
            }
            m_nodeListProperty.serializedObject.ApplyModifiedProperties();
        }

        private void AddNode(QuestNodeType questNodeType, Vector2 mousePosition, int parentIndex)
        {
            var parentNodeProperty = (parentIndex >= 0) ? m_nodeListProperty.GetArrayElementAtIndex(parentIndex) : null;
            m_nodeListProperty.arraySize++;
            QuestEditorWindow.selectedNodeListIndex = m_nodeListProperty.arraySize - 1;
            var childIndex = m_nodeListProperty.arraySize - 1;
            var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(childIndex);
            UnityEngine.Assertions.Assert.IsNotNull(nodeProperty, "Quest Machine: Internal error - node property is null in AddNode().");
            if (nodeProperty == null) return;
            var idProperty = nodeProperty.FindPropertyRelative("m_id");
            UnityEngine.Assertions.Assert.IsNotNull(idProperty, "Quest Machine: Internal error - m_id property is null in AddNode().");
            if (idProperty == null) return;
            var internalNameProperty = nodeProperty.FindPropertyRelative("m_internalName");
            UnityEngine.Assertions.Assert.IsNotNull(internalNameProperty, "Quest Machine: Internal error - m_internalName property is null in AddNode().");
            if (internalNameProperty == null) return;
            var stateProperty = nodeProperty.FindPropertyRelative("m_state");
            UnityEngine.Assertions.Assert.IsNotNull(stateProperty, "Quest Machine: Internal error - m_state property is null in AddNode().");
            if (stateProperty == null) return;
            var nodeTypeProperty = nodeProperty.FindPropertyRelative("m_nodeType");
            UnityEngine.Assertions.Assert.IsNotNull(nodeTypeProperty, "Quest Machine: Internal error - m_nodeType property is null in AddNode().");
            if (nodeTypeProperty == null) return;
            var isOptionalProperty = nodeProperty.FindPropertyRelative("m_isOptional");
            UnityEngine.Assertions.Assert.IsNotNull(isOptionalProperty, "Quest Machine: Internal error - m_isOptional property is null in AddNode().");
            if (isOptionalProperty == null) return;
            var stateInfoListProperty = nodeProperty.FindPropertyRelative("m_stateInfoList");
            UnityEngine.Assertions.Assert.IsNotNull(stateInfoListProperty, "Quest Machine: Internal error - m_stateInfoList property is null in AddNode().");
            if (stateInfoListProperty == null) return;
            var conditionSetProperty = nodeProperty.FindPropertyRelative("m_conditionSet");
            UnityEngine.Assertions.Assert.IsNotNull(conditionSetProperty, "Quest Machine: Internal error - m_conditionSet property is null in AddNode().");
            if (conditionSetProperty == null) return;
            var conditionListProperty = conditionSetProperty.FindPropertyRelative("m_conditionList");
            UnityEngine.Assertions.Assert.IsNotNull(conditionListProperty, "Quest Machine: Internal error - m_conditionList property is null in AddNode().");
            if (conditionListProperty == null) return;
            var conditionCountModeProperty = conditionSetProperty.FindPropertyRelative("m_conditionCountMode");
            UnityEngine.Assertions.Assert.IsNotNull(conditionCountModeProperty, "Quest Machine: Internal error - m_conditionCountMode property is null in AddNode().");
            if (conditionCountModeProperty == null) return;
            var canvasRectProperty = nodeProperty.FindPropertyRelative("m_canvasRect");
            UnityEngine.Assertions.Assert.IsNotNull(canvasRectProperty, "Quest Machine: Internal error - m_canvasRect property is null in AddNode().");
            if (canvasRectProperty == null) return;
            var initialName = (questNodeType == QuestNodeType.Success) ? "Success"
                : ((questNodeType == QuestNodeType.Failure) ? "Failure" 
                   : questNodeType.ToString() + " " + (m_nodeListProperty.arraySize - 1));
            StringFieldDrawer.SetStringFieldValue(idProperty, initialName);
            StringFieldDrawer.SetStringFieldValue(internalNameProperty, string.Empty);
            stateProperty.enumValueIndex = (int)QuestState.WaitingToStart;
            nodeTypeProperty.enumValueIndex = (int)questNodeType;
            isOptionalProperty.boolValue = false;
            stateInfoListProperty.ClearArray();
            conditionListProperty.ClearArray();
            conditionCountModeProperty.enumValueIndex = (int)ConditionCountMode.All;
            ClearConnections(nodeProperty);
            var height = (questNodeType == QuestNodeType.Success || questNodeType == QuestNodeType.Failure) ? QuestEditorStyles.shortNodeHeight : QuestEditorStyles.nodeHeight;
            var rect = (parentNodeProperty != null) ? GetRectForNewChild(parentNodeProperty, height)
                : new Rect(mousePosition.x, mousePosition.y, QuestEditorStyles.nodeWidth, height);
            canvasRectProperty.rectValue = rect;
            if (parentNodeProperty != null) AddConnection(parentIndex, childIndex);
        }

        private Rect GetRectForNewChild(SerializedProperty parentNodeProperty, float height)
        {
            var canvasRectProperty = parentNodeProperty.FindPropertyRelative("m_canvasRect");
            UnityEngine.Assertions.Assert.IsNotNull(canvasRectProperty, "Quest Machine: Internal error - parent's m_canvasRect property is null in AddNode().");
            if (canvasRectProperty == null) return new Rect(0, 0, QuestEditorStyles.nodeWidth, height);
            return new Rect(canvasRectProperty.rectValue.x, canvasRectProperty.rectValue.y + canvasRectProperty.rectValue.height + 20f, QuestEditorStyles.nodeWidth, height);
        }

        private void DeleteNode(int index)
        {
            if (!EditorUtility.DisplayDialog("Delete Quest Node", "Are you sure you want to delete this quest node?", "OK", "Cancel")) return;
            RemoveParentConnectionsTo(index);
            m_nodeListProperty.DeleteArrayElementAtIndex(index);
            QuestEditorWindow.selectedNodeListIndex = -1;
            QuestEditorWindow.RepaintNow();
        }

        private void RemoveParentConnectionsTo(int childIndex)
        {
            if (m_nodeListProperty == null) return;
            for (int i = 0; i < m_nodeListProperty.arraySize; i++)
            {
                var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(i);
                if (nodeProperty == null) continue;
                var childIndexListProperty = nodeProperty.FindPropertyRelative("m_childIndexList");
                UnityEngine.Assertions.Assert.IsNotNull(childIndexListProperty, "Quest Machine: Internal error - m_childIndexList property is null in RemoveParentConnections().");
                if (childIndexListProperty == null) continue;

                for (int j = childIndexListProperty.arraySize - 1; j >= 0; j--)
                {
                    if (childIndexListProperty.GetArrayElementAtIndex(j).intValue == childIndex)
                    {
                        childIndexListProperty.DeleteArrayElementAtIndex(j);
                    }
                }
            }
        }

        private void DrawNodeCurve(Rect start, Rect end, Color color)
        {
            Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + 24 * Vector3.up;
            Vector3 endTan = endPos + -24 * Vector3.up;
            Color shadowCol = new Color(0, 0, 0, .06f);
            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 2);
        }

        #endregion

        #region Gear Menu

        protected virtual void DrawGearMenu(Rect position)
        {
            if (MoreEditorGuiUtility.DoGearMenu(new Rect(position.width - 2 - MoreEditorGuiUtility.GearWidth, 2, MoreEditorGuiUtility.GearWidth, MoreEditorGuiUtility.GearHeight)))
            {
                var menu = new GenericMenu();
                AddCanvasControlGearMenuItems(menu);
                AddExtraGearMenuItems(menu);
                if (Application.isPlaying) AddRuntimeGearMenuItems(menu);
                menu.ShowAsContext();
            }
        }

        protected virtual void AddCanvasControlGearMenuItems(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Pan/Top Left"), false, PanTopLeft, null);
            menu.AddItem(new GUIContent("Zoom/Lock"), QuestEditorPrefs.zoomLock, ToggleZoomLock, null);
            menu.AddItem(new GUIContent("Zoom/25%"), false, Zoom, 0.25f);
            menu.AddItem(new GUIContent("Zoom/50%"), false, Zoom, 0.5f);
            menu.AddItem(new GUIContent("Zoom/100%"), false, Zoom, 1f);
            menu.AddItem(new GUIContent("Zoom/150%"), false, Zoom, 1.5f);
            menu.AddItem(new GUIContent("Zoom/200%"), false, Zoom, 2f);
        }

        protected virtual void AddExtraGearMenuItems(GenericMenu menu)
        {
            if (QuestEditorWindow.selectedQuest == null)
            {
                menu.AddDisabledItem(new GUIContent("Text/Tags To Text Table"));
            }
            else
            {
                menu.AddItem(new GUIContent("Text/Tags To Text Table"), false, OpenTagsToTextTableWizard);
            }
        }

        protected virtual void AddRuntimeGearMenuItems(GenericMenu menu)
        { 
            menu.AddItem(new GUIContent("Refresh Frequency/0.5 sec", "Refresh window every 0.5 second."), 
                Mathf.Approximately(0.5f, QuestEditorPrefs.runtimeRepaintFrequency), SetRuntimeRepaintFrequency, 0.5f);
            menu.AddItem(new GUIContent("Refresh Frequency/1 sec", "Refresh window every 1 second."), 
                Mathf.Approximately(1f, QuestEditorPrefs.runtimeRepaintFrequency), SetRuntimeRepaintFrequency, 1f);
            menu.AddItem(new GUIContent("Refresh Frequency/5 sec", "Refresh window every 5 seconds."), 
                Mathf.Approximately(5f, QuestEditorPrefs.runtimeRepaintFrequency), SetRuntimeRepaintFrequency, 5f);
            menu.AddItem(new GUIContent("Refresh Frequency/10 sec", "Refresh window every 10 seconds."), 
                Mathf.Approximately(10f, QuestEditorPrefs.runtimeRepaintFrequency), SetRuntimeRepaintFrequency, 10f);
            menu.AddItem(new GUIContent("Refresh Frequency/Never", "Never refresh at runtime."), 
                Mathf.Approximately(0f, QuestEditorPrefs.runtimeRepaintFrequency), SetRuntimeRepaintFrequency, 0f);
            if (m_quest != null && m_quest.isProcedurallyGenerated)
            {
                menu.AddItem(new GUIContent("Save As Asset...", "Save this procedurally-generated quest as an asset."), false, SaveGeneratedQuestAsAsset);
            }
        }

        protected void Pan(float x, float y)
        {
            m_panX = x;
            m_panY = y;
        }

        protected void Zoom(float zoom)
        {
            m_zoom = zoom;
        }

        protected void PanTopLeft(object data)
        {
            Pan(0, 0);
        }

        protected void Zoom(object data)
        {
            m_zoom = (float)data;
        }

        protected void ToggleZoomLock(object data)
        {
            QuestEditorPrefs.zoomLock = !QuestEditorPrefs.zoomLock;
        }

        protected void SetRuntimeRepaintFrequency(object data)
        {
            QuestEditorPrefs.runtimeRepaintFrequency = (float)data;
        }

        private void SaveGeneratedQuestAsAsset()
        {
            var filename = EditorUtility.SaveFilePanelInProject("Save Quest As", "New Quest", "asset", "Save quest as");
            if (string.IsNullOrEmpty(filename)) return;
            QuestEditorAssetUtility.SaveQuestAsAsset(m_quest, filename, true);
        }

        private void OpenTagsToTextTableWizard()
        {
            QuestTagsToTextTableWizard.Open();
        }

        #endregion

    }
}
