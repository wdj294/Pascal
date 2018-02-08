using System;
using UnityEngine;

namespace Ez.Examples
{
    public class TransformProperties : MonoBehaviour
    {
        [Range(0, 4)]
        public int decimals = 1;

        public Vector3 TransformPosition, TransformRotation, TransformScale;

        public float posX { get { return (float)Math.Round(transform.position.x, decimals); } set { } }
        public float posY { get { return (float)Math.Round(transform.position.y, decimals); } set { } }
        public float posZ { get { return (float)Math.Round(transform.position.z, decimals); } set { } }

        public float rotX { get { return (float)Math.Round(transform.rotation.eulerAngles.x, decimals); } set { } }
        public float rotY { get { return (float)Math.Round(transform.rotation.eulerAngles.y, decimals); } set { } }
        public float rotZ { get { return (float)Math.Round(transform.rotation.eulerAngles.z, decimals); } set { } }

        public float scaX { get { return (float)Math.Round(transform.localScale.x, decimals); } set { } }
        public float scaY { get { return (float)Math.Round(transform.localScale.y, decimals); } set { } }
        public float scaZ { get { return (float)Math.Round(transform.localScale.z, decimals); } set { } }

        public string position { get { return "(" + posX + ", " + posY + ", " + posZ + ")"; } set { } }
        public string rotation { get { return "(" + rotX + ", " + rotY + ", " + rotZ + ")"; } set { } }
        public string scale { get { return "(" + scaX + ", " + scaY + ", " + scaZ + ")"; } set { } }

        private void Update()
        {
            TransformPosition = new Vector3(posX, posY, posZ);
            TransformRotation = new Vector3(rotX, rotY, rotZ);
            TransformScale = new Vector3(scaX, scaY, scaZ);
        }
    }
}
