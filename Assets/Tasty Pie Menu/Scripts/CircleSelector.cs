using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Xamin
{
    [RequireComponent(typeof(AudioSource))]
    public class CircleSelector : MonoBehaviour
    {
        [Range(2, 10)]
        private int buttonCount;
        private int startButCount;

        [Header("Customization")]
        public Color accentColor;
        public Color disabledColor, backgroundColor;
        [Space(10)]
        public bool useSeparators;
        [SerializeField]
        private GameObject separatorPrefab;

        [Header("Animations")]
        [Range(0.0001f, 1)]
        public float lerpAmount = .145f;
        public AnimationType openAnimation, closeAnimation;
        public float size;
        private Image cursor, background;
        private float desiredFill;
        float radius = 120f;

        [Header("Sound")]
        public AudioClip segmentChangedSound;
        public AudioClip segmentClickedSound;

        [Header("Interaction")]
        public List<GameObject> buttons = new List<GameObject>();
        public ButtonSource buttonSource;
        private List<Xamin.Button> buttonsInstances = new List<Xamin.Button>();
        private List<float> allRotations = new List<float>();

        private GameObject _selectedSegment;
        private float audioCoolDown;
        [HideInInspector]
        public GameObject SelectedSegment
        {
            get
            {
                return _selectedSegment;
            }
            set
            {
                if (value == null) return;
                Debug.Log(value.name);
                if (value == SelectedSegment) return;
                _selectedSegment = value;
                if (segmentChangedSound == null || !(audioCoolDown <= 0)) return;
                localAudioSource.PlayOneShot(segmentChangedSound);
                audioCoolDown = .05f;
            }
        }

        public bool snap, tiltTowardsMouse;
        public float tiltAmount = 15;
        private bool opened;
        public enum ControlType { mouseAndTouch, gamepad }
        /// <summary>
        /// Button source
        /// <para>use prefabs in a menu where you want to add or remove elements at runtime</para>
        /// <para>use scene if you want a static menu that you can only modify on the editor</para>
        /// </summary>
        public enum ButtonSource { prefabs, scene }
        [Header("Controls")]
        public ControlType controlType;
        public string gamepadAxisX, gamepadAxisY;

        

        public enum AnimationType { zoomIn, zoomOut }

        private AudioSource localAudioSource;

        void Start()
        {
            transform.localScale = Vector3.zero;
            Assert.IsNotNull(transform.Find("Cursor"));
            cursor = transform.Find("Cursor").GetComponent<Image>();
            Assert.IsNotNull(transform.Find("Background"));
            background = transform.Find("Background").GetComponent<Image>();
            buttonCount = buttons.Count;
            allRotations.Clear();
            foreach (Xamin.Button b in buttonsInstances)
                Destroy(b.gameObject);
            buttonsInstances.Clear();
            if (buttonCount > 0 && buttonCount < 11)
            {
                #region Arrange Buttons
                startButCount = buttonCount;
                desiredFill = 1f / (float)buttonCount;
                float fillRadius = desiredFill * 360f;
                float previousRotation = 0;
                foreach (Transform sep in transform.Find("Separators"))
                    Destroy(sep.gameObject);
                for (int i = 0; i < buttonCount; i++)
                {
                    //TIP   y=sin(angle)
                    //      x=cos(angle)
                    GameObject b;
                    if (buttonSource == ButtonSource.prefabs)
                        b = Instantiate(buttons[i], Vector2.zero, transform.rotation) as GameObject;
                    else
                        b = buttons[i];
                    b.transform.SetParent(transform.Find("Buttons"));
                    float bRot = previousRotation + fillRadius / 2;
                    previousRotation = bRot + fillRadius / 2;
                    GameObject separator = Instantiate(separatorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    separator.transform.parent = transform.Find("Separators");
                    separator.transform.localScale = Vector3.one;
                    separator.transform.localPosition = Vector3.zero;
                    separator.transform.localRotation = Quaternion.Euler(0, 0, previousRotation);

                    b.transform.localPosition = new Vector2(radius * Mathf.Cos((bRot - 90) * Mathf.Deg2Rad), -radius * Mathf.Sin((bRot - 90) * Mathf.Deg2Rad));
                    b.transform.localScale = Vector3.one;
                    if (bRot > 360)
                        bRot -= 360;
                    b.name = bRot.ToString();
                    allRotations.Add(bRot);
                    if (buttonSource == ButtonSource.prefabs)
                        buttonsInstances.Add(b.GetComponent<Button>());
                }
                #endregion
            }
            localAudioSource = GetComponent<AudioSource>();
        }

        public void Open()
        {
            opened = true;
            transform.localScale = (openAnimation == AnimationType.zoomIn) ? Vector3.zero : Vector3.one * 10;
        }

        public void Close()
        {
            opened = false;
        }

        public Xamin.Button GetButtonWithId(string id)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Xamin.Button b = (buttonSource == ButtonSource.prefabs) ? buttonsInstances[i] : buttons[i].GetComponent<Xamin.Button>();
                if (b.id == id)
                    return b;
            }
            return null;
        }

        void Update()
        {
            if (audioCoolDown > 0)
                audioCoolDown -= Time.deltaTime;
            if (opened)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(size, size, size), .2f);
                background.color = backgroundColor;
                if (transform.Find("Separators"))
                {
                    transform.Find("Separators").gameObject.SetActive(useSeparators);
                }
                if (transform.localScale.x >= size - .2f)
                {
                    #region Check if should re-arrange
                    buttonCount = buttons.Count;
                    if (startButCount != buttonCount && buttonSource == ButtonSource.prefabs)
                    {
                        Start();
                        return;
                    }
                    #endregion
                    #region Update the mouse rotation and extimate cursor rotation
                    cursor.fillAmount = Mathf.Lerp(cursor.fillAmount, desiredFill, .2f);
                    //orientamento cursore
                    Vector3 screenBounds = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
                    Vector3 vector = UnityEngine.Input.mousePosition - screenBounds;
                    if (tiltTowardsMouse)
                    {
                        float x = vector.x / screenBounds.x, y = vector.y / screenBounds.y;
                        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(x, y, 0) * -tiltAmount), lerpAmount);
                    }
                    float mouseRotation = ((controlType == ControlType.mouseAndTouch) ? Mathf.Atan2(vector.x, vector.y) : Mathf.Atan2(Input.GetAxis(gamepadAxisX), Input.GetAxis(gamepadAxisY))) * 57.29578f;
                    if (mouseRotation < 0f)
                        mouseRotation += 360f;
                    float cursorRotation = -(mouseRotation - cursor.fillAmount * 360f / 2f);
                    #endregion
                    #region Find and color the selected button
                    float difference = 9999;
                    GameObject nearest = null;
                    for (int i = 0; i < buttonCount; i++)
                    {
                        GameObject b;
                        if (buttonSource == ButtonSource.prefabs)
                            b = buttonsInstances[i].gameObject;
                        else
                            b = buttons[i];
                        b.transform.localScale = Vector3.one;
                        float rotation = System.Convert.ToSingle(b.name);// - 360 / buttonCount / 2;
                        if (Mathf.Abs(rotation - mouseRotation) < difference)
                        {
                            nearest = b;
                            difference = Mathf.Abs(rotation - mouseRotation);
                        }
                    }
                    SelectedSegment = nearest;

                    if (snap)
                        cursorRotation = -(System.Convert.ToSingle(SelectedSegment.name) - cursor.fillAmount * 360f / 2f);
                    cursor.transform.localRotation = Quaternion.Slerp(cursor.transform.localRotation, Quaternion.Euler(0, 0, cursorRotation), lerpAmount);

                    SelectedSegment.GetComponent<Image>().color = Color.Lerp(SelectedSegment.GetComponent<Image>().color, backgroundColor, lerpAmount);

                    for (int i = 0; i < buttons.Count; i++)
                    {
                        Button b = (buttonSource == ButtonSource.prefabs) ? buttonsInstances[i] : buttons[i].GetComponent<Button>();
                        if (b.gameObject != SelectedSegment)
                        {
                            if (b.unlocked)
                                b.GetComponent<Image>().color = Color.Lerp(b.GetComponent<Image>().color, accentColor, lerpAmount);
                            else
                                b.GetComponent<Image>().color = Color.Lerp(b.GetComponent<Image>().color, disabledColor, lerpAmount);
                        }
                    }
                    try
                    {
                        if (SelectedSegment.GetComponent<Button>().unlocked)
                            cursor.color = Color.Lerp(cursor.color, accentColor, lerpAmount);
                        else
                            cursor.color = Color.Lerp(cursor.color, disabledColor, lerpAmount);
                    }
                    catch { }

                    #endregion
                    #region Call the selected button action
                    if (Input.GetMouseButton(0))
                    {
                        cursor.rectTransform.localPosition = Vector3.Lerp(cursor.rectTransform.localPosition, new Vector3(0, 0, -10), lerpAmount);
                        if (SelectedSegment.GetComponent<Xamin.Button>().unlocked)
                        {
                            SelectedSegment.transform.localScale = new Vector2(.8f, .8f);
                        }
                    }
                    else
                        cursor.rectTransform.localPosition = Vector3.Lerp(cursor.rectTransform.localPosition, Vector3.zero, lerpAmount);
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (SelectedSegment.GetComponent<Xamin.Button>().unlocked)
                        {
                            SelectedSegment.GetComponent<Xamin.Button>().ExecuteAction();
                            localAudioSource.PlayOneShot(segmentClickedSound);
                            Close();
                        }
                    }
                    #endregion
                }
            }
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, (closeAnimation == AnimationType.zoomIn) ? Vector3.zero : Vector3.one * 10, .05f);
                cursor.color = Color.Lerp(cursor.color, Color.clear, lerpAmount / 5f);
                background.color = Color.Lerp(background.color, Color.clear, lerpAmount / 5f);
            }
        }
    }
}