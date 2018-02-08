using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace TutorialDesigner
{
	/// <summary>
	/// Definition for Dialogue position within the Canvas Gameobject in the scene
	/// </summary>
	public enum DialoguePanelPosition {middle, top, bottom, left, right};

	/// <summary>
	/// Definition for Image position within the Dialogue window. Either above or below the text
	/// </summary>
	public enum DialogueImagePosition {top, bottom};

	[System.Serializable]
	/// <summary>
	/// This is an interface between 1.) Editor scripts, where the user can change all kinds of dialogue settings,
	/// and 2.) all Gameobjects in the scene that have to do with the Dialogue. Text, Backgrounds, Images...
	/// </summary>
	public class Dialogue {	
		/// <summary>
		/// If the game should pause while displaying the dialogue
		/// </summary>
		public bool pauseGame;

		/// <summary>
		/// If Dialogue should have an animated appearance when it gets active
		/// </summary>
		public bool animate;

		/// <summary>
		/// The button result (clicked button) of up to 3 Dialogue buttons
		/// </summary>
		public int buttonResult = -1;

		/// <summary>
		/// Buttons panel. Variable size, depending on the number of visible buttons
		/// </summary>
		public Transform bPanel;

		[SerializeField]
		private RectTransform rectTransform;

		#if UNITY_EDITOR
		/// <summary>
		/// If this is checked, user can see much more options in dialogue settings
		/// </summary>
		public bool advancedSettings;

		/// <summary>
		/// Background design of the dialogue panel
		/// </summary>
		public Image dialogueBackgrImg;

		[SerializeField]
		private DialoguePanelPosition _panelPosition = DialoguePanelPosition.middle;
		/// <summary>
		/// Gets or sets the dialogue position on the screen
		/// </summary>
		public DialoguePanelPosition panelPosition {
			get {
				return _panelPosition;
			}
			set {                
				if (value != _panelPosition) {
					switch (value) {
						case DialoguePanelPosition.middle:
							rectTransform.anchorMin = new Vector2 (0.5f, 0.5f);
							rectTransform.anchorMax = new Vector2 (0.5f, 0.5f);
							rectTransform.pivot = new Vector2 (0.5f, 0.5f);
						break;
						case DialoguePanelPosition.top:
							rectTransform.anchorMin = new Vector2 (0.5f, 1f);
							rectTransform.anchorMax = new Vector2 (0.5f, 1f);
							rectTransform.pivot = new Vector2 (0.5f, 1f);
						break;
						case DialoguePanelPosition.bottom:
							rectTransform.anchorMin = new Vector2 (0.5f, 0f);
							rectTransform.anchorMax = new Vector2 (0.5f, 0f);
							rectTransform.pivot = new Vector2 (0.5f, 0f);
						break;
						case DialoguePanelPosition.left:
							rectTransform.anchorMin = new Vector2 (0f, 0.5f);
							rectTransform.anchorMax = new Vector2 (0f, 0.5f);
							rectTransform.pivot = new Vector2 (0f, 0.5f);
						break;
						case DialoguePanelPosition.right:
							rectTransform.anchorMin = new Vector2 (1f, 0.5f);
							rectTransform.anchorMax = new Vector2 (1f, 0.5f);
							rectTransform.pivot = new Vector2 (1f, 0.5f);
						break;
					}

					panelOffset = Vector2.zero;
					rectTransform.anchoredPosition = Vector2.zero;
				}

				_panelPosition = value;
			}
		}

		[SerializeField]
		private Vector2 _panelOffset;
		/// <summary>
		/// Gets or sets the panel offset to the screen border to wich it is aligned
		/// </summary>
		/// <value>Offset value</value>
		public Vector2 panelOffset {
			get {
				return _panelOffset;
			}
			set {
				if (value != _panelOffset) {
					rectTransform.anchoredPosition = value;
				}

				_panelOffset = value;
			}
		}

        /// <summary>
        /// The panel Layout Group Component
        /// </summary>
        public LayoutGroup panelLayout;

		/// <summary>
		/// Reference to the text object within dialogue
		/// </summary>
		public Text textObj;

		[SerializeField]
		private string _text;  
		/// <summary>
		/// Gets or sets the dialogue text and refreshes Scene- /Gamewindow in realtime
		/// </summary>
		public string text {
			get {
				return _text;
			}
			set {
				if (_text != value) {
					if (textObj != null) {
						textObj.text = value;
						// Make changes visible in scene immediately
						UnityEditor.EditorUtility.SetDirty(textObj);
					}
				}

				_text = value;
			}
		}

		/// <summary>
		/// Optional Image that can be displayed within the dialogue. This is only a reference.
		/// User sets it by sprite
		/// </summary>
		public Image imgObj;

		/// <summary>
		/// Enables scaling by user
		/// </summary>
		public LayoutElement imgLayout;

		/// <summary>
		/// Ratio: width / height
		/// </summary>
		public float imgRatio;

		/// <summary>
		/// Initial image size is 1. Can be changed by user via slide control while keeping the ratio
		/// </summary>
		public float imgSize = 1f;

		[SerializeField]
		private Sprite _sprite;
		/// <summary>
		/// Content of the dialogue Image. This is what the user can see / change in the dialogue settings
		/// </summary>
		public Sprite sprite {
			get {
				return _sprite;
			}
			set {
				if (value != _sprite) {
					if (value != null) {
						imgObj.gameObject.SetActive(true);
					} else {
						imgObj.gameObject.SetActive(false);
					}

					imgObj.sprite = value;
					if (value != null) {
						imgRatio = value.textureRect.width / value.textureRect.height;
					}
				}

				_sprite = value;
			}
		}

		[SerializeField]
		private DialogueImagePosition _imagePosition = DialogueImagePosition.bottom;
		/// <summary>
		/// Gets or sets the image position within dialogue
		/// </summary>
		/// <value>The image position.</value>
		public DialogueImagePosition imagePosition {
			get {
				return _imagePosition;
			}
			set {
				if (value != _imagePosition) {
					switch (value) {
					case DialogueImagePosition.top:
						textObj.transform.SetSiblingIndex(1);
						break;
					case DialogueImagePosition.bottom:
						textObj.transform.SetSiblingIndex(0);
						break;
					}
				}

				_imagePosition = value;
			}
		}

		/// <summary>
		/// Array containing button captions
		/// </summary>
		public string[] buttonCaptions;

		private LayoutElement[] btnLayout;
		private Text[] buttonTextObj;
		private Image[] buttonImgObj;
		#endif

		[SerializeField]
		private int _buttonCount;
		/// <summary>
		/// The number of buttons in dialogue. Can be up to 3 till now. F.i. "Yes", "No", "Cancel"
		/// </summary>
		public int buttonCount {
			get {
				return _buttonCount;
			}
			#if UNITY_EDITOR
			set {			
				// Unhide Responsebuttons if there are some, and caption them
                if (value != _buttonCount) { 
                    if (value > 0) {        
                        bPanel.gameObject.SetActive(true); 
                        for (int i=0; i<buttonCaptions.Length; i++) {  
                            if (i < value) {                         
                                buttonTextObj[i].transform.parent.gameObject.SetActive(true);   
                                buttonTextObj[i].text = buttonCaptions[i];  
                            } else { 
                                buttonTextObj[i].transform.parent.gameObject.SetActive(false);    
                            } 
                        } 
                    } else if (bPanel != null) { 
                        bPanel.gameObject.SetActive(false); 
                        for (int i=0; i<buttonCaptions.Length; i++) { 
                            buttonTextObj[i].transform.parent.gameObject.SetActive(false); 
                        } 
                    } 
                } 

				_buttonCount = value;
			}
			#endif
		}

		#if UNITY_EDITOR
		/// <summary>
		/// This size controls every button's layout size. They're always of the same size
		/// </summary>
		public Vector2 buttonSize;

		[SerializeField]
		private Sprite _buttonBackgroundImg;
		/// <summary>
		/// Background design of the buttons
		/// </summary>
		/// <value>Must be a Sprite with borders defined</value>
		public Sprite buttonBackgroundImg {
			get {
				return _buttonBackgroundImg;
			}
			set {
				if (value != _buttonBackgroundImg) {
					foreach (Image img in buttonImgObj) {
						img.sprite = value;
					}
				}

				_buttonBackgroundImg = value;
			}
		}

        [SerializeField]
        private Color _buttonBackgroundColor = Color.white;
        /// <summary>
        /// Color of the Button Background Sprite
        /// </summary>
        /// <value>Must be a Sprite with borders defined</value>
        public Color buttonBackgroundColor {
            get {
                return _buttonBackgroundColor;
            }
            set {
                InitComponents();
                if (value != _buttonBackgroundColor) {
                    foreach (Image img in buttonImgObj) {
                        img.color = value;
                    }
                }

                _buttonBackgroundColor = value;
            }
        }

		/// <summary>
		/// Updates either buttonSize, or the LayoutElement Components of their Gameobject. Depending on onlyGUI
		/// </summary>
		/// <param name="onlyGUI"><c>true</c>Internally used (buttonSize only). <c>true</c>Change by user. LayoutObjects</param>
		/// <param name="size">New desired size</param>
		public void UpdateButtonSizes(bool onlyGUI, Vector2 size) {
			if (onlyGUI) {
				// Update only GUI here (buttonSize). Called after other Elements alter the Buttons LayoutObjects
				if (buttonImgObj != null) {
					if (buttonImgObj.Length > 0) {
                        if (buttonImgObj[0] != null)
					        buttonSize = buttonImgObj[0].rectTransform.rect.size;
					}
				}
			} else {
				// Update Button Layout Objects. Called after User resizes them by Inspector
				foreach (LayoutElement le in btnLayout) {
					le.preferredWidth = size.x;
					le.preferredHeight = size.y;
				}
			}
		}

		/// <summary>
		/// The name of the currently selected dialogue prefab
		/// </summary>
		public string selectedPrefabName;

		/// <summary>
		/// Array of all dialogue prefabs
		/// </summary>
		public static string[] dialoguePrefabs;
		#endif

		[SerializeField]
		private int _selectedDialogueID;
		/// <summary>
		/// ID of currently selected dialogue. It will be set by user via EditorGUI.Popup
		/// </summary>
		public int selectedDialogueID {
			get {
				return _selectedDialogueID;
			}
			#if UNITY_EDITOR
			set {		
                string path = "Assets/TutorialDesigner/Resources/Dialogues";
                string[] files = AssetDatabase.FindAssets("t:prefab", new string[]{path});

				// Previous dialogue
				GameObject oldDialogue = GetGameObject();				
                if (oldDialogue != null) Undo.RegisterCompleteObjectUndo(oldDialogue, "Dialogue Change");

				if (value != 0) {			                         
                    string objectName = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(files[value - 1]));
                    selectedPrefabName = path + "/" + objectName + ".prefab";
					TutorialEditor.savePoint.HideAllDialogues();
					CreatePrefab(TutorialEditor.savePoint.canvas.gameObject, objectName);
                    if (_selectedDialogueID != 0) InitComponents(true);
                } else {
                    ClearComponents ();
                }

                // Delete the old dialogue
                if (oldDialogue != null) Undo.DestroyObjectImmediate(oldDialogue);

				_selectedDialogueID = value;
			}
			#endif
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Basic Constructor
		/// </summary>
		public Dialogue () {		            
			buttonCaptions = new string[3]; // Maximum 3
		}

		/// <summary>
		/// Initialization of all components
		/// </summary>
        public void InitComponents(bool force = false) {                
			bool needToInitialize = false;
			if (buttonTextObj == null || textObj == null) {
				needToInitialize = true;
			} else if (buttonTextObj.Length == 0) {
				needToInitialize = true;
			}

			if (needToInitialize || force) {
				if (rectTransform == null) {					
					CreatePrefab (TutorialEditor.savePoint.canvas.gameObject, Path.GetFileNameWithoutExtension(selectedPrefabName));
					return;
				}

                if (dialogueBackgrImg != null) EditorUtility.CopySerialized(dialogueBackgrImg, rectTransform.GetComponent<Image>());                
                dialogueBackgrImg = rectTransform.GetComponent<Image>();

                Vector2 tempOffset = _panelOffset;
                if ((int)panelPosition != 0) {
                    DialoguePanelPosition tempPos = _panelPosition;
                    _panelPosition = 0;
                    panelPosition = tempPos;
                }
                panelOffset = tempOffset;

                if (panelLayout != null) EditorUtility.CopySerialized(panelLayout, rectTransform.GetComponent<LayoutGroup>());
                panelLayout = rectTransform.GetComponent<LayoutGroup>();

                bPanel = rectTransform.Find ("ButtonsPanel");
                if (textObj != null) {                    
                    EditorUtility.CopySerialized(textObj, rectTransform.Find("Text").GetComponent<Text>());
                    // Button Text Objects
                    foreach(Text t in bPanel.GetComponentsInChildren<Text>(true)) {
                        EditorUtility.CopySerialized(textObj, t);
                        t.alignment = TextAnchor.MiddleCenter;
                    }
                }

                textObj = rectTransform.Find("Text").GetComponent<Text>();
				textObj.text = text;
                buttonTextObj = bPanel.GetComponentsInChildren<Text>(true);
                // Button Component Initialization Trigger
                if (buttonCount > 0) {
                    // Enables GameObjects for ButtonPanel and Buttons if available
                    int btCount = buttonCount;
                    buttonCount = 0; // set it to 0 and back to previous value
                    buttonCount = btCount; // to trigger variable change event at the top
                }

                if (buttonCaptions != null) if (buttonCaptions.Length > 0) {                     
                    for (int i=0; i<buttonCaptions.Length; i++) { 
                        buttonTextObj[i].text = buttonCaptions[i]; 
                    } 
                } 

				Transform img = rectTransform.Find("Image");
                if (imgObj != null) {                             
                    EditorUtility.CopySerialized(imgObj, img.GetComponent<Image>());
                    imgObj = img.GetComponent<Image>();
                    sprite = imgObj.sprite;
                    if (sprite != null) imgObj.gameObject.SetActive(true);
                    if (imagePosition == DialogueImagePosition.top) textObj.transform.SetSiblingIndex(1);
                } else {
                    imgObj = img.GetComponent<Image>();
                }
				
                if (imgLayout != null) EditorUtility.CopySerialized(imgLayout, img.GetComponent<LayoutElement>());
				imgLayout = img.GetComponent<LayoutElement>();                								

				btnLayout = bPanel.GetComponentsInChildren<LayoutElement> (true);
                buttonImgObj = new Image[bPanel.childCount];
				for (int i=0; i<buttonImgObj.Length; i++) {
					buttonImgObj[i] = bPanel.GetChild(i).GetComponent<Image>(); 
				}

                if (buttonBackgroundImg == null) {
                    buttonBackgroundImg = buttonImgObj[0].sprite;
                } else {
                    for (int i=0; i<buttonImgObj.Length; i++) {
                        buttonImgObj[i].sprite = buttonBackgroundImg; 
                        if (buttonBackgroundColor != Color.white) buttonImgObj[i].color = buttonBackgroundColor;
                    }
                }                        
				
				if (buttonSize == Vector2.zero) {						
					buttonSize = new Vector2 (50f, 50f);
                } else {
                    UpdateButtonSizes(false, buttonSize);
                }
			}
		}

        /// <summary>
        /// Reset all of the dialogue's components, so they must be freshly initialized
        /// </summary>
		public void ClearComponents() {
			buttonImgObj = null;
			_buttonBackgroundImg = null;
			rectTransform = null;
			buttonTextObj = null;
			textObj = null;
			imgObj = null;
			bPanel = null;
			btnLayout = null;
            text = "";
		}

		/// <summary>
		/// Creates the prefab for the dialogue. Will be a child of selected SavePoint.canvas
		/// </summary>
		/// <param name="canvas">Canvas which will be the parent of this dialogue</param>
		/// <param name="name">Prefab name</param>
		public void CreatePrefab(GameObject canvas, string name) {
			RectTransform prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<RectTransform>(selectedPrefabName) as RectTransform;
			RectTransform gObj = (RectTransform)GameObject.Instantiate(prefab);
			gObj.name = name;
			rectTransform = gObj.GetComponent<RectTransform>();

			rectTransform.SetParent(canvas.transform);
			rectTransform.anchoredPosition = prefab.anchoredPosition;
			rectTransform.offsetMax = prefab.offsetMax;
			rectTransform.offsetMin = prefab.offsetMin;
			rectTransform.localScale = prefab.localScale;

			InitComponents();
			Undo.RegisterCreatedObjectUndo (gObj.gameObject, "Dialogue change");
		}

		/// <summary>
		/// Returns the dialogue's Gameobject if available
		/// </summary>
		public GameObject GetGameObject() {
			if (rectTransform != null)
				return rectTransform.gameObject;
			else
				return null;
		}

		/// <summary>
		/// Copies the settings to another dialogue. Used after duplication of a StepNode
		/// </summary>
		/// <param name="d">The other dialogue</param>
		public void CopySettingsTo(Dialogue d) {
			if (selectedDialogueID == 0) return;

			d.selectedDialogueID = selectedDialogueID;

			// Dialogue Panel
			d.panelPosition = panelPosition;
			d.panelOffset = panelOffset;
            d.panelLayout.padding = panelLayout.padding;
            d.animate = animate;
			EditorUtility.CopySerialized(dialogueBackgrImg, d.dialogueBackgrImg);

			// Text
			d.text = text;
			EditorUtility.CopySerialized(textObj, d.textObj);

			// Image
			d.sprite = sprite;
			EditorUtility.CopySerialized(imgObj, d.imgObj);
			d.imagePosition = imagePosition;
			d.imgSize = imgSize;

			// Buttons
			d.buttonCount = buttonCount;
			buttonCaptions.CopyTo(d.buttonCaptions, 0);
			d.InitComponents();
			d.buttonBackgroundImg = buttonBackgroundImg;
            d.buttonBackgroundColor = buttonBackgroundColor;
			d.buttonSize = buttonSize;
			for (int i = 0; i < buttonTextObj.Length; i++) {
				EditorUtility.CopySerialized (buttonTextObj [i], d.buttonTextObj [i]);
				EditorUtility.CopySerialized (btnLayout [i], d.btnLayout [i]);
				EditorUtility.CopySerialized (buttonImgObj [i], d.buttonImgObj [i]);
			}

			d.pauseGame = pauseGame;
		}

		/// <summary>
		/// Apply text settings, set by user, to all Text objects within the buttons
		/// </summary>
		public void ApplyTextSettingsToButtons() {
            InitComponents();
            Undo.RecordObjects(buttonTextObj, "Text Settings");
            foreach (Text t in buttonTextObj) {                
				t.fontSize = textObj.fontSize;
				t.fontStyle = textObj.fontStyle;
				t.color = textObj.color;
				t.material = textObj.material;
                t.font = textObj.font;
			}
		}
		#endif

		/// <summary>
		/// Activates or Deactivated the dialogue. Including visibility in Sceneview
		/// </summary>
		public void SetActive(bool value) {	
			if (selectedDialogueID != 0) {
				if (rectTransform != null) {
					rectTransform.gameObject.SetActive (value);
					if (Application.isPlaying && value) {
						if (animate && Time.timeScale > 0) {
							// Dialogue must appear animated
                            GameObject.Find("TutorialSystem").GetComponent<SavePoint>().StartCoroutine(PopupAnimation());
						} else if (pauseGame){
							// Pause the game
							Time.timeScale = 0;
						}
					}
				}
				#if UNITY_EDITOR
				else 
					selectedDialogueID = 0;
				#endif
			}		
		}

		// Window appears animated. Scales up a little
		private IEnumerator PopupAnimation() {
			Vector3 targetScale = rectTransform.localScale;

			float t = 0.85f;
			while (t < 1f) {
				t += Time.deltaTime / Time.timeScale;
				rectTransform.localScale = targetScale * t * t;
				yield return null;
			}

			rectTransform.localScale = targetScale;
			if (pauseGame) Time.timeScale = 0;
		}

		/// <summary>
		/// Adds listeners to the buttons, which will (if pressed) set buttonResult to
		/// according to their position in hierarchy (SiblingIndex)
		/// </summary>
		public void AddButtonListeners() {
			if (buttonCount > 0) {			
				for (int i=0; i<buttonCount; i++) {				
					Transform button = bPanel.GetChild(i);	
					button.gameObject.SetActive(true);
					button.GetComponentInChildren<Button> ().onClick.AddListener (delegate() {
						buttonResult = button.GetSiblingIndex();
					});
				}
			}
		}

        #if UNITY_EDITOR
		/// <summary>
		/// Updates the button captions, to be visible in Game- or Scene-Window
		/// </summary>
		public void UpdateButtonCaptions(int index=-1) {			
			if (buttonTextObj != null) {
				if (index == -1) {				
					for (int i = 0; i < buttonTextObj.Length; i++) {
						buttonTextObj [i].text = buttonCaptions [i];
						EditorUtility.SetDirty (buttonTextObj [i]);
					}
				} else {				
					buttonTextObj [index].text = buttonCaptions [index];
					EditorUtility.SetDirty (buttonTextObj [index]);
				}
			}
		}
        #endif
	}
}
