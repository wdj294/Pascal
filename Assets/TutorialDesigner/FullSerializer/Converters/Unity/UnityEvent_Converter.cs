#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace TutorialDesigner.FullSerializer {
    partial class fsConverterRegistrar {
        // Disable the converter for the time being. Unity's JsonUtility API
        // cannot be called from within a C# ISerializationCallbackReceiver
        // callback.

        // public static Internal.Converters.UnityEvent_Converter
        // Register_UnityEvent_Converter;
    }
}

namespace TutorialDesigner.FullSerializer.Internal.Converters {
    // The standard FS reflection converter has started causing Unity to crash
    // when processing UnityEvent. We can send the serialization through
    // JsonUtility which appears to work correctly instead.
    //
    // We have to support legacy serialization formats so importing works as
    // expected.
    public class UnityEvent_Converter : fsConverter {
        public override bool CanProcess(Type type) {
            return typeof(UnityEvent).Resolve().IsAssignableFrom(type) && type.IsGenericType == false;
        }

        public override bool RequestCycleSupport(Type storageType) {
            return false;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType) {            
            fsResult result = fsResult.Success;

			// Get List of UnityEvent calls
			List<fsData> m_calls = data.AsDictionary ["m_PersistentCalls"].AsDictionary ["m_Calls"].AsList;				
			foreach (fsData call in m_calls) {
				// Get the Object name (Game Objectreference from UnityEvent), find out its current InstanceID
				// and pass that into UnityEvent, because Event stores only the reference to the object (instanceID)
                string objname = call.AsDictionary["m_Target"].AsDictionary["objName"].AsString;
                string typename = call.AsDictionary["m_Target"].AsDictionary["type"].AsString;
                int instanceID = FindID(objname, typename);
				call.AsDictionary["m_Target"].AsDictionary.Clear();
				call.AsDictionary["m_Target"].AsDictionary.Add("instanceID", new fsData(instanceID));

				// The same for m_ObjectArgument in UnityEvent, if it is set (not 0)
                objname = call.AsDictionary ["m_Arguments"].AsDictionary["m_ObjectArgument"].AsDictionary["objName"].AsString;
                typename = call.AsDictionary ["m_Arguments"].AsDictionary["m_ObjectArgumentAssemblyTypeName"].AsString;
				string assPath = call.AsDictionary ["m_Arguments"].AsDictionary ["m_ObjectArgument"].AsDictionary ["assetPath"].AsString;
				instanceID = FindID(objname, typename, assPath);
				call.AsDictionary ["m_Arguments"].AsDictionary["m_ObjectArgument"].AsDictionary.Clear();
				call.AsDictionary ["m_Arguments"].AsDictionary["m_ObjectArgument"].AsDictionary.Add("instanceID", new fsData(instanceID));
			}
            instance = JsonUtility.FromJson<UnityEvent>(fsJsonPrinter.CompressedJson(data));
			//Debug.Log (fsJsonPrinter.PrettyJson(data));
            return result;
        }

        public override fsResult TrySerialize(object instance, out fsData data, Type storageType) {
            fsResult result = fsResult.Success;
			data = fsJsonParser.Parse(JsonUtility.ToJson(instance));

			List<fsData> m_calls = data.AsDictionary ["m_PersistentCalls"].AsDictionary ["m_Calls"].AsList;
			foreach (fsData call in m_calls) {
				// Instead saving the GameObject Reference as $type ... which is default by FullSerializer,
				// save the Object Name, so it can be searched at the Import. The name must not be changed of course
				int instanceID = (int)call.AsDictionary["m_Target"].AsDictionary["instanceID"].AsInt64;
				call.AsDictionary["m_Target"].AsDictionary.Clear();
                UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceID);
                call.AsDictionary["m_Target"].AsDictionary.Add(
					"objName",
                    new fsData(obj == null ? "null" : obj.name));
                call.AsDictionary["m_Target"].AsDictionary.Add(
                    "type",
                    new fsData(obj == null ? "null" : obj.GetType().ToString()));

				// Same thing for ObjectArgument in UnityEvents, another GameObject reference
				instanceID = (int)call.AsDictionary ["m_Arguments"].AsDictionary ["m_ObjectArgument"].AsDictionary["instanceID"].AsInt64;
				call.AsDictionary["m_Arguments"].AsDictionary ["m_ObjectArgument"].AsDictionary.Clear ();
                obj = EditorUtility.InstanceIDToObject(instanceID);
                call.AsDictionary["m_Arguments"].AsDictionary ["m_ObjectArgument"].AsDictionary.Add(
					"objName",
                    new fsData(obj == null ? "null" : obj.name));
				string assPath = AssetDatabase.GetAssetPath(instanceID); // Asset Path if loaded from Resources
				call.AsDictionary["m_Arguments"].AsDictionary ["m_ObjectArgument"].AsDictionary.Add(
					"assetPath",
					new fsData(assPath));				
			}

			return result;
        } 

		private int FindID(string name, string typename, string assetPath="")
        {
			// If m_target is an asset, loaded from Project folder
			if (assetPath != "") {
				UnityEngine.Object o = AssetDatabase.LoadAssetAtPath (assetPath, Type.GetType (typename + ", UnityEngine"));
				if (o != null)
					return o.GetInstanceID ();
			}

            // If m_target is a component, search for an Object with a Component of that type
            foreach (UnityEngine.Object o in Resources.FindObjectsOfTypeAll(typeof(GameObject))) {
                if (o.name == name) {
                    GameObject go = (GameObject)o;
                    if (go.GetComponent(typename) != null)
                        return go.GetComponent(typename).GetInstanceID();
                }
            }

            // If m_target is another Object (no component), search for an Object of that type
            if (Type.GetType(typename + ", UnityEngine") != null) {
                foreach (UnityEngine.Object o in Resources.FindObjectsOfTypeAll(Type.GetType(typename + ", UnityEngine"))) {
                    if (o.name == name) {
                        return o.GetInstanceID();
                    }
                }
            }

            if (name != "null") Debug.LogWarning("A referenced Object or Component of type: " + typename + " was not found. Name: " + name);

            return 0;
        }
    }
}
#endif