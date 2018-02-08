using System.Collections;
using UnityEngine;

namespace Ez.Examples
{
    public class CycleScale : MonoBehaviour
    {
        public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f);
        public Vector3 maxScale = new Vector3(1.5f, 1.5f, 1.5f);
        [Space(10)]
        public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        public bool AutoStart = true;
        public float StartDelay = 2;
        public float Time = 1;

        public float currentScaleX { get { return transform.localScale.x; } set { } }
        public float currentScaleY { get { return transform.localScale.y; } set { } }
        public float currentScaleZ { get { return transform.localScale.z; } set { } }

        private Vector3 startScale = Vector3.one;
        private Coroutine cCycle = null;

        private enum ScaleDirection { MinScale, MaxScale, StartScale }
        private ScaleDirection scaleTo = ScaleDirection.MinScale;
        private ScaleDirection scaleFrom = ScaleDirection.StartScale;

        private void Start()
        {
            startScale = transform.localScale;
            if(AutoStart) { StartCycle(StartDelay, Time); }
        }

        public void ScaleToMinScale() { StopCycle(); StartCoroutine(iScale(transform, minScale, Time)); scaleFrom = ScaleDirection.MinScale; }
        public void ScaleToMinScale(float time) { StartCoroutine(iScale(transform, minScale, time)); scaleFrom = ScaleDirection.MinScale; }
        public void ScaleToMaxScale() { StopCycle(); StartCoroutine(iScale(transform, maxScale, Time)); scaleFrom = ScaleDirection.MaxScale; }
        public void ScaleToMaxScale(float time) { StartCoroutine(iScale(transform, maxScale, time)); scaleFrom = ScaleDirection.MaxScale; }
        public void ScaleToStartScale() { StopCycle(); StartCoroutine(iScale(transform, startScale, Time)); scaleFrom = ScaleDirection.StartScale; }
        public void ScaleToStartScale(float time) { StartCoroutine(iScale(transform, startScale, time)); scaleFrom = ScaleDirection.StartScale; }

        IEnumerator iScale(Transform transform, Vector3 toScale, float time)
        {
            var currentScale = transform.localScale;
            var t = 0f;
            while(t < 1)
            {
                t += UnityEngine.Time.deltaTime / time * Curve.Evaluate(time);
                transform.localScale = Vector3.Lerp(currentScale, toScale, t);
                yield return null;
            }
        }


        public void StartCycle()
        {
            if(cCycle != null) { StopCoroutine(cCycle); cCycle = null; }
            cCycle = StartCoroutine(iCycle(0, Time));
        }

        public void StartCycle(float startDelay, float time)
        {
            if(cCycle != null) { StopCoroutine(cCycle); cCycle = null; }
            cCycle = StartCoroutine(iCycle(StartDelay, time));
        }

        public void StopCycle()
        {
            if(cCycle == null) { return; }
            StopCoroutine(cCycle);
            cCycle = null;
        }

        IEnumerator iCycle(float startDelay, float time)
        {
            yield return new WaitForSeconds(startDelay);
            scaleTo = scaleTo == ScaleDirection.MinScale ? ScaleDirection.MaxScale : ScaleDirection.MinScale;

            while(true)
            {
                switch(scaleTo)
                {
                    case ScaleDirection.MinScale:
                        ScaleToMinScale(scaleFrom == ScaleDirection.StartScale ? Time / 2f : Time);
                        scaleFrom = ScaleDirection.MinScale;
                        scaleTo = ScaleDirection.MaxScale;
                        break;
                    case ScaleDirection.MaxScale:
                        ScaleToMaxScale(scaleFrom == ScaleDirection.StartScale ? Time / 2f : Time);
                        scaleFrom = ScaleDirection.MaxScale;
                        scaleTo = ScaleDirection.MinScale;
                        break;
                    case ScaleDirection.StartScale:
                        ScaleToStartScale(Time / 2f);
                        scaleFrom = ScaleDirection.StartScale;
                        scaleTo = ScaleDirection.MinScale;
                        break;
                    default:
                        break;
                }
                yield return new WaitForSeconds(time);
            }
        }
    }
}
