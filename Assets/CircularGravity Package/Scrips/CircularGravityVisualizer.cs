/*******************************************************************************************
 *       Author: Lane Gresham, AKA LaneMax
 *         Blog: http://lanemax.blogspot.com/
 *      Version: 2.50
 * Created Date: 04/15/13 
 * Last Updated: 12/20/13
 *  
 *  Description: 
 *  
 *      The CircularGravityVisualizer is used in correlation with a GameObject that contains
 *      the CircularGravity script and allows you to interact with the objects CircularGravity 
 *      properties. A good tool for getting an idea for a scene or debugging.
 * 
 *  How To Use:
 *  
 *      Simply drag and drop / assign this script to whatever GameObject you would like. Then 
 *      drag and drop the object that contains the CircularGravity script into the 
 *      circularGravityGameObject property.
 * 
 *  Inputs:
 *      
 *      circularGravityGameObject: Assign the current object that has CircularGravity to display the GUI for those properties.
 *      
 *      guiLocation: Used to set the GUI location.
 *      
 *      sizeProperties->
 *          min: Min size of size.
 *          max: Max size of size.
 *          
 *      forcePower->
 *          min: Min size of force power.
 *          max: Max size of force power.
 *          
 *      capsuleRadius->
 *          min: Min size of capsule radius.
 *          max: Max size of capsule radius.
 *          
 *      pulseSpeed->
 *          min: Min size of pulse speed.
 *          max: Max size of pulse speed.
 *          
 *      pulseMinSize->
 *          min: Min size of pulse min.
 *          max: Max size of pulse min.
 *          
 *      pulseMaxSize->
 *          min: Min size of pulse max.
 *          max: Max size of pulse max.
 *          
 *      defaultGameObjectEffect: Sets the default effect for when using the visualizer.
 * 
 *      toggleGravityEffect: Toggles cool gravity effect.
 * 
 *      noGravity: Defaults if to use no gravity or not on start.
 * 
 *******************************************************************************************/
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace CircularGravityForce
{
    public class CircularGravityVisualizer : MonoBehaviour
    {
        #region Properties

        //Assign the current object that has CircularGravity to display the GUI for thoese properties
        public GameObject circularGravityGameObject;

        //Used to set the GUI location
        public Vector2 guiLocation = new Vector2(0f, 0f);

        //Min and Max for the hSliderRadius
        [System.Serializable]
        public class SizeProperties
        {
            public float min = .01f;
            public float max = 35f;
        }
        public SizeProperties sizeProperties;

        //Min and Max for the hSliderForcePower
        [System.Serializable]
        public class ForcePower
        {
            public float min = -200f;
            public float max = 200f;
        }
        public ForcePower forcePower;

        //Min and Max for the hSliderForcePower
        [System.Serializable]
        public class CapsuleRadius
        {
            public float min = .01f;
            public float max = 25f;
        }
        public CapsuleRadius capsuleRadius;

        //Min and Max for the hSliderPulseSpeed
        [System.Serializable]
        public class PulseSpeed
        {
            public float min = 0f;
            public float max = 100f;
        }
        public PulseSpeed pulseSpeed;

        //Min and Max for the hSliderPulseMinSize
        [System.Serializable]
        public class PulseMinSize
        {
            public float min = 1f;
            public float max = 74f;
        }
        public PulseMinSize pulseMinSize;

        //Min and Max for the hSliderPulseMaxSize
        [System.Serializable]
        public class PulseMaxSize
        {
            public float min = 2f;
            public float max = 75f;
        }
        public PulseMaxSize pulseMaxSize;

        //Min and Max for the hSliderPulseMaxSize
        [System.Serializable]
        public class SceneTimeScale
        {
            public float min = 0f;
            public float max = 1f;
        }
        public SceneTimeScale sceneTimeScale;

        //Default GameObject Effect
        public GameObject defaultAttachedGameObject;

        //Toggles all effected objects to angled towards face
        public bool toggleAngleToForce = false;

        //Toggles cool gravity effect.
        public bool toggleGravityEffect = false;

        //Default for toggleNoGravity
        public bool noGravity;

        //GUI Properties
        private float hSliderSize;
        private float hSceneTimeScale;
        private float hSliderForcePower;
        private float hSliderCapsuleRadius;
        private bool togglePulse;
        private bool toggleNoGravity;
        private bool toggleDrawForce;
        private float hSliderPulseSpeed;
        private float hSliderPulseMinSize;
        private float hSliderPulseMaxSize;

        //Gravity Options
        private Vector3 previewsGravity = new Vector3(0f, -9.78f, 0f);
        private Vector3 gravityZeroG = new Vector3(0f, 0f, 0f);

        #endregion

        #region Unity Events

        //Use this for initialization
        void Start()
        {
            //Default Gravity
            Physics.gravity = gravityZeroG;

            try
            {
                CircularGravity circularGravity = circularGravityGameObject.GetComponent<CircularGravity>();

                //Default GUI values 
                hSliderSize = circularGravity.size;
                hSceneTimeScale = Time.timeScale;
                hSliderForcePower = circularGravity.forcePower;
                hSliderCapsuleRadius = circularGravity.capsuleRadius;
                togglePulse = circularGravity.pulseProperties.pulse;
                toggleDrawForce = circularGravity.drawGravityProperties.drawGravityForce;
                hSliderPulseSpeed = circularGravity.pulseProperties.speed;
                hSliderPulseMinSize = circularGravity.pulseProperties.minSize;
                hSliderPulseMaxSize = circularGravity.pulseProperties.maxSize;
                noGravity = toggleNoGravity;
            }
            catch (System.Exception)
            {
                Debug.LogWarning("Error reading CircularGravity GameObject");
            }
        }

        void Awake()
        {
            //Defaults gravity
            if (Physics.gravity == gravityZeroG || noGravity == true)
            {
                toggleNoGravity = true;
            }
            else
            {
                toggleNoGravity = false;

                previewsGravity = Physics.gravity;
            }
        }

        //Update is called once per frame
        void Update()
        {
            ToggleGravity();
        }

        //Draws the GUI
        void OnGUI()
        {
            GUIStyle labelGUIStyle = new GUIStyle();
            labelGUIStyle.normal.textColor = Color.white;
            labelGUIStyle.alignment = TextAnchor.MiddleRight;

            GUI.Label(new Rect(guiLocation.x + 5, guiLocation.y + 5, 100, 20), "Shape:", labelGUIStyle);
            if (GUI.Button(new Rect(guiLocation.x + 110, guiLocation.y + 5, 63, 20), "Sphere"))
            {
                circularGravityGameObject.GetComponent<CircularGravity>().shape = CircularGravity.Shape.Sphere;
            }
            else if (GUI.Button(new Rect(guiLocation.x + 178, guiLocation.y + 5, 63, 20), "Capsule"))
            {
                circularGravityGameObject.GetComponent<CircularGravity>().shape = CircularGravity.Shape.Capsule;
            }
            else if (GUI.Button(new Rect(guiLocation.x + 248, guiLocation.y + 5, 63, 20), "Raycast"))
            {
                circularGravityGameObject.GetComponent<CircularGravity>().shape = CircularGravity.Shape.RayCast;
            }

            GUI.Label(new Rect(guiLocation.x + 5, guiLocation.y + 25, 100, 20), "Force Power:", labelGUIStyle);
            hSliderForcePower = GUI.HorizontalSlider(new Rect(guiLocation.x + 110, guiLocation.y + 30, 200, 20), hSliderForcePower, forcePower.min, forcePower.max);

            if (circularGravityGameObject.GetComponent<CircularGravity>().shape == CircularGravity.Shape.Capsule)
            {
                GUI.Label(new Rect(guiLocation.x + 5, guiLocation.y + 50, 100, 20), "Capsule Radius:", labelGUIStyle);
                hSliderCapsuleRadius = GUI.HorizontalSlider(new Rect(guiLocation.x + 110, guiLocation.y + 55, 200, 20), hSliderCapsuleRadius, capsuleRadius.min, capsuleRadius.max);
            }
            toggleGravityEffect = GUI.Toggle(new Rect(guiLocation.x + 315, guiLocation.y + 25, 150, 20), toggleGravityEffect, "Gravity Effect");
            toggleAngleToForce = GUI.Toggle(new Rect(guiLocation.x + 315, guiLocation.y + 50, 150, 20), toggleAngleToForce, "Angle to Force");
            togglePulse = GUI.Toggle(new Rect(guiLocation.x + 315, guiLocation.y + 75, 50, 20), togglePulse, "Pulse");
            toggleDrawForce = GUI.Toggle(new Rect(guiLocation.x + 315, guiLocation.y + 100, 80, 20), toggleDrawForce, "Draw Force");
            toggleNoGravity = GUI.Toggle(new Rect(guiLocation.x + 315, guiLocation.y + 125, 80, 20), toggleNoGravity, "No Gravity");

            float topLocButton;

            if (!togglePulse)
            {
                GUI.Label(new Rect(guiLocation.x + 5, guiLocation.y + 75, 100, 20), "Custom Size:", labelGUIStyle);
                hSliderSize = GUI.HorizontalSlider(new Rect(guiLocation.x + 110, guiLocation.y + 80, 200, 20), hSliderSize, sizeProperties.min, sizeProperties.max);

                GUI.Label(new Rect(guiLocation.x + 5, guiLocation.y + 100, 100, 20), "Time Scale:", labelGUIStyle);
                hSceneTimeScale = GUI.HorizontalSlider(new Rect(guiLocation.x + 110, guiLocation.y + 105, 200, 20), hSceneTimeScale, sceneTimeScale.min, sceneTimeScale.max);

                topLocButton = 125;
            }
            else
            {
                GUI.Label(new Rect(guiLocation.x + 5, guiLocation.y + 75, 100, 20), "pulse Max Size:", labelGUIStyle);
                hSliderPulseMaxSize = GUI.HorizontalSlider(new Rect(guiLocation.x + 110, guiLocation.y + 80, 200, 20), hSliderPulseMaxSize, pulseMaxSize.min, pulseMaxSize.max);

                GUI.Label(new Rect(guiLocation.x + 5, guiLocation.y + 100, 100, 20), "Pulse Min Size:", labelGUIStyle);
                hSliderPulseMinSize = GUI.HorizontalSlider(new Rect(guiLocation.x + 110, guiLocation.y + 105, 200, 20), hSliderPulseMinSize, pulseMinSize.min, pulseMinSize.max);

                GUI.Label(new Rect(guiLocation.x + 5, guiLocation.y + 125, 100, 20), "Pulse Speed:", labelGUIStyle);
                hSliderPulseSpeed = GUI.HorizontalSlider(new Rect(guiLocation.x + 110, guiLocation.y + 130, 200, 20), hSliderPulseSpeed, pulseSpeed.min, pulseSpeed.max);

                GUI.Label(new Rect(guiLocation.x + 5, guiLocation.y + 150, 100, 20), "Time Scale:", labelGUIStyle);
                hSceneTimeScale = GUI.HorizontalSlider(new Rect(guiLocation.x + 110, guiLocation.y + 155, 200, 20), hSceneTimeScale, sceneTimeScale.min, sceneTimeScale.max);

                topLocButton = 175;
            }

            if (GUI.Button(new Rect(guiLocation.x + 260, guiLocation.y + topLocButton, 50, 20), "Reset"))
            {
                Application.LoadLevel(Application.loadedLevelName);

                hSceneTimeScale = 1f;
            }

            try
            {
                CircularGravity circularGravity = circularGravityGameObject.GetComponent<CircularGravity>();

                //Defaults the effect
                if (circularGravity.specialEffect.attachedGameObject == null && defaultAttachedGameObject != null)
                {
                    circularGravity.specialEffect.attachedGameObject = defaultAttachedGameObject;
                }

                if (!togglePulse)
                {
                    circularGravity.size = hSliderSize;
                }

                circularGravity.constraintProperties.alignToForce = toggleAngleToForce;

                if (toggleGravityEffect)
                {
                    circularGravity.specialEffect.timeEffected = 3.0f;
                    circularGravity.specialEffect.physicsEffect = CircularGravity.PhysicsEffect.NoGravity;
                }
                else
                {
                    circularGravity.specialEffect.timeEffected = 0.0f;
                    circularGravity.specialEffect.physicsEffect = CircularGravity.PhysicsEffect.None;
                }

                circularGravity.forcePower = hSliderForcePower;
                circularGravity.capsuleRadius = hSliderCapsuleRadius;
                circularGravity.pulseProperties.pulse = togglePulse;
                circularGravity.drawGravityProperties.drawGravityForce = toggleDrawForce;
                circularGravity.pulseProperties.speed = hSliderPulseSpeed;
                circularGravity.pulseProperties.minSize = hSliderPulseMinSize;
                circularGravity.pulseProperties.maxSize = hSliderPulseMaxSize;

                Time.timeScale = hSceneTimeScale;
            }
            catch (System.Exception)
            {
                Debug.LogWarning("Error reading CircularGravity GameObject");
            }
        }

        #endregion

        #region Functions

        //Used to switch gravity toggle the gravity
        private void ToggleGravity()
        {
            if (toggleNoGravity)
            {
                if (Physics.gravity != gravityZeroG)
                    Physics.gravity = gravityZeroG;
            }
            else
            {
                if (Physics.gravity != previewsGravity)
                    Physics.gravity = previewsGravity;
            }
        }

        #endregion
    }
}