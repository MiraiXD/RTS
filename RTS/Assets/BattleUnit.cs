using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KK.StatePattern;
using System;
using KK.NavGrid;
public enum UnitType { Infantry, Knight, WaterMage }
public class BattleUnit : MonoBehaviour, ISelectable, IHighlightable
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavGridAgent agent;
    //[SerializeField] private NavMeshAgent agent;
    //[SerializeField] private NavMeshObstacle obstacle;
    private FSM fsm;

    public BattleUnitModel model;

    private IdleState idleState;
    private MoveState moveState;
    private WalkOutOfSpawnPlaceState walkOutOfBarracksState;
    private FollowTargetState followTargetState;

    private CharacterAttack characterAttack;
    public float AttackSpeed => characterAttack.speed;
    public float AttackRange => characterAttack.range;
    public float AttackDamage => characterAttack.damage;

    public static Dictionary<Animator, BattleUnitModel> models = new Dictionary<Animator, BattleUnitModel>();
    [SerializeField] private SpriteRenderer selectedEffect;
    public Action<BattleUnit, bool> onUnitSelected;
    [SerializeField] private SpriteRenderer highlightedEffect;
    public Action<BattleUnit, bool> onUnitHighlighted;

    public Action<BattleUnit> onUnitDied;

    public void Init(Team team, NavGrid navGrid, Vector3 startPosition)
    {
        model = new BattleUnitModel(team, 100, 5f, 10f, 1f, 1f, 1f);
        UpdateVisuals(model.Team.teamColor);
        fsm = new FSM();

        agent.Init(navGrid, navGrid.WorldToCell( startPosition));

        idleState = new IdleState(animator, fsm);
        moveState = new MoveState(agent, animator, this, fsm);
        walkOutOfBarracksState = new WalkOutOfSpawnPlaceState(agent,  animator, this, fsm);
        followTargetState = new FollowTargetState(agent, animator, this, fsm);

        fsm.defaultState = idleState;

        characterAttack = new FistsAttack(animator, model.AttackSpeedModifier, model.RangeModifier, model.DamageModifier);
        model.WeaponAttackSpeed = characterAttack.weaponSpeed;

        models.Add(animator, model);

        //walkOutOfBarracksState.SetDestination(startPosition);
        //fsm.SetState(walkOutOfBarracksState);
        fsm.SetState(idleState);

    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space)) { if (characterAttack.CanAttack()) { characterAttack.Attack(); } }



        fsm.UpdateFSM(Time.deltaTime);
    }
    public void FollowTarget(Transform target, int stoppingDistance)
    {
        followTargetState.SetTarget(target, stoppingDistance);
        fsm.SetState(followTargetState);
    }
    public void MoveTo(Vector3 destination, int stoppingDistance = 0)
    {
        moveState.SetDestination(destination, stoppingDistance);
        fsm.SetState(moveState);
    }
    public void SetSpeed(float currentSpeed)
    {
        model.CurrentMovementSpeed = currentSpeed;
        agent.speed = model.CurrentMovementSpeed;
    }
    public void ClearCommands()
    {
        fsm.ClearStates();
        fsm.SetState(idleState);
    }

    public bool CanSelect(Team team)
    {
        return model.Team.CompareTeams(team);
    }


    public void SetTeam(Team team)
    {
        model.Team = team;
        UpdateVisuals(team.teamColor);

    }
    private void UpdateVisuals(Color color)
    {
        selectedEffect.color = new Color(color.r, color.g, color.b, selectedEffect.color.a);
        highlightedEffect.color = new Color(color.r, color.g, color.b, highlightedEffect.color.a);
    }
    private bool isSelected, isHighlighted;
    public void Select()
    {
        isSelected = true;
        isHighlighted = false;

        highlightedEffect.enabled = isHighlighted;
        selectedEffect.enabled = isSelected;

        onUnitSelected?.Invoke(this, true);
    }

    public void Deselect()
    {
        isSelected = false;
        isHighlighted = false;

        highlightedEffect.enabled = isHighlighted;
        selectedEffect.enabled = isSelected;

        onUnitSelected?.Invoke(this, false);
    }
    public void HighlightOn()
    {
        if (!isSelected)
        {
            isHighlighted = true;
            highlightedEffect.enabled = isHighlighted;

            onUnitHighlighted?.Invoke(this, true);
        }
    }

    public void HighlightOff()
    {
        isHighlighted = false;
        highlightedEffect.enabled = isHighlighted;

        onUnitHighlighted?.Invoke(this, false);
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using KK.StatePattern;
//using System;
//using KK.NavGrid;
//public enum UnitType { Infantry, Knight, WaterMage }
//public class BattleUnit : MonoBehaviour, ISelectable, IHighlightable
//{
//    [SerializeField] private Animator animator;
//    [SerializeField] private NavGridAgent agent;
//    //[SerializeField] private NavMeshAgent agent;
//    //[SerializeField] private NavMeshObstacle obstacle;
//    private FSM fsm;

//    public BattleUnitModel model;

//    private IdleState idleState;
//    private MoveState moveState;
//    private WalkOutOfSpawnPlaceState walkOutOfBarracksState;
//    private FollowTargetState followTargetState;

//    private CharacterAttack characterAttack;
//    public float AttackSpeed => characterAttack.speed;
//    public float AttackRange => characterAttack.range;
//    public float AttackDamage => characterAttack.damage;

//    public static Dictionary<Animator, BattleUnitModel> models = new Dictionary<Animator, BattleUnitModel>();
//    [SerializeField] private SpriteRenderer selectedEffect;
//    public Action<BattleUnit, bool> onUnitSelected;
//    [SerializeField] private SpriteRenderer highlightedEffect;
//    public Action<BattleUnit, bool> onUnitHighlighted;

//    public Action<BattleUnit> onUnitDied;

//    public void Init(Team team, Vector3 startPosition)
//    {
//        model = new BattleUnitModel(team, 100, 5f, 10f, 1f, 1f, 1f);
//        UpdateVisuals(model.Team.teamColor);
//        fsm = new FSM();

//        idleState = new IdleState(animator, fsm);
//        moveState = new MoveState(agent,obstacle, animator, this, fsm);
//        walkOutOfBarracksState = new WalkOutOfSpawnPlaceState(agent, obstacle, animator, this, fsm);
//        followTargetState = new FollowTargetState(agent,obstacle, animator, this, fsm);

//        fsm.defaultState = idleState;

//        characterAttack = new FistsAttack(animator, model.AttackSpeedModifier, model.RangeModifier, model.DamageModifier);
//        model.WeaponAttackSpeed = characterAttack.weaponSpeed;

//        models.Add(animator, model);

//        //walkOutOfBarracksState.SetDestination(startPosition);
//        //fsm.SetState(walkOutOfBarracksState);
//        fsm.SetState(idleState);

//    }

//    private void Update()
//    {
//        //if (Input.GetKeyDown(KeyCode.Space)) { if (characterAttack.CanAttack()) { characterAttack.Attack(); } }



//        fsm.UpdateFSM(Time.deltaTime);
//    }
//    public void FollowTarget(Transform target, float stoppingDistance)
//    {
//        followTargetState.SetTarget(target, stoppingDistance);
//        fsm.SetState(followTargetState);
//    }
//    public void MoveTo(Vector3 destination, float stoppingDistance = 0f)
//    {
//        moveState.SetDestination(destination, stoppingDistance);
//        fsm.SetState(moveState);
//    }
//    public void SetSpeed(float currentSpeed)
//    {
//        model.CurrentMovementSpeed = currentSpeed;
//        agent.speed = model.CurrentMovementSpeed;
//    }
//    public void ClearCommands()
//    {
//        fsm.ClearStates();
//        fsm.SetState(idleState);
//    }

//    public bool CanSelect(Team team)
//    {
//        return model.Team.CompareTeams(team);
//    }


//    public void SetTeam(Team team)
//    {
//        model.Team = team;
//        UpdateVisuals(team.teamColor);

//    }
//    private void UpdateVisuals(Color color)
//    {
//        selectedEffect.color = new Color(color.r, color.g, color.b, selectedEffect.color.a);
//        highlightedEffect.color = new Color(color.r, color.g, color.b, highlightedEffect.color.a);
//    }
//    private bool isSelected, isHighlighted;
//    public void Select()
//    {
//        isSelected = true;
//        isHighlighted = false;

//        highlightedEffect.enabled = isHighlighted;
//        selectedEffect.enabled = isSelected;

//        onUnitSelected?.Invoke(this, true);
//    }

//    public void Deselect()
//    {
//        isSelected = false;
//        isHighlighted = false;

//        highlightedEffect.enabled = isHighlighted;
//        selectedEffect.enabled = isSelected;

//        onUnitSelected?.Invoke(this, false);
//    }
//    public void HighlightOn()
//    {
//        if (!isSelected)
//        {
//            isHighlighted = true;
//            highlightedEffect.enabled = isHighlighted;

//            onUnitHighlighted?.Invoke(this, true);
//        }            
//    }

//    public void HighlightOff()
//    {
//        isHighlighted = false;
//        highlightedEffect.enabled = isHighlighted;

//        onUnitHighlighted?.Invoke(this, false);
//    }
//}
