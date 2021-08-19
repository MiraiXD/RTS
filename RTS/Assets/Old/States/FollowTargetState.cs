using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KK.StatePattern;
using KK.NavGrid;

public class FollowTargetState : MoveState
{
    private Transform target;

    public FollowTargetState(NavGridAgent agent, Animator animator, BattleUnit unit, FSM fsm) : base(agent, animator, unit, fsm)
    {

    }

    public void SetTarget(Transform target, int stoppingDistance)
    {
        this.target = target;
        SetDestination(target.position, stoppingDistance);
    }
    public override void OnStateUpdate(float deltaTime)
    {
        if (target.position != destination)
        {
            destination = target.position;
            agent.SetDestination(agent.navGrid.WorldToCell( destination));
        }

        base.OnStateUpdate(deltaTime);
    }

}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using KK.StatePattern;
//using UnityEngine.AI;

//public class FollowTargetState : MoveState
//{
//    private Transform target;

//    public FollowTargetState(NavMeshAgent agent, NavMeshObstacle obstacle, Animator animator, BattleUnit unit, FSM fsm) : base(agent,obstacle, animator, unit, fsm)
//    {

//    }

//    public void SetTarget(Transform target, float stoppingDistance)
//    {
//        this.target = target;
//        SetDestination(target.position, stoppingDistance);
//    }  
//    public override void OnStateUpdate(float deltaTime)
//    {
//        if(target.position != destination)
//        {
//            destination = target.position;
//            agent.SetDestination(destination);
//        }

//        base.OnStateUpdate(deltaTime);
//    }

//}
