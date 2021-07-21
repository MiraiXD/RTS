using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Barracks : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform unitsParent;


    [SerializeField] private SerializableDictionary<UnitType, GameObject> allUnits;
    private GameObject CreateBattleUnit(UnitType unitType)
    {
        if (!allUnits.TryGetValue(unitType, out GameObject unitPrefab)) { Debug.LogError("No such unit!"); return null; }

        GameObject unitGO = Instantiate(unitPrefab, unitsParent);
        unitGO.transform.position = spawnPosition.position;
        unitGO.transform.rotation = spawnPosition.rotation;

        return unitGO;
    }

    //public BattleUnit TrainNewUnit(UnitType unitType)
    //{
    //    GameObject unitGO = CreateBattleUnit(unitType);
    //    unitGO.GetComponent<NavMeshAgent>().enabled = false;
    //    BattleUnit unit = unitGO.GetComponent<BattleUnit>();
    //    unit.Init(new Team(Color.red), startPosition.position);
    //    return unit;
    //}

    private void Start()
    {
        //TrainNewUnit(UnitType.Infantry);        
    }
}
