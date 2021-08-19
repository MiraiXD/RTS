using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyComboCounter : StateMachineBehaviour
{
    BattleUnitModel model;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (BattleUnit.models.ContainsKey(animator)) model = BattleUnit.models[animator];
        //if (!BattleUnit.models.TryGetValue(animator, out BattleUnitModel model)) Debug.Log("HUJ");
        //else
        //{

        //    animator.speed = stateInfo.length * model.WeaponAttackSpeed;
        //    Debug.Log("ENTER " + animator.speed);
        //}
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (model != null)
            animator.speed = Mathf.Clamp( stateInfo.length * model.WeaponAttackSpeed, 1f, 3f);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.speed = 1f;
        //Debug.Log("EXIT " + animator.speed);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
