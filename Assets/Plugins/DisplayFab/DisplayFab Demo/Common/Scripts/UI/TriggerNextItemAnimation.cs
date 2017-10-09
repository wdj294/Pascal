
/*******************************************
Permission is granted to re-use this script provided this copyright notice remains intact

Techooka Labs
Author: Oz

This Script is a part of DisplayFab Demo Assets

http://displayfab.techooka.com
*******************************************/


using UnityEngine;
using System.Collections;

public class TriggerNextItemAnimation : MonoBehaviour {

   
    public TriggerNextItemAnimation nextItem;

    public string initialAnimatorStateName = "PanelAppear";
    private Animator nextItemAnimatorComp;

    private CanvasGroup currentCanvasGroup;
	// Use this for initialization
	void Start ()
    {
        if(nextItem!= null)
        nextItemAnimatorComp = nextItem.GetComponent<Animator>();

        currentCanvasGroup = GetComponent<CanvasGroup>();
    }
	
    public void SendTriggerAnim()
    {
        if (nextItemAnimatorComp != null)
            nextItemAnimatorComp.SetTrigger("triggerAppear");
    }

    public void PanelAppear()
    {
        if (currentCanvasGroup != null)
            currentCanvasGroup.alpha = 1.0f;

        if (gameObject.GetComponent<Animator>() != null)
        {
            gameObject.GetComponent<Animator>().enabled = true;
            gameObject.GetComponent<Animator>().Play(initialAnimatorStateName);
        }

    }

    public void PanelDisappear()
    {
        if (currentCanvasGroup != null)
            currentCanvasGroup.alpha = 0.0f;

        if (gameObject.GetComponent<Animator>() != null)
            gameObject.GetComponent<Animator>().enabled = false;

    }

}
