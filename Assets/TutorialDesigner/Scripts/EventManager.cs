using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* 	Simple Event System. Receives events and sends it to it's listeners
	Can be called from everywhere like this:

	EventManager.AddListener(Callback);

	// According function
	void Callback(string e) {
		Debug.Log(e);
	}


	Events can be globally triggered:
	EventManager.TriggerEvent("eventname");
*/
namespace TutorialDesigner
{
	public static class EventManager{

		public delegate void EventCall(string e); // callback funktion that will be the listener.
		static private List<EventCall> EventListeners; // list of listeners

		// Initialization
		public static void Initialize() {
			EventListeners = new List<EventCall>();
		}

		// New Listener
		public static void AddListener(EventCall ec) {
			if (EventListeners != null) {
				EventListeners.Add(ec);
			} else {
				Debug.LogError("EventManager was not initialized");
			}
		}

		// Sent Triggered Event to every Listener
		public static void TriggerEvent(string e) {
			if (EventListeners != null) {
				for (int i=0; i<EventListeners.Count; i++) {
					EventListeners[i](e);
				}
			}
		}

		public static void RemoveListener(EventCall ec) {
			EventListeners.Remove (ec);
		}
	}
}