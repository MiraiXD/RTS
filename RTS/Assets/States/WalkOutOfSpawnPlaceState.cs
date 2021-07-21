using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KK.StatePattern;
using KK.NavGrid;
public class WalkOutOfSpawnPlaceState : MoveState
{
    public WalkOutOfSpawnPlaceState(NavGridAgent agent, Animator animator, BattleUnit unit, FSM fsm) : base(agent, animator, unit, fsm)
    {
    }
    public override void OnStateEnter()
    {
        unit.SetSpeed(5f);
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(agent.navGrid.WorldToCell(destination));
    }
    public override void OnStateExit()
    {
        base.OnStateExit();

        unit.SetSpeed(unit.model.DefaultMovementSpeed);
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using KK.StatePattern;
//using UnityEngine.AI;
//public class WalkOutOfSpawnPlaceState : MoveState
//{
//    public WalkOutOfSpawnPlaceState(NavMeshAgent agent,NavMeshObstacle obstacle, Animator animator, BattleUnit unit, FSM fsm) : base(agent, obstacle, animator, unit, fsm)
//    {
//    }
//    public override void OnStateEnter()
//    {
//        obstacle.enabled = false;
//        agent.enabled = true;

//        unit.SetSpeed(5f);
//        agent.stoppingDistance = stoppingDistance;
//        agent.SetDestination(destination);
//    }
//    public override void OnStateExit()
//    {
//        base.OnStateExit();

//        unit.SetSpeed(unit.model.DefaultMovementSpeed);        
//    }
//}