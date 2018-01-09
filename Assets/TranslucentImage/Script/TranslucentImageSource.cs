using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace LeTai.Asset.TranslucentImage
{
    /// <summary>
    /// Common source of blur for Translucent Images. 
    /// </summary>
    /// <remarks>
    /// It is an Image effect that blur the render target of the Camera it attached to, then save the result to a global read-only  Render Texture
    /// </remarks>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Tai Le Assets/Translucent Image Source")]
    public class TranslucentImageSource : MonoBehaviour
    {
        #region Public field

        /// <summary>
        /// Maximum number of times to update the blurred image each second
        /// </summary>
        [Tooltip(
            "Maximum number of times that the blur algorithm can run per second." +
            " Lower if you need more performance")]
        public float maxUpdateRate = float.PositiveInfinity;

        /// <summary>
        /// Render the blurred result to the render target
        /// </summary>
        [Tooltip("Preview the effect on entire screen")]
        public bool preview;

        #endregion

        #region Field

        [SerializeField]
        float size = 5;

        [SerializeField]
        int iteration = 4;

        [SerializeField]
        int maxDepth = 4;

        [SerializeField]
        int downsample,
            lastDownsample;

        [SerializeField]
        float strength;

        float lastUpdate;
        //Disable non-sense warning from Unity
#pragma warning disable 0108
        Camera camera;
#pragma warning restore 0108
        Shader shader;
        Material material;

        #endregion

        #region Properties

        /// <summary>
        /// Result of the image effect. Translucent Image use this as their content (read-only)
        /// </summary>
        public RenderTexture BlurredScreen { get; private set; }

        /// <summary>
        /// The Camera attached to the same GameObject. Cached in field 'camera'
        /// </summary>
        public Camera Cam
        {
            get { return camera ?? (camera = GetComponent<Camera>()); }
        }

        /// <summary>
        /// User friendly property to control the amount of blur
        /// </summary>
        ///<value>
        /// Must be non-negative
        /// </value>
        public float Strength
        {
            get { return strength = Size * Mathf.Pow(2, Iteration + Downsample); }
            set
            {
                strength = Mathf.Max(0, value);
                SetAdvancedFieldFromSimple();
            }
        }

        /// <summary>
        /// Distance between the base texel and the texel to be sampled.
        /// </summary>
        public float Size
        {
            get { return size; }
            set { size = Mathf.Max(0, value); }
        }

        /// <summary>
        /// Half the number of time to process the image. It is half because the real number of iteration must alway be even. Using half also make calculation simpler
        /// </summary>
        /// <value>
        /// Must be non-negative
        /// </value>
        public int Iteration
        {
            get { return iteration; }
            set { iteration = Mathf.Max(0, value); }
        }

        /// <summary>
        /// The rendered image will be shrinked by a factor of 2^{{this}} before bluring to reduce processing time
        /// </summary>
        /// <value>
        /// Must be non-negative. Default to 0
        /// </value>
        public int Downsample
        {
            get { return downsample; }
            set { downsample = Mathf.Max(0, value); }
        }

        /// <summary>
        /// Clamp the minimum size of the intermediate texture. Reduce flickering and blur
        /// </summary>
        /// <value>
        /// Must larger than 0
        /// </value>
        public int MaxDepth
        {
            get { return maxDepth; }
            set { maxDepth = Mathf.Max(1, value); }
        }

        /// <summary>
        /// A small number base on the smaller dimension of the camera render target. 
        /// Used to retain the blur amount across screen size
        /// </summary>
        float ScreenSize
        {
            //1080f is an arbitrary number. It keep 'size' in a reasonable range
            get { return Mathf.Min(Cam.pixelWidth, Cam.pixelHeight) / 1080f; }
        }

        /// <summary>
        /// Minimum time in second to wait before refresh the blurred image. 
        /// If maxUpdateRate non-positive then just stop updating
        /// </summary>
        float MinUpdateCycle
        {
            get { return (maxUpdateRate > 0) ? (1f / maxUpdateRate) : float.PositiveInfinity; }
        }

        #endregion

        /// <summary>
        /// Calculate size and iteration from strength
        /// </summary>
        protected virtual void SetAdvancedFieldFromSimple()
        {
            Size = strength / Mathf.Pow(2, Iteration + Downsample);
            //Does not handle negative size
            while (Size < 1)
            {
                if (Downsample > 0)
                {
                    Downsample--;
                    Size *= 2;
                } else if (Iteration > 0)
                {
                    Iteration--;
                    Size *= 2;
                }
                break;
            }
            while (Size > 8)
            {
                Size /= 2;
                Iteration++;
            }
        }


#if UNITY_EDITOR

        [InitializeOnLoadMethod]
#endif
        protected virtual void Start()
        {
            camera = Cam;
            shader = Shader.Find("Hidden/EfficientBlur");
            if (!shader.isSupported) enabled = false;

            material = new Material(shader);

            BlurredScreen = new RenderTexture(
                                    Cam.pixelWidth >> Downsample,
                                    Cam.pixelHeight >> Downsample,
                                    0)
                                {
                                    filterMode = FilterMode.Bilinear
                                };
            lastDownsample = Downsample;
        }

        /// <summary>
        /// Resize the source texture then run it through a shader before assign to target texure
        /// </summary>
        /// <param name="source"></param>
        /// <param name="level">Resampling depth</param>
        /// <param name="target"></param>
        protected virtual void ProgressiveResampling(RenderTexture source, int level, ref RenderTexture target)
        {
            level = Mathf.Min(level, MaxDepth);
            int rtW = source.width >> level + Downsample;
            int rtH = source.height >> level + Downsample;

            RenderTexture rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
            rt2.filterMode = FilterMode.Bilinear;

            Graphics.Blit(target, rt2, material, 0);
            RenderTexture.ReleaseTemporary(target);
            target = rt2;
        }

        protected virtual void ProgressiveBlur(RenderTexture source)
        {
            //Resize global texture if base downsample changed
            if (Downsample != lastDownsample)
            {
                BlurredScreen = new RenderTexture(
                    Cam.pixelWidth >> Downsample,
                    Cam.pixelHeight >> Downsample,
                    0);

                lastDownsample = Downsample;
            }
            if (BlurredScreen.IsCreated()) BlurredScreen.DiscardContents();

            //Relative blur size to maintain same look across multiple resolution
            material.SetFloat("size", Size * ScreenSize);

            int firstDownsampleFactor = iteration > 0 ? 1 : 0 + Downsample;

            //= width / (downsample + 1)^2
            int rtW = source.width >> firstDownsampleFactor;
            int rtH = source.height >> firstDownsampleFactor;

            RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
            //Bilinear filtering mode increase blurriness when scaled
            rt.filterMode = FilterMode.Bilinear;
            source.filterMode = FilterMode.Bilinear;

            //Initial downsample
            Graphics.Blit(source, rt, material, 0);

            //Downsample. (iteration - 1) pass 
            for (int i = 2; i < Iteration + 1; i++)
            {
                ProgressiveResampling(source, i, ref rt);
            }

            //Upsample. (iteration - 1) pass 
            for (int i = Iteration - 1; i > 0; i--)
            {
                ProgressiveResampling(source, i, ref rt);
            }

            //Final upsample. Blit to blurredRt
            Graphics.Blit(rt, BlurredScreen, material, 0);

            RenderTexture.ReleaseTemporary(rt);
        }

        protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            float now = Time.unscaledTime;
#if UNITY_EDITOR
            now = (float) EditorApplication.timeSinceStartup;
#endif
            if (now - lastUpdate >= MinUpdateCycle)
            {
                ProgressiveBlur(source);

                lastUpdate = Time.unscaledTime;
#if UNITY_EDITOR
                lastUpdate = (float) EditorApplication.timeSinceStartup;
#endif
            }

            #region Preview

            if (preview)
            {
                Graphics.Blit(BlurredScreen, destination);
                return;
            }
            //Draw the screen unmodified
            Graphics.Blit(source, destination);

            #endregion
        }
    }
}