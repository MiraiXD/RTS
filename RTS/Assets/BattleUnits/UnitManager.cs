using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private List<BattleUnitData> allUnits;
    private Dictionary<Entities.BattleUnitModel.UnitType, GameObject> prefabs;
    public void Init()
    {
        prefabs = new Dictionary<Entities.BattleUnitModel.UnitType, GameObject>();
        foreach(var data in allUnits)
        {
            prefabs.Add(data.unitType, data.prefab);
        }
    }

    public BattleUnitController CreateUnit(Entities.BattleUnitModel model, Vector3 position, Quaternion rotation)
    {
        if (prefabs.TryGetValue(model.unitType, out GameObject prefab))
        {
            var unitGO = Instantiate(prefab);
            unitGO.transform.parent = transform;
            unitGO.transform.position = position;
            unitGO.transform.rotation = rotation;
            var controller = unitGO.GetComponent<BattleUnitController>();
            return controller;
        }
        else { Debug.LogError("No such unit"); return null; }
    }
}
