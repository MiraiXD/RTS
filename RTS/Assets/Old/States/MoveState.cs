using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using KK.StatePattern;
using UnityEngine;
using KK.NavGrid;
public class MoveState : State
{
    //protected NavMeshAgent agent;
    //protected NavMeshObstacle obstacle;
    protected NavGridAgent agent;
    protected Animator animator;
    protected BattleUnit unit;
    protected Vector3 destination;
    protected int stoppingDistance;
    protected int speedHash = Animator.StringToHash("Speed");

    //public MoveState(NavMeshAgent agent, NavMeshObstacle obstacle, Animator animator, BattleUnit unit, FSM fsm) : base(fsm)
    //{
    //    this.agent = agent;
    //    this.obstacle = obstacle;
    //    this.animator = animator;
    //    this.unit = unit;
    //    agent.autoTraverseOffMeshLink = false;
    //    agent.updatePosition = false;
    //    agent.updateRotation = false;
    //    agent.acceleration = 999999999999999f;

    //}
    public MoveState(NavGridAgent agent, Animator animator, BattleUnit unit, FSM fsm) : base(fsm)
    {
        this.agent = agent;
        this.animator = animator;
        this.unit = unit;

        agent.onDestinationReached += OnDestinationReached;
    }
    public void SetDestination(Vector3 destination, int stoppingDistance = 0)
    {
        this.destination = destination;
        this.stoppingDistance = stoppingDistance;
    }
    public override void OnStateEnter()
    {
        base.OnStateEnter();

        unit.SetSpeed(unit.model.CurrentMovementSpeed);
        agent.stoppingDistance = stoppingDistance;        
        agent.SetDestination(agent.navGrid.WorldToCell(destination));
    }
    public override void OnStateUpdate(float deltaTime)
    {
        base.OnStateUpdate(deltaTime);

        //if (agent.pathStatus != PathStatus.PathComplete) { return; }

        if (agent.UpdateAgent(deltaTime))
        {
            Vector3 moveVector = agent.nextPosition - agent.transform.position;

            if (moveVector != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveVector.normalized);
                agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, targetRotation, deltaTime * 500f);
            }

            agent.transform.position = agent.nextPosition;

            animator.SetFloat(speedHash, moveVector.magnitude / deltaTime / unit.model.MaxMovementSpeed);
        }
    }

    //public override void OnStateExit()
    //{
    //    base.OnStateExit();
    //    agent.enabled = false;
    //    obstacle.enabled = true;
    //    agent.avoidancePriority = 0;
    //}

    private void OnDestinationReached()
    {
        fsm.FinishCurrentState();
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.AI;
//using KK.StatePattern;
//using UnityEngine;
//using KK.NavGrid;
//public class MoveState : State
//{
//    //protected NavMeshAgent agent;
//    //protected NavMeshObstacle obstacle;
//    protected NavGridAgent agent;
//    protected Animator animator;
//    protected BattleUnit unit;
//    protected Vector3 destination;
//    protected int stoppingDistance;
//    protected int speedHash = Animator.StringToHash("Speed");

//    //public MoveState(NavMeshAgent agent, NavMeshObstacle obstacle, Animator animator, BattleUnit unit, FSM fsm) : base(fsm)
//    //{
//    //    this.agent = agent;
//    //    this.obstacle = obstacle;
//    //    this.animator = animator;
//    //    this.unit = unit;
//    //    agent.autoTraverseOffMeshLink = false;
//    //    agent.updatePosition = false;
//    //    agent.updateRotation = false;
//    //    agent.acceleration = 999999999999999f;

//    //}
//    public MoveState(NavGridAgent agent, Animator animator, BattleUnit unit, FSM fsm) : base(fsm)
//    {
//        this.agent = agent;        
//        this.animator = animator;
//        this.unit = unit;        
//    }
//    public void SetDestination(Vector3 destination, int stoppingDistance = 0)
//    {
//        this.destination = destination;
//        this.stoppingDistance = stoppingDistance;
//    }
//    public override void OnStateEnter()
//    {
//        base.OnStateEnter();

//        //obstacle.enabled = false;
//        //agent.enabled = true;
//        //agent.avoidancePriority = 50;

//        unit.SetSpeed(unit.model.CurrentMovementSpeed);
//        agent.stoppingDistance = stoppingDistance;
//        agent.SetDestination(agent.navGrid.WorldToCell(destination));  
//    }    
//    public override void OnStateUpdate(float deltaTime)
//    {
//        base.OnStateUpdate(deltaTime);

//        if (agent.pathPending) { return; }

//        Vector3 nextPosition;
//        if (agent.isOnOffMeshLink)
//        {
//            var data = agent.currentOffMeshLinkData;
//            //nextPosition = agent.transform.position + (data.endPos - data.startPos).normalized * deltaTime * unit.model.CurrentSpeed;
//            nextPosition = Vector3.MoveTowards(agent.transform.position, data.endPos, unit.model.CurrentMovementSpeed * deltaTime);
//            if (nextPosition == data.endPos) agent.CompleteOffMeshLink();
//        }
//        else
//        {
//            nextPosition = agent.nextPosition;
//        }


//        Vector3 moveVector = nextPosition - agent.transform.position;

//        if (moveVector != Vector3.zero)
//        {
//            Quaternion targetRotation = Quaternion.LookRotation(moveVector.normalized);
//            agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, targetRotation, deltaTime * 500f);
//        }

//        agent.transform.position = nextPosition;
//        animator.SetFloat(speedHash, moveVector.magnitude / deltaTime / unit.model.MaxMovementSpeed);

//        if (agent.remainingDistance <= agent.stoppingDistance) fsm.FinishCurrentState();
//    }

//    public override void OnStateExit()
//    {
//        base.OnStateExit();
//        agent.enabled = false;
//        obstacle.enabled = true;
//        agent.avoidancePriority = 0;
//    }
//}

