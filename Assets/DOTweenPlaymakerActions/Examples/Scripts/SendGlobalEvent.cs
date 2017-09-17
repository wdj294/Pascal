using UnityEngine;

public class SendGlobalEvent : MonoBehaviour {


    public string eventName;


    public void SendEvent(string _eventName)
    {
        PlayMakerFSM.BroadcastEvent(_eventName);
    }
}
