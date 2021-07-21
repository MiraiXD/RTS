using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
public class UnitSpawner : MonoBehaviour, ISelectable, IHighlightable
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform unitsParent;

    [SerializeField] private SpriteRenderer selectedEffect;
    [SerializeField] private SpriteRenderer highlightedEffect;

    private Team team;

    [SerializeField] private List<BattleUnitData> spawnableUnits = new List<BattleUnitData>();
    private Dictionary<UnitType, BattleUnitData> units;

    public void Init(Team team)
    {
        this.team = team;

        isSelected = false;
        isHighlighted = false;

        units = new Dictionary<UnitType, BattleUnitData>();
        foreach(var data in spawnableUnits)
        {
            if (units.ContainsKey(data.unitType)) Debug.LogError("Such unit already exists in the dictionary");
            else
            {
                units.Add(data.unitType, data);
            }
        }
    }
    private GameObject CreateBattleUnit(UnitType unitType)
    {
        if (!units.TryGetValue(unitType, out BattleUnitData unitData)) { Debug.LogError("No such unit!"); return null; }

        GameObject unitGO = Instantiate(unitData.prefab, unitsParent);
        unitGO.transform.position = spawnPosition.position;
        unitGO.transform.rotation = spawnPosition.rotation;

        return unitGO;
    }

    //public BattleUnit TrainNewUnit(UnitType unitType, Team team)
    //{
    //    GameObject unitGO = CreateBattleUnit(unitType);
    //    if (unitGO == null) return null;

    //    unitGO.GetComponent<NavMeshAgent>().enabled = false;
    //    BattleUnit unit = unitGO.GetComponent<BattleUnit>();
    //    unit.Init(team, startPosition.position);
    //    return unit;
    //}
    
    public bool CanSelect(Team team)
    {
        return true;
    }
   
    private bool isSelected, isHighlighted;
    public void Select()
    {        
        isSelected = true;
        isHighlighted = false;

        highlightedEffect.enabled = isHighlighted;
        selectedEffect.enabled = isSelected;
    }

    public void Deselect()
    {
        isSelected = false;
        isHighlighted = false;

        highlightedEffect.enabled = isHighlighted;
        selectedEffect.enabled = isSelected;
    }
    public void HighlightOn()
    {
        if (!isSelected)
        {
            isHighlighted = true;
            highlightedEffect.enabled = isHighlighted;
        }
    }

    public void HighlightOff()
    {
        isHighlighted = false;
        highlightedEffect.enabled = isHighlighted;
    }
}
