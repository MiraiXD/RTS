using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="BattleUnit")]
public class BattleUnitData : ScriptableObject
{
    public Entities.BattleUnitModel.UnitType unitType;    
    public GameObject prefab;
    public Sprite portrait;
}
