using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="BattleUnit")]
public class BattleUnitData : ScriptableObject
{
    public UnitType unitType;
    public int maxHealth;
    public float movementSpeed;
    public float attackSpeedModifier = 1f;
    public float rangeModifier = 1f;
    public float damageModifier = 1f;
    public GameObject prefab;
    public Sprite portrait;
}
