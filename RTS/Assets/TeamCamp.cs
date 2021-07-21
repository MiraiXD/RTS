using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamCamp : MonoBehaviour
{
    //public UnitSpawner barracks;
    //public UnitSpawner academy;

    private Team team;

    private List<BattleUnit> units;
    private List<BattleUnit> selectedUnits;
    public int units_Count => units.Count;
    public int selectedUnits_Count => selectedUnits.Count;


    public Transform target;

    public void Init(Team team)
    {
        this.team = team;

        units = new List<BattleUnit>();
        selectedUnits = new List<BattleUnit>();
    }

    public void TrainNewUnit(UnitType unitType)
    {
        GameObject unitGO = UnitManager.CreateBattleUnit(unitType);
        BattleUnit battleUnit = unitGO.GetComponent<BattleUnit>();

        Vector3 unitStartPosition = transform.position;
        battleUnit.transform.position = GameController.navGrid.GetCellCenterWorld(GameController.navGrid.WorldToCell(unitStartPosition));
        battleUnit.Init(team, GameController.navGrid, unitStartPosition);
                
        battleUnit.onUnitSelected += OnUnitSelected;
        battleUnit.onUnitDied += OnUnitDied;

        units.Add(battleUnit);        
    }
    //public Vector3 GetPositionAround(float radius, float angle)//float radiusIncrement, float angleIncrement, int index)
    //{
    //    //float angle = index * angleIncrement;
    //    //float currentRadius = radius + radiusIncrement * Mathf.Floor(angle / 360f);
    //    //float currentAngle = angle % 360f;

    //    float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
    //    float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

    //    return new Vector3(x, 0f, y);
    //}
    //public void FollowTargetSelectedUnits(Transform target)
    //{
    //    foreach (var unit in selectedUnits)
    //    {
    //        unit.FollowTarget(target, unit.AttackRange);
    //    }
    //    //float currentRadius = 1.2f;
    //    //float currentAngle = 0f;

    //    //foreach (var unit in selectedUnits)
    //    //{
    //    //    unit.FollowTarget(target, GetPositionAround(unit.AttackRange, currentAngle));
    //    //    currentAngle += 50f - currentRadius * 5f;
    //    //    if (currentAngle >= 360f)
    //    //    {
    //    //        currentAngle = currentAngle % 360f;
    //    //        currentRadius += 1.2f;
    //    //    }
    //    //}
    //}    
    //public void MoveSelectedUnits(Vector3 position)
    //{
    //    //selectedUnits.Sort(delegate (BattleUnit u1, BattleUnit u2)
    //    //{
    //    //    return (u1.transform.position - position).sqrMagnitude.CompareTo((u2.transform.position - position).sqrMagnitude);
    //    //});
    //    //StartCoroutine(d(position));
    //    //int index = 0;
    //    //float stoppingDistance = 0f;
    //    //foreach (var unit in selectedUnits)
    //    //{
    //    //    if (index > 0) stoppingDistance = 1f;
    //    //    else if (index > 4) stoppingDistance = 2f;
    //    //    else if (index > 8) stoppingDistance = 3f;
    //    //    else if (index > 12) stoppingDistance = 4f;
    //    //    else if (index > 16) stoppingDistance = 5f;
    //    //    unit.MoveTo(position, stoppingDistance);
    //    //    index++;
    //    //}

    //    float currentRadius = 0f;
    //    float currentAngle = 360f;

    //    foreach (var unit in selectedUnits)
    //    {
    //        unit.MoveTo(position + GetPositionAround(currentRadius, currentAngle));
    //        currentAngle += 50f - currentRadius * 5f;
    //        if (currentAngle >= 360f)
    //        {
    //            currentAngle = currentAngle % 360f;
    //            currentRadius += 1.2f;
    //        }
    //    }
    //}
    public void MoveSelectedUnits(Vector3 position)
    {       
        foreach (var unit in selectedUnits)
        {
            unit.MoveTo(position); 
        }
    }
    public void DeselectAllUnits()
    {
        var copy = selectedUnits.ToArray();
        foreach (var selectedUnit in copy) selectedUnit.Deselect();
    }
    public void SelectAllUnits()
    {
        DeselectAllUnits();
        foreach (var unit in units) unit.Select();
    }
    private void OnUnitDied(BattleUnit unit)
    {
        units.Remove(unit);
    }
    private void OnUnitSelected(BattleUnit unit, bool isSelected)
    {
        if (!unit.model.Team.CompareTeams(team)) return;

        if (isSelected && !selectedUnits.Contains(unit))
        {
            selectedUnits.Add(unit);
        }
        else if (!isSelected && selectedUnits.Contains(unit))
        {
            selectedUnits.Remove(unit);
        }
    }
}
