using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

namespace Xamin
{
    [RequireComponent(typeof(Image))]
    public class Button : MonoBehaviour
    {
        public UnityEvent action;
        public Sprite image;
        public bool unlocked;
        public string id;
        void Start()
        {
            if (image)
                GetComponent<UnityEngine.UI.Image>().sprite = image;
        }
        public void ExecuteAction()
        {
            action.Invoke();
        }
    }
}