using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KK.StatePattern;
public class IdleState : State
{
    private Animator animator;
    public IdleState(Animator animator, FSM fsm) : base(fsm)
    {
        this.animator = animator;
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        animator.SetFloat("Speed", 0f);
    }
}
