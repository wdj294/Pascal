/*******************************************************************************************
 *       Author: Lane Gresham, AKA LaneMax
 *         Blog: http://lanemax.blogspot.com/
 *      Version: 2.50
 * Created Date: 04/21/14
 * Last Updated: 04/21/14
 *  
 *  Description: 
 *      Adds menu item to Toolbar under Tools->Circular Gravity Force
 *      
*******************************************************************************************/
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CircularGravityForce
{
    public class CircularGravityTool : EditorWindow
    {
        //Constructor
        public CircularGravityTool()
        {
        }

        #region ToolBar

        // Add menu named "CGF Tool" to Toolbar
        [MenuItem("Tools/Circular Gravity Force")]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = EditorWindow.GetWindow(typeof(CircularGravityTool));
			editorWindow.title = "CGF Tool";
        }

        #endregion

        #region Properties

        //GUI Properties
        private Vector2 scrollPos;

        private float cgfSize = 10;
        private float cgfForcePower = 10;
        private CircularGravity.Shape cgfShape = CircularGravity.Shape.Sphere;
        private float cgfCapsuleRadius = 2;
        private bool cgfAngleToForce = false;
        private bool cgfDrawGravityForce = true;
        private float cgfDrawGravityForceThickness = 0.05f;
        private bool cgfGizmos = true;
        private bool cgfVisualizer = false;
        private float cgfVisualizerGUIx = 0f;
        private float cgfVisualizerGUIy = 0f;
        private bool buttonCreate = false;

        #endregion

        #region Unity Functions

        void Update()
        {
            if (buttonCreate)
            {
                //Creates empty gameobject.
                GameObject cgf = new GameObject();

                //Creates Circular Gravity Force component
                CircularGravity circularGravity = cgf.AddComponent<CircularGravity>();

                //Sets up properties
                circularGravity.shape = cgfShape;
                circularGravity.forcePower = cgfForcePower;
                circularGravity.size = cgfSize;
                circularGravity.capsuleRadius = cgfCapsuleRadius;
                circularGravity.constraintProperties.alignToForce = cgfAngleToForce;

                circularGravity.drawGravityProperties.drawGravityForce = cgfDrawGravityForce;
                circularGravity.drawGravityProperties.thickness = cgfDrawGravityForceThickness;

                if (cgfVisualizer)
                {
                    CircularGravityVisualizer circularGravityVisualizer = cgf.AddComponent<CircularGravityVisualizer>();

                    circularGravityVisualizer.circularGravityGameObject = cgf;
                    circularGravityVisualizer.guiLocation.x = cgfVisualizerGUIx;
                    circularGravityVisualizer.guiLocation.y = cgfVisualizerGUIy;
                }

                //Sets gameojbect Name
                cgf.name = "CGF";

                //Creates Circular Gravity Force Gizmos componet
                if (cgfGizmos)
                {
                    cgf.AddComponent<CircularGravityGizmos>();
                }

                //Sets the create location for the Circular Gravity Force gameobject
				if(SceneView.lastActiveSceneView != null)
				{
                	cgf.transform.position = SceneView.lastActiveSceneView.pivot;
					
					//Sets the Circular Gravity Force gameobject selected in the hierarchy
	                Selection.activeGameObject = cgf;
	
	                //focus the editor camera on the Circular Gravity Force gameobject
	                SceneView.lastActiveSceneView.FrameSelected();
				}
				else
				{
					cgf.transform.position = Vector3.zero;
				}

                //Disables the buttonCreateNewCGF
                buttonCreate = false;
            }
        }

        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUIUtility.LookLikeControls(120f, 75f);

            cgfSize = EditorGUILayout.FloatField("Size:", cgfSize, GUILayout.ExpandWidth(false));
            cgfForcePower = EditorGUILayout.FloatField("Force Power:", cgfForcePower, GUILayout.ExpandWidth(false));

            cgfShape = (CircularGravity.Shape)EditorGUILayout.EnumPopup("Shape:", cgfShape, GUILayout.ExpandWidth(false));

            if (cgfShape == CircularGravity.Shape.Capsule)
            {
                cgfCapsuleRadius = EditorGUILayout.FloatField("Capsule Radius:", cgfCapsuleRadius, GUILayout.ExpandWidth(false));
            }

            cgfAngleToForce = EditorGUILayout.Toggle("Align to Force:", cgfAngleToForce, GUILayout.ExpandWidth(false));

            cgfDrawGravityForce = EditorGUILayout.Toggle("Draw Gravity Force:", cgfDrawGravityForce, GUILayout.ExpandWidth(false));
            if (cgfDrawGravityForce)
            {
                cgfDrawGravityForceThickness = EditorGUILayout.FloatField("   Thickness:", cgfDrawGravityForceThickness, GUILayout.ExpandWidth(false));
            }

            cgfGizmos = EditorGUILayout.Toggle("Gizmos:", cgfGizmos, GUILayout.ExpandWidth(false));

            cgfVisualizer = EditorGUILayout.Toggle("Visualizer:", cgfVisualizer, GUILayout.ExpandWidth(false));

            if (cgfVisualizer)
            {
                cgfVisualizerGUIx = EditorGUILayout.FloatField("   x:", cgfVisualizerGUIx, GUILayout.ExpandWidth(false));
                cgfVisualizerGUIy = EditorGUILayout.FloatField("   y:", cgfVisualizerGUIy, GUILayout.ExpandWidth(false));
            }

            buttonCreate = GUILayout.Button("Create ");

            EditorGUILayout.EndScrollView();
        }

        #endregion
    }
}

#endif