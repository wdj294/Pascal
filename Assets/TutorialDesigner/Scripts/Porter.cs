#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using TutorialDesigner.FullSerializer;

namespace TutorialDesigner{
    /// <summary>
    /// Custom Converter for FullSerializer. Convertes resources from Project folder or built in resources for being
    /// imported and exported to JSON format.
    /// </summary>
    public class ResourceConverter : fsConverter {
        public override bool CanProcess(Type type) {
            // CanProcess will be called over every type that Full Serializer
            // attempts to serialize. If this converter should be used, return true
            // in this function.
            return (type == typeof(Sprite) || type == typeof(Material) || type == typeof(Font));
        }

        /// <summary>
        /// This Function must be included when inheriting fsConverter. It won't be used, though.
        /// For serialization, the processer will be used. Because there is also "id" and "ref" data stored
        /// </summary>
        /// <param name="instance">The object instance to serialize. This will never be null.</param>
        /// <param name="serialized">The serialized state.</param>
        /// <param name="storageType">The field/property type that is storing this instance.</param>
        /// <returns>If serialization was successful.</returns>
        public override fsResult TrySerialize(object instance,
            out fsData serialized, Type storageType) {

            serialized = new fsData();
            return fsResult.Success;
        }

        /// <summary>
        /// Deserialize data into the object instance.
        /// </summary>
        /// <param name="data">Serialization data to deserialize from.</param>
        /// <param name="instance">The object instance to deserialize into.</param>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <returns>True if serialization was successful, false otherwise.</returns>
        public override fsResult TryDeserialize(fsData data,
            ref object instance, Type storageType) {

            if (data.AsDictionary.ContainsKey("path")) {            
                // Resource is in Project folder
                if (data.AsDictionary.ContainsKey("arrayIndex")) {
                    // Resource is part of a collection, like a sprite in a multisheet
                    UnityEngine.Object[] array = AssetDatabase.LoadAllAssetsAtPath(data.AsDictionary["path"].AsString);
                    string[] stringArray = Array.ConvertAll<UnityEngine.Object, string>(array, new Converter<UnityEngine.Object, string>(ObjectsConverter));
                    int index = Array.IndexOf(stringArray, data.AsDictionary["arrayIndex"].AsString);
                    if (index != -1) instance = array[index];
                } else {
                    // Resource is a single object
                    instance = AssetDatabase.LoadAssetAtPath(data.AsDictionary["path"].AsString, storageType);
                }                    
            } else if (data.AsDictionary.ContainsKey("extra")) {
                // Resource is builtIn. Must be loaded from there
                UnityEngine.Object[] extraContent = AssetDatabase.LoadAllAssetsAtPath("Resources/unity_builtin_extra");
                string name = data.AsDictionary["extra"].AsString;
                // Look in builtIn Resources if name matches
                foreach (UnityEngine.Object o in extraContent) {
                    if (o.GetType () == storageType) {
                        if (((UnityEngine.Object)o).name == name) {
                            instance = o;
                        }
                    }
                }
            }

            return fsResult.Success;
        }

        private string ObjectsConverter(UnityEngine.Object input) {
            return input.name;
        }

        /// <summary>
        /// Must be implemented but This is never executed because fsMetaType.cs - Activator.CreateInstance won't be called.
        /// This is because MonoBeaviour cannot be instantiated. Instead a new ScriptableObject is created in fsMetaType.fs
        /// </summary>
        /// <param name="data">The data the object was serialized with.</param>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <returns>An object instance</returns>
        public override object CreateInstance(fsData data, Type storageType) {
            return null;
        }
    }

    /// <summary>
    /// Custom Processor for Dialogue Serialization by FullSerializer
    /// </summary>
	public class DialogueProcessor : fsObjectProcessor {
		public override bool CanProcess(Type type) {
			return type == typeof(Dialogue);
		}

        /// <summary>
        /// Called after deserialization.
        /// </summary>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <param name="instance">The type of the instance.</param>
		public override void OnAfterDeserialize(Type storageType, object instance) {	
            Dialogue d = ((Dialogue)instance);
            if (d.selectedDialogueID != 0) d.InitComponents();
		}
	}

