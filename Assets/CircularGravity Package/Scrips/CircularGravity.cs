/*******************************************************************************************
 *       Author: Lane Gresham, AKA LaneMax
 *         Blog: http://lanemax.blogspot.com/
 *      Version: 2.50
 * Created Date: 04/15/13 
 * Last Updated: 04/21/14
 *  
 *  Description: 
 *  
 *      The Circular Gravity Force v2.50 script allows you to easily drag and drop a customizable
 *      gravitational force on to any GameObject within your project with a positive or negative
 *      power. Features include, shaping the gravitational force to ether a sphere, capsule, or
 *      even a raycast, also customizable force points giving you the flexibility to set one or
 *      multiple points where the force is coming from within the gravitational area giving you
 *      tons of ways to customize your effect, also tag and trigger filtering allows you to 
 *      filter out or include unwanted objects. Circular Gravity Force makes it incredibly easy 
 *      to add customizable gravity/force to planets, cannons, magnets, explosions, tractor 
 *      beams, or even rail guns, the possibilities are endless!
 *      
 *  How To Use:
 *  
 *      Simply drag and drop / assign this script to whatever GameObject you would like the
 *      circular gravity force to emanate from.
 * 
 *  Inputs:
 * 
 *      enable: Enable/Disable CircularGravity.
 *      
 *      forcePower: Power for the force, can be negative or positive.
 * 
 * 		shape: Shape of the CirularGravity
 * 		
 * 		forcePoint: Force starting point where the force is created, if not supplied it will use this.transform.position instead.
 * 	
 * 		forcePoints: Force points allowing you to add multiple force points, be careful this can affect performance if not used correctly.
 * 
 * 		ConstraintProperties->
 * 			alignToForce: Enables objects to always angle towards the force point.
 * 
 *      sizeProperties->
 *          size: Size of the force.
 *      	capsuleRadius: Capsule Radius, used only when using the capsule shape.
 * 
 *      specialEffect->
 *          timeEffected: Time the effect lasts.
 *          AttachGameObject: Attach GameObject to effected object.
 *          physicsEffect: Physics Effect.
 *          
 *      pulseProperties->
 *          pulse: Enable a Pulse.
 *          speed: Pulsing speed if pulse if enabled.
 *          minSize: Minimum pulse size.
 *          maxSize: Maximum pulse size.
 * 
 *      triggerAreaFilter->
 *          triggerArea: This is the trigger that will be used for the area filtering.
 *          triggerAreaFilterOptions: Trigger filter options.
 * 
 *      tagFilter->
 *          tagFilterOptions: Tag filter options.
 *          tagsList: Tags used for the filter option.
 * 
 *      DrawGravityProperties->
 *          thickness: Thickness of the line drawn.
 *          gravityLineMaterial: Material for line drawn.
 *          drawGravityForce: Used to see gravity area of gravity.
 *          
 * 
*******************************************************************************************/
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace CircularGravityForce
{
    public class CircularGravity : MonoBehaviour
    {
        #region Constructor

        public CircularGravity()
        {
			forcePoints = new List<GameObject>();
            constraintProperties = new ConstraintProperties();
            specialEffect = new SpecialEffect();
            pulseProperties = new PulseProperties();
            triggerAreaFilter = new TriggerAreaFilter();
            tagFilter = new TagFilter();
            drawGravityProperties = new DrawGravityProperties();
        }

        #endregion

        #region Properties

        //Enable/Disable CircularGravity
        public bool enable = true;

        //Radius of the force
        public float size = 10f;

        //Capsule Radius, used only when using the capsule shape
        public float capsuleRadius = 2f;

        //Power for the force, can be negative or positive
        public float forcePower = 10f;

        //Shape of the CirularGravity
        public Shape shape = CircularGravity.Shape.Sphere;

        //Force Point where the force is created, if not supplied it will use this.transform.position instead
        public GameObject forcePoint;

        //Force points allowing you to add multiple force points, be careful this can affect performance if not used correctly
        public List<GameObject> forcePoints;

        [System.Serializable]
        public class ConstraintProperties
        {
            //Enables objects to always angle towards the force point
            public bool alignToForce = false;
        }
        public ConstraintProperties constraintProperties;

        [System.Serializable]
        public class SpecialEffect
        {
            //Time the effect lasts
            public float timeEffected = 0.0f;

            //Attach GameObject to effected object.
            public GameObject attachedGameObject;

            //Physics Effect
            public PhysicsEffect physicsEffect = PhysicsEffect.None;

            //Holds all the effected objects of the timeEffected
            public EffectedObjects effectedObjects;

            //Constructor
            public SpecialEffect()
            {
                effectedObjects = new EffectedObjects();
            }
        }
        public SpecialEffect specialEffect;

        //Used for when creating the GameObject on an effected item
        static private string SpecialEffectGameObject = "CirularGravityForce_SpecialEffect";

        //Pulse properties
        [System.Serializable]
        public class PulseProperties
        {
            //Enable a Pulse
            public bool pulse = false;

            //Pulsing speed if pulse if enabled
            public float speed = 50f;

            //Minimum pulse size
            public float minSize = 1f;

            //Maximum pulse size
            public float maxSize = 5f;
        }
        public PulseProperties pulseProperties;

        //Trigger Area Filter
        [System.Serializable]
        public class TriggerAreaFilter
        {
            //Trigger Object
            public Collider triggerArea;

            //Trigger Options
            public enum TriggerAreaFilterOptions
            {
                Disabled,
                OnlyEffectWithinTigger,
                DontEffectWithinTigger,
            }
            public TriggerAreaFilterOptions triggerAreaFilterOptions = TriggerAreaFilterOptions.Disabled;
        }
        public TriggerAreaFilter triggerAreaFilter;

        //Tag filtering options
        [System.Serializable]
        public class TagFilter
        {
            //Tag filter options
            public enum TagFilterOptions
            {
                Disabled,
                OnlyEffectListedTags,
                DontEffectListedTags,
            }
            public TagFilterOptions tagFilterOptions = TagFilterOptions.Disabled;

            //Tags used for the filter option
            public string[] tagsList;
        }
        public TagFilter tagFilter;

        //Draw gravity properties
        [System.Serializable]
        public class DrawGravityProperties
        {
            //Thinkness of the line drawn
            public float thickness = 0.05f;

            public Material gravityLineMaterial;

            //Used to see gravity area of gravity
            public bool drawGravityForce = true;
        }
        public DrawGravityProperties drawGravityProperties;

        //Used to tell whether to add or subtract to pulse
        private bool pulse_Positive;

        //Line Object
        static private string CirularGravityLineName = "CirularGravityForce_LineDisplay";
        private GameObject CirularGravityLine;

        //Force Types
        public enum Shape
        {
            Sphere,
            Capsule,
            RayCast,
        }

        //Effect Types
        public enum PhysicsEffect
        {
            None,
            NoGravity,
        }

        #endregion

        #region Classes

        //Manages all effected objects for when using SpecialEffect
        public class EffectedObjects
        {
            //List of all EffectedObject
            public List<EffectedObject> effectedObjectList { get; set; }

            //Constructor
            public EffectedObjects()
            {
                effectedObjectList = new List<EffectedObject>();
            }

            //Used to add to the effectedObjectList
            public void AddedEffectedObject(Rigidbody touchedObject, PhysicsEffect physicsEffect, GameObject attachedGameObject)
            {
                if (effectedObjectList.Count == 0)
                {
                    EffectedObject effectedObject = new EffectedObject(Time.time, touchedObject, physicsEffect);
                    effectedObjectList.Add(effectedObject);

                    CreateAttachedGameObject(touchedObject, attachedGameObject);
                }
                else if ((effectedObjectList.Count > 0))
                {
                    bool checkIfExists = false;

                    foreach (EffectedObject item in effectedObjectList)
                    {
                        if (item.touchedObject == touchedObject)
                        {
                            item.effectedTime = Time.time;
                            checkIfExists = true;
                            break;
                        }
                    }

                    if (!checkIfExists)
                    {
                        EffectedObject effectedObject = new EffectedObject(Time.time, touchedObject, physicsEffect);
                        effectedObjectList.Add(effectedObject);

                        CreateAttachedGameObject(touchedObject, attachedGameObject);
                    }
                }
            }

            //Creates the attached gameobject for the effect
            private static void CreateAttachedGameObject(Rigidbody touchedObject, GameObject attachGameObject)
            {
                if (attachGameObject != null)
                {
                    if (touchedObject.transform.Find(SpecialEffectGameObject) == null)
                    {
                        GameObject newAttachGameObject = Instantiate(attachGameObject, touchedObject.gameObject.transform.position, touchedObject.gameObject.transform.rotation) as GameObject;
                        newAttachGameObject.name = SpecialEffectGameObject;
                        newAttachGameObject.transform.parent = touchedObject.gameObject.transform;
                    }
                }
            }

            //Removes the attached gameobject for the effect
            private static void RemoveAttachedGameObject(Rigidbody touchedObject, GameObject attachGameObject)
            {
                if (touchedObject != null && attachGameObject != null)
                {
                    if (touchedObject.transform.Find(SpecialEffectGameObject) != null)
                    {
                        Destroy(touchedObject.transform.Find(SpecialEffectGameObject).gameObject);
                    }
                }
            }

            //Refreshs the EffectedObjects over a timer
            public void RefreshEffectedObjectListOverTime(float timer, GameObject attachedGameObject)
            {
                if (effectedObjectList.Count != 0)
                {
                    List<EffectedObject> removeItems = new List<EffectedObject>();

                    foreach (EffectedObject item in effectedObjectList)
                    {
                        if (item.effectedTime + timer < Time.time)
                        {
                            switch (item.physicsEffect)
                            {
                                case PhysicsEffect.None:
                                    break;
                                case PhysicsEffect.NoGravity:
                                    if (item.touchedObject != null)
                                    {
                                        item.touchedObject.useGravity = true;
                                    }
                                    break;
                            }

                            RemoveAttachedGameObject(item.touchedObject, attachedGameObject);
                            removeItems.Add(item);
                        }
                    }

                    //Clears effected items out of the list
                    foreach (var item in removeItems)
                    {
                        effectedObjectList.Remove(item);
                    }
                }
            }
        }

        //Data structure for effected object
        public class EffectedObject
        {
            public EffectedObject()
            {
            }

            public EffectedObject(float effectedTime, Rigidbody touchedObject, PhysicsEffect physicsEffect)
            {
                this.effectedTime = effectedTime;
                this.touchedObject = touchedObject;
                this.physicsEffect = physicsEffect;
            }

            //Time Effected
            public float effectedTime { get; set; }

            //Rigidbody of touched object
            public Rigidbody touchedObject { get; set; }

            //Type of effect
            public PhysicsEffect physicsEffect { get; set; }
        }

        #endregion

        #region Unity Events

        void Awake()
        {
            #region Default Settings

            //Sets up pulse
            if (pulseProperties.pulse)
            {
                size = pulseProperties.minSize;
                pulse_Positive = true;
            }

            #endregion
        }

        //Use this for initialization
        void Start()
        {
        }

        //Update is called once per frame
        void Update()
        {
            if (enable)
            {
                if (pulseProperties.pulse)
                {
                    CalculatePulse();
                }

                //Sets up the line that gets rendered showing the area of forces
                if (drawGravityProperties.drawGravityForce)
                {
                    if (CirularGravityLine == null)
                    {
                        //Creates line for showing the force
                        CirularGravityLine = new GameObject(CirularGravityLineName);
                        CirularGravityLine.transform.position = this.transform.position;
                        CirularGravityLine.transform.parent = this.gameObject.transform;
                        CirularGravityLine.AddComponent<LineRenderer>();
                    }

                    DrawGravityForceLineRenderer();
                }
                else
                {
                    if (CirularGravityLine != null)
                    {
                        //Destroys line when not using
                        Destroy(CirularGravityLine);
                    }
                }
            }
            else
            {
                if (CirularGravityLine != null)
                {
                    //Destroys line when not using
                    Destroy(CirularGravityLine);
                }
            }
        }

        //This function is called every fixed frame
        void FixedUpdate()
        {
            if (enable && forcePower != 0)
            {
                CalculateAndEstimateForce();
            }

            specialEffect.effectedObjects.RefreshEffectedObjectListOverTime(specialEffect.timeEffected, specialEffect.attachedGameObject);
        }

        #endregion

        #region Functions

        //Calculatie the given pulse
        private void CalculatePulse()
        {
            if (pulseProperties.pulse)
            {
                if (pulse_Positive)
                {
                    if (size <= pulseProperties.maxSize)
                        size = size + (pulseProperties.speed * Time.deltaTime);
                    else
                        pulse_Positive = false;
                }
                else
                {
                    if (size >= pulseProperties.minSize)
                        size = size - (pulseProperties.speed * Time.deltaTime);
                    else
                        pulse_Positive = true;
                }
            }
        }

        //Calculate and Estimate the force
        private void CalculateAndEstimateForce()
        {
            Collider[] colliders = null;
            RaycastHit[] raycastHits = null;

            if (shape == Shape.Sphere)
            {
                #region Sphere

                colliders = Physics.OverlapSphere(this.transform.position, size);

                if (colliders.Length > 0)
                {
                    var hits =
                        from h in colliders
                        select h;

                    hits = hits.Where(h => h.GetComponent<Rigidbody>() != this.GetComponent<Rigidbody>());
                    hits = hits.Where(h => h.GetComponent<Rigidbody>() == true);

                    //Used for Tag filtering
                    switch (tagFilter.tagFilterOptions)
                    {
                        case TagFilter.TagFilterOptions.Disabled:
                            break;
                        case TagFilter.TagFilterOptions.OnlyEffectListedTags:
                            hits = hits.Where(h => tagFilter.tagsList.Contains<string>(h.tag));
                            break;
                        case TagFilter.TagFilterOptions.DontEffectListedTags:
                            hits = hits.Where(h => !tagFilter.tagsList.Contains<string>(h.tag));
                            break;
                    }

                    //Used for Trigger Area Filtering
                    switch (triggerAreaFilter.triggerAreaFilterOptions)
                    {
                        case TriggerAreaFilter.TriggerAreaFilterOptions.Disabled:
                            break;
                        case TriggerAreaFilter.TriggerAreaFilterOptions.OnlyEffectWithinTigger:
                            hits = hits.Where(h => triggerAreaFilter.triggerArea.GetComponent<Collider>().bounds.Contains(h.transform.position));
                            break;
                        case TriggerAreaFilter.TriggerAreaFilterOptions.DontEffectWithinTigger:
                            hits = hits.Where(h => !triggerAreaFilter.triggerArea.GetComponent<Collider>().bounds.Contains(h.transform.position));
                            break;
                        default:
                            break;
                    }

                    foreach (var hit in hits)
                    {
                        //If forcePoint is not supplied and no forcePoints, it will use the this.transform.position
                        if (forcePoint == null && forcePoints.Count == 0)
                        {
                            SetupSpecialEffect(hit.GetComponent<Rigidbody>());
                            hit.GetComponent<Rigidbody>().AddExplosionForce(forcePower, this.transform.position, size);

                            if (constraintProperties.alignToForce)
                            {
                                SetConstraintSettings(hit.transform, this.transform);
                            }
                        }
                        else if (forcePoint != null)
                        {
                            SetupSpecialEffect(hit.GetComponent<Rigidbody>());
                            hit.GetComponent<Rigidbody>().AddExplosionForce(forcePower, forcePoint.transform.position, size);

                            if (constraintProperties.alignToForce)
                            {
                                SetConstraintSettings(hit.transform, forcePoint.transform);
                            }
                        }

                        //Adds force to all the GameObjects listed in the forcePoints array
                        if (forcePoints.Count > 0)
                        {
                            foreach (var forcepoint in forcePoints)
                            {
                                GameObject updateForcepoint = forcepoint as GameObject;

                                SetupSpecialEffect(hit.GetComponent<Rigidbody>());
                                hit.GetComponent<Rigidbody>().AddExplosionForce(forcePower, updateForcepoint.transform.position, size);

                                if (constraintProperties.alignToForce)
                                {
                                    SetConstraintSettings(hit.transform, updateForcepoint.transform);
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region RayCast

                //Circular Gravity Force Transform
                Transform cgfTran = this.transform;

                if (shape == Shape.RayCast)
                {
                    raycastHits = Physics.RaycastAll(cgfTran.position, cgfTran.rotation * Vector3.forward, size);
                }
                else if (shape == Shape.Capsule)
                {
                    raycastHits = Physics.CapsuleCastAll
                    (
                        cgfTran.position,
                        cgfTran.position + ((cgfTran.rotation * Vector3.back)),
                        capsuleRadius,
                        cgfTran.position - ((cgfTran.position + (cgfTran.rotation * (Vector3.back)))),
                        size
                    );
                }

                if (raycastHits.Length > 0)
                {
                    var rayhits =
                    from h2 in raycastHits
                    select h2;

                    rayhits = rayhits.Where(h2 => h2.rigidbody != this.GetComponent<Rigidbody>());
                    rayhits = rayhits.Where(h2 => h2.rigidbody == true);

                    //Used for Tag filtering
                    switch (tagFilter.tagFilterOptions)
                    {
                        case TagFilter.TagFilterOptions.Disabled:
                            break;
                        case TagFilter.TagFilterOptions.OnlyEffectListedTags:
                            rayhits = rayhits.Where(h2 => tagFilter.tagsList.Contains<string>(h2.transform.gameObject.tag));
                            break;
                        case TagFilter.TagFilterOptions.DontEffectListedTags:
                            rayhits = rayhits.Where(h2 => !tagFilter.tagsList.Contains<string>(h2.transform.gameObject.tag));
                            break;
                    }

                    //Used for Trigger Area Filtering
                    switch (triggerAreaFilter.triggerAreaFilterOptions)
                    {
                        case TriggerAreaFilter.TriggerAreaFilterOptions.Disabled:
                            break;
                        case TriggerAreaFilter.TriggerAreaFilterOptions.OnlyEffectWithinTigger:
                            rayhits = rayhits.Where(h => triggerAreaFilter.triggerArea.GetComponent<Collider>().bounds.Contains(h.transform.position));
                            break;
                        case TriggerAreaFilter.TriggerAreaFilterOptions.DontEffectWithinTigger:
                            rayhits = rayhits.Where(h2 => !triggerAreaFilter.triggerArea.GetComponent<Collider>().bounds.Contains(h2.transform.position));
                            break;
                        default:
                            break;
                    }

                    foreach (var hit in rayhits)
                    {
                        //If forcePoint is not supplied and no forcePoints, it will use the this.transform.position
                        if (forcePoint == null && forcePoints.Count == 0)
                        {
                            SetupSpecialEffect(hit.rigidbody);
                            hit.rigidbody.AddExplosionForce(forcePower, cgfTran.position, size);

                            if (constraintProperties.alignToForce)
                            {
                                SetConstraintSettings(hit.transform, cgfTran.transform);
                            }
                        }
                        else if (forcePoint != null)
                        {
                            SetupSpecialEffect(hit.rigidbody);
                            hit.rigidbody.AddExplosionForce(forcePower, forcePoint.transform.position, size);

                            if (constraintProperties.alignToForce)
                            {
                                SetConstraintSettings(hit.transform, forcePoint.transform);
                            }
                        }

                        //Adds force to all the GameObjects listed in the forcePoints array
                        if (forcePoints.Count > 0)
                        {
                            foreach (var forcepoint in forcePoints)
                            {
                                GameObject updateForcepoint = forcepoint as GameObject;

                                SetupSpecialEffect(hit.rigidbody);
                                hit.rigidbody.AddExplosionForce(forcePower, updateForcepoint.transform.position, size);

                                if (constraintProperties.alignToForce)
                                {
                                    SetConstraintSettings(hit.transform, updateForcepoint.transform);
                                }
                            }
                        }
                    }
                }

                #endregion
            }
        }

        private void SetupSpecialEffect(Rigidbody rigidbody)
        {
            switch (specialEffect.physicsEffect)
            {
                case PhysicsEffect.None:
                    break;
                case PhysicsEffect.NoGravity:
                    rigidbody.useGravity = false;
                    break;
                default:
                    break;
            }

            if (specialEffect.timeEffected > 0)
            {
                specialEffect.effectedObjects.AddedEffectedObject(rigidbody, specialEffect.physicsEffect, specialEffect.attachedGameObject);
            }
        }

        private void SetConstraintSettings(Transform hitTran, Transform tran)
        {
            Vector3 direction = (hitTran.position - tran.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            hitTran.transform.rotation = lookRotation;
        }

        #endregion

        #region Draw

        //Draws effected area by forces line renderer
        private void DrawGravityForceLineRenderer()
        {
            //Circular Gravity Force Transform
            Transform cgfTran = this.transform;

            Color DebugGravityLineColor;

            if (forcePower == 0)
                DebugGravityLineColor = Color.white;
            else if (forcePower > 0)
                DebugGravityLineColor = Color.green;
            else
                DebugGravityLineColor = Color.red;

            //Line setup
            LineRenderer lineRenderer = CirularGravityLine.GetComponent<LineRenderer>();
            lineRenderer.SetWidth(drawGravityProperties.thickness, drawGravityProperties.thickness);
            lineRenderer.material = drawGravityProperties.gravityLineMaterial;
            lineRenderer.SetColors(DebugGravityLineColor, DebugGravityLineColor);

            //Renders type outline
            switch (shape)
            {
                case Shape.Sphere:

                    //Models line
                    lineRenderer.SetVertexCount(12);

                    lineRenderer.SetPosition(0, cgfTran.position + ((cgfTran.rotation * Vector3.up) * size));
                    lineRenderer.SetPosition(1, cgfTran.position);
                    lineRenderer.SetPosition(2, cgfTran.position + ((cgfTran.rotation * Vector3.down) * size));
                    lineRenderer.SetPosition(3, cgfTran.position);
                    lineRenderer.SetPosition(4, cgfTran.position + ((cgfTran.rotation * Vector3.left) * size));
                    lineRenderer.SetPosition(5, cgfTran.position);
                    lineRenderer.SetPosition(6, cgfTran.position + ((cgfTran.rotation * Vector3.right) * size));
                    lineRenderer.SetPosition(7, cgfTran.position);
                    lineRenderer.SetPosition(8, cgfTran.position + ((cgfTran.rotation * Vector3.forward) * size));
                    lineRenderer.SetPosition(9, cgfTran.position);
                    lineRenderer.SetPosition(10, cgfTran.position + ((cgfTran.rotation * Vector3.back) * size));
                    lineRenderer.SetPosition(11, cgfTran.position);

                    break;

                case Shape.Capsule:

                    //Model Capsule
                    lineRenderer.SetVertexCount(17);

                    //Starting Point
                    lineRenderer.SetPosition(0, cgfTran.position + ((cgfTran.rotation * Vector3.up) * capsuleRadius));
                    lineRenderer.SetPosition(1, cgfTran.position);
                    lineRenderer.SetPosition(2, cgfTran.position + ((cgfTran.rotation * Vector3.down) * capsuleRadius));
                    lineRenderer.SetPosition(3, cgfTran.position);
                    lineRenderer.SetPosition(4, cgfTran.position + ((cgfTran.rotation * Vector3.left) * capsuleRadius));
                    lineRenderer.SetPosition(5, cgfTran.position);
                    lineRenderer.SetPosition(6, cgfTran.position + ((cgfTran.rotation * Vector3.right) * capsuleRadius));
                    lineRenderer.SetPosition(7, cgfTran.position);

                    //Middle Line
                    Vector3 endPointLoc = cgfTran.position + ((cgfTran.rotation * Vector3.forward) * size);
                    lineRenderer.SetPosition(8, endPointLoc);

                    //End Point
                    lineRenderer.SetPosition(9, endPointLoc + ((cgfTran.rotation * Vector3.up) * capsuleRadius));
                    lineRenderer.SetPosition(10, endPointLoc);
                    lineRenderer.SetPosition(11, endPointLoc + ((cgfTran.rotation * Vector3.down) * capsuleRadius));
                    lineRenderer.SetPosition(12, endPointLoc);
                    lineRenderer.SetPosition(13, endPointLoc + ((cgfTran.rotation * Vector3.left) * capsuleRadius));
                    lineRenderer.SetPosition(14, endPointLoc);
                    lineRenderer.SetPosition(15, endPointLoc + ((cgfTran.rotation * Vector3.right) * capsuleRadius));
                    lineRenderer.SetPosition(16, endPointLoc);

                    break;

                case Shape.RayCast:

                    //Model Line
                    lineRenderer.SetVertexCount(2);

                    lineRenderer.SetPosition(0, cgfTran.position);
                    lineRenderer.SetPosition(1, cgfTran.position + ((cgfTran.rotation * Vector3.forward) * size));

                    break;
            }
        }

        #endregion
    }
}