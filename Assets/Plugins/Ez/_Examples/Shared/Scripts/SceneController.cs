using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ez.Examples
{
    public class SceneController : MonoBehaviour
    {
        public GameObject rootCanvas = null;
        public float transitionSpeed = 1f;
        public List<Zone> zones = new List<Zone>();
        [HideInInspector]
        public int currentZoneIndex = 0;

        private bool start = true;

        private void Start()
        {
            start = true;
            currentZoneIndex = 0;
            NextZone(true);
        }

        private void Update()
        {
            KeyInput();
        }

        protected virtual void KeyInput()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { PreviousZone(); }
            if (Input.GetKeyDown(KeyCode.RightArrow)) { NextZone(); }
            if (Input.GetKeyDown(KeyCode.DownArrow)) { rootCanvas.SetActive(!rootCanvas.activeInHierarchy); }
        }

        public virtual void NextZone(bool instant = false)
        {
            if (zones == null || zones.Count == 0) { return; }
            if ((currentZoneIndex >= 0 || currentZoneIndex < zones.Count) && !start) { zones[currentZoneIndex].OnHide.Invoke(); }
            if (currentZoneIndex == zones.Count - 1) { currentZoneIndex = 0; }
            else { currentZoneIndex++; }
            if (start) { currentZoneIndex--; start = false; }
            zones[currentZoneIndex].OnShow.Invoke();
            ChangeZone(instant);
        }

        public virtual void PreviousZone(bool instant = false)
        {
            if (zones == null || zones.Count == 0) { return; }
            if (currentZoneIndex >= 0 || currentZoneIndex < zones.Count) { zones[currentZoneIndex].OnHide.Invoke(); }
            if (currentZoneIndex == 0) { currentZoneIndex = zones.Count - 1; }
            else { currentZoneIndex--; }
            zones[currentZoneIndex].OnShow.Invoke();
            ChangeZone(instant);
        }

        protected virtual void ChangeZone(bool instant = false)
        {
            if (zones == null || zones.Count == 0) { return; }
            for (int i = 0; i < zones.Count; i++)
            {
                if (zones[i].canvas == null)
                {
                    //Debug.Log("No canvas has been set for zone " + i + ".");
                    continue;
                }
                zones[i].canvas.SetActive(currentZoneIndex == i ? true : false);
            }
            StopAllCoroutines();
            if (instant)
            {
                Camera.main.transform.position = zones[currentZoneIndex].cameraPosition;
                Camera.main.transform.rotation = Quaternion.Euler(zones[currentZoneIndex].cameraRotation);
                return;
            }
            StartCoroutine(iMoveToPosition(Camera.main.transform, zones[currentZoneIndex].cameraPosition, transitionSpeed));
            StartCoroutine(iRotateToRotation(Camera.main.transform, zones[currentZoneIndex].cameraRotation, transitionSpeed));
        }

        IEnumerator iMoveToPosition(Transform target, Vector3 position, float duration)
        {
            var startPosition = target.position;
            var t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / duration;
                target.position = Vector3.Lerp(startPosition, position, t);
                yield return null;
            }
        }

        IEnumerator iRotateToRotation(Transform target, Vector3 rotation, float duration)
        {
            var startRotation = target.rotation.eulerAngles;
            var t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / duration;
                target.rotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(rotation), t);
                yield return null;
            }
        }
    }

    [Serializable]
    public class Zone
    {
        public string name;
        public GameObject canvas;
        public GameObject container;
        public Vector3 cameraPosition;
        public Vector3 cameraRotation;
        public UnityEvent OnShow;
        public UnityEvent OnHide;

        public Zone()
        {
            name = "Unnamed Zone";
            canvas = null;
            cameraPosition = Vector3.zero;
            cameraRotation = Vector3.zero;
            OnShow = new UnityEvent();
            OnHide = new UnityEvent();
        }
    }

}
