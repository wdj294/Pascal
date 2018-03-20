#region Namespace Imports
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#endregion

namespace UI.ThreeDimensional
{
    [RequireComponent(typeof(UIObject3D))]
    [AddComponentMenu("UI/UIObject3D/Drag Rotate UIObject3D")]
    public class DragRotateUIObject3D : MonoBehaviour
    {
        [Header("X")]
        public bool RotateX = true;
        public bool InvertX = false;
        private int _xMultiplier
        {
            get { return InvertX ? -1 : 1; }
        }

        [Header("Y")]
        public bool RotateY = true;
        public bool InvertY = false;
        private int _yMultiplier
        {
            get { return InvertY ? -1 : 1; }
        }

        [Header("Sensitivity")]
        public float Sensitivity = 0.4f;

        private UIObject3D UIObject3D;        

        void Awake()
        {
            UIObject3D = this.GetComponent<UIObject3D>();            

            SetupEvents();
        }                

        void SetupEvents()
        {
            // get or add the event trigger
            EventTrigger trigger = this.GetComponent<EventTrigger>() ?? this.gameObject.AddComponent<EventTrigger>();

            var onDrag = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
            onDrag.callback.AddListener((e) => OnDrag(e as PointerEventData));
            trigger.triggers.Add(onDrag);            
        }

        void OnDrag(PointerEventData e)
        {
            if (RotateX)
            {
                var x = e.delta.x * Sensitivity * _xMultiplier * -1;
                var xRotation = Quaternion.AngleAxis(x, Vector3.up);
                UIObject3D.TargetRotation = (xRotation * Quaternion.Euler(UIObject3D.TargetRotation)).eulerAngles;
            }

            if (RotateY)
            {
                var y = e.delta.y * Sensitivity * _yMultiplier;
                var yRotation = Quaternion.AngleAxis(y, Vector3.right);
                UIObject3D.TargetRotation = (yRotation * Quaternion.Euler(UIObject3D.TargetRotation)).eulerAngles;
            }
        }
    }
}
