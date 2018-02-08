using UnityEngine;

namespace Ez.Examples
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteChanger : MonoBehaviour
    {
        public bool Enabled = true;
        public Sprite[] sprites;
        public float delay = 2f;

        private bool IsEnabled { get { return Enabled && sprites != null && sprites.Length > 0; } }

        private float timeForNextSpriteChange = -1;
        private int currentSpriteIndex = -1;
        private SpriteRenderer sr;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if(!IsEnabled) { timeForNextSpriteChange = -1; return; }
            if(timeForNextSpriteChange == -1) { timeForNextSpriteChange = Time.realtimeSinceStartup + delay; }
            if(timeForNextSpriteChange < Time.realtimeSinceStartup)
            {
                if(currentSpriteIndex == -1 || currentSpriteIndex == sprites.Length) { currentSpriteIndex = 0; }
                sr.sprite = sprites[currentSpriteIndex];
                currentSpriteIndex++;
                timeForNextSpriteChange = -1;
            }
        }
    }
}

