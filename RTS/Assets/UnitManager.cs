using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private List<BattleUnitData> spawnableUnits = new List<BattleUnitData>();
    private static Dictionary<UnitType, BattleUnitData> units;

    public static GameObject CreateBattleUnit(UnitType unitType)
    {
        if (!units.TryGetValue(unitType, out BattleUnitData unitData)) { Debug.LogError("No such unit!"); return null; }

        GameObject unitGO = Instantiate(unitData.prefab);
        
        return unitGO;
    }

    public void Init()
    {
        units = new Dictionary<UnitType, BattleUnitData>();
        foreach (var data in spawnableUnits)
        {
            if (units.ContainsKey(data.unitType)) Debug.LogError("Such unit already exists in the dictionary: " + data.unitType);
            else
            {
                units.Add(data.unitType, data);
            }
        }
    }
}
