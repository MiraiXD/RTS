using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistsAttack : CharacterAttack
{
    public FistsAttack(Animator animator, float speedModifier, float rangeModifier, float damageModifier) : base(animator, speedModifier, rangeModifier, damageModifier)
    {
    }

    public override float weaponSpeed => .67f;
    public override float weaponRange => 1f;
    public override float weaponDamage => 1f;
    public override int comboCount => 3;
}
