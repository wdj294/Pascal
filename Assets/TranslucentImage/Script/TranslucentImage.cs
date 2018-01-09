using UnityEngine;
using UnityEngine.UI;

namespace LeTai.Asset.TranslucentImage
{
    /// <summary>
    /// Dynamic blur-behind UI element
    /// </summary>
    public partial class TranslucentImage : Image, IMeshModifier
    {
        /// <summary>
        /// Source of blur for this image
        /// </summary>
        public TranslucentImageSource source;

        /// <summary>
        /// (De)Saturate them image, 1 is normal, 0 is grey scale, below zero make the image negative
        /// </summary>
        [Tooltip("(De)Saturate them image, 1 is normal, 0 is black and white, below zero make the image negative")]
        [Range(-1, 3)]
        public float vibrancy = 1;

        /// <summary>
        /// Brighten/darken them image
        /// </summary>
        [Tooltip("Brighten/darken them image")]
        [Range(-1, 1)]
        public float brightness = 0;

        /// <summary>
        /// Flatten the color behind to help keep contrast on varying background
        /// </summary>
        [Tooltip("Flatten the color behind to help keep contrast on varying background")]
        [Range(0, 1)]
        public float flatten = .1f;


        Shader correctShader;
        int vibrancyPropId;
        int brightnessPropId;
        int flattenPropId;


        protected override void Start()
        {
            base.Start();

            PrepShader();

            oldVibrancy = vibrancy;
            oldBrightness = brightness;
            oldFlatten = flatten;

            source = source ?? FindObjectOfType<TranslucentImageSource>();
            material.SetTexture("_BlurTex", source.BlurredScreen);
        }

        void PrepShader()
        {
            correctShader = Shader.Find("UI/TranslucentImage");
            vibrancyPropId = Shader.PropertyToID("_Vibrancy");
            brightnessPropId = Shader.PropertyToID("_Brightness");
            flattenPropId = Shader.PropertyToID("_Flatten");
        }

        protected void LateUpdate()
        {
            if (!source)
            {
                Debug.LogError(
                    "Source missing. Add TranslucentImageSource component to your main camera, then drag the camera to Source slot");
                return;
            }
            if (!IsActive() || !source.BlurredScreen)
                return;

            if (!material || material.shader != correctShader)
            {
                Debug.LogError("Material using \"UI/TranslucentImage\" is required");
            }
            materialForRendering.SetTexture("_BlurTex", source.BlurredScreen);
#if UNITY_EDITOR
            material.SetTexture("_BlurTex", source.BlurredScreen);
#endif
        }

        void Update()
        {
            if (vibrancyPropId == 0 || brightnessPropId == 0 || flattenPropId == 0)
                return;

            SyncMaterialProperty(vibrancyPropId, ref vibrancy, ref oldVibrancy);
            SyncMaterialProperty(brightnessPropId, ref brightness, ref oldBrightness);
            SyncMaterialProperty(flattenPropId, ref flatten, ref oldFlatten);
        }

        float oldVibrancy, oldBrightness, oldFlatten;

        void SyncMaterialProperty(int propId, ref float value, ref float oldValue)
        {
            float matProp = materialForRendering.GetFloat(propId);
            if (!Mathf.Approximately(matProp, value))
            {
                if (!Mathf.Approximately(value, oldValue))
                {
                    material.SetFloat(propId, value);
                    materialForRendering.SetFloat(propId, value);
                    SetMaterialDirty();
                } else
                {
                    value = matProp;
                }
            }
            oldValue = value;
        }
    }
}