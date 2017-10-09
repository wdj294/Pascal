
/*******************************************
Permission is granted to re-use this script provided this copyright notice remains intact

Techooka Labs
Author: Oz

This Script is a part of DisplayFab Demo Assets

http://displayfab.techooka.com
*******************************************/

using UnityEngine;
using System.Collections;

public class TriggerNextItemAnimationSMB : StateMachineBehaviour {


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
 
        animator.SendMessage("SendTriggerAnim",SendMessageOptions.DontRequireReceiver);
    }

}