    /// <summary>
    /// Resource processor for FullSerializer. Writes paths of the resources. By doing so, all "id" and "ref" links will be deleted
    /// for this specific objects. Sprites and Materials won't work by that references. They get confused
    /// </summary>
    public class ResourceProcessor : fsObjectProcessor {
        private fsData storageData;
        public override bool CanProcess(Type type) {
            return (type == typeof(Sprite) || type == typeof(Material) || type == typeof(Font));
        }

        /// <summary>
        /// Write Paths of the resource
        /// </summary>
        /// <param name="storageType">The field/property type that is storing the instance.</param>
        /// <param name="instance">The type of the instance.</param>
        /// <param name="data">The data that was serialized.</param>
        public override void OnAfterSerialize(Type storageType, object instance, ref fsData data) {  
            data = new fsData();

            // If current item is null, return empty fsData. Which will be converted to "null" in the json
            if (instance == null) return;            

            string path = AssetDatabase.GetAssetPath(instance.GetHashCode());
            Dictionary<string, fsData> dic = new Dictionary<string, fsData>();

            // Resource is a builtIn Resource
            if (path == "Resources/unity_builtin_extra") {      
                dic.Add("extra", new fsData(((UnityEngine.Object)instance).name));          
            } else {
                // Resource is from Project folder
                dic.Add("path", new fsData(path));
                if (instance.ToString() != "null") {
                    bool isArray = AssetDatabase.LoadAllAssetsAtPath(path).Length > 2;
                    if (isArray) {
                        dic.Add("arrayIndex", new fsData(((UnityEngine.Object)instance).name));
                    }
                }
            }

            data = new fsData(dic);
        }
    }

    /// <summary>
    /// Class for Import and Export of the Tutorial Nodes to JSON format
    /// </summary>
	public static class Porter{
		private static fsSerializer fs;
		private static fsData data;
		private static string json;
		private static double version = 1.2;

		private static void Init() {
			fs = new fsSerializer ();
			fs.AddConverter (new FullSerializer.Internal.Converters.UnityEvent_Converter());
            fs.AddConverter (new ResourceConverter());
            fs.AddProcessor(new ResourceProcessor());
			fs.AddProcessor(new DialogueProcessor());
			data = new fsData ();
			json = "";
		}

        /// <summary>
        /// Export all nodes to JSON string
        /// </summary>
		public static string Export() {
			Init ();
			data.BecomeDictionary ();
			data.AsDictionary.Add ("version", new fsData (version));

			fsData nodeData = new fsData();
			fs.TrySerialize<List<Node>> (TutorialEditor.savePoint.nodes, out nodeData).AssertSuccessWithoutWarnings ();  
			data.AsDictionary.Add("nodes", nodeData);

			json = FullSerializer.fsJsonPrinter.CompressedJson(data);
			return json;
		}

        /// <summary>
        /// Import a JSON string to this Tutorial. Add all imported Nodes to the existing ones
        /// </summary>
        /// <param name="json">Json.</param>
        public static string Import(string json) {
			Init ();

            data = fsJsonParser.Parse (json);

			// Check if the Version matches (or newer)
			double tutorialVersion = data.AsDictionary["version"].AsDouble;
			if (tutorialVersion > version) {
				// Imported Version is newer and cannot be imported
				Debug.LogError("Tutorial cannot be imported because its version is newer than yours. Please upgrade");
				return "";
			}

            List<Node> importedNodes = new List<Node>();

            Undo.RegisterCompleteObjectUndo (TutorialEditor.savePoint, "Import Nodes");
			fs.TryDeserialize<List<Node>> (data.AsDictionary["nodes"], ref importedNodes);

            // Check the imported nodes and save the ones to a list, that exists only because the connection entries are cycled by FullSerializer
            foreach (Node n in importedNodes) {
                TutorialEditor.savePoint.nodes.Add(n);
                n.InitAfterImport ();
            }

            TutorialEditor.savePoint.HideAllDialogues();
            return json;
		}
	}
}
#endif