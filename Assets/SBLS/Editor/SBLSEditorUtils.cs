using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SBLS;

namespace SBLS {

	public class SBLSEditorUtils : MonoBehaviour {

		/**
		 * For taking care of our assets
		 **/
		public static void CreateAsset<T>() where T : ScriptableObject {
			CreateAsset<T>(typeof(T).ToString());
		}
		
		// thanks to http://www.jacobpennock.com/Blog/?p=670
		public static void CreateAsset<T>(string baseName, string forcedPath = "") where T : ScriptableObject {
			T asset = ScriptableObject.CreateInstance<T>();
			
			string path = "";
			if (!string.IsNullOrEmpty(forcedPath)) {
				path = forcedPath;
				Directory.CreateDirectory(path);
			} else {
				if (!baseName.Contains("/")) {
					path = AssetDatabase.GetAssetPath(Selection.activeObject);
				}
				
				if (string.IsNullOrEmpty(path)) {
					path = "Assets";
				} else if (System.IO.Path.GetExtension(path) != "") {
					path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
				}
			}
			
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + baseName + ".asset");
			
			AssetDatabase.CreateAsset(asset, assetPathAndName);
			
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
		}
		
		// path like "Assets/scene.unity"
		public static bool Exists(string path) {
			return File.Exists(path);
		}
		
		public static T TryLoadComponent<T>(string guid) where T : Component {
			var go = TryLoadGameObject(guid);
			if (go == null) {
				return null;
			}
			
			return go.GetComponent<T>();
		}
		
		public static GameObject TryLoadGameObject(string guid) {
			string assetPath = AssetDatabase.GUIDToAssetPath(guid);
			if (string.IsNullOrEmpty(assetPath)) {
				return null;
			}
			
			var go = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
			return go;
		}
	}


}
