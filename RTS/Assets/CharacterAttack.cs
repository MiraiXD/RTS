using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//public enum AttackType { RPG, Trigger }
public abstract class CharacterAttack 
{
    // public AttackType attackType;
    //public IDamagable target { get; set; }
    //protected int comboCounter = 1;
    protected Animator animator;
    
    protected int currentComboCounter;
    protected int comboCounterHash = Animator.StringToHash("CurrentComboCounter");
    protected int attackHash = Animator.StringToHash("Attack");
    protected int attackSpeedHash = Animator.StringToHash("AttackSpeed");
    //protected Equipment equipment;
    public abstract float weaponSpeed { get; }
    public abstract float weaponRange { get; }
    public abstract float weaponDamage { get; }
    public abstract int comboCount { get; }

    public float speed => weaponSpeed * speedModifier;
    public float range => weaponRange * rangeModifier;
    public float damage => weaponDamage * damageModifier;
    public float speedModifier
    {
        get { return _speedModifier; }
        set
        {

            if (value > _speedModifier) //increasing attacks per second
            {
                float difference = 1f / (weaponSpeed * _speedModifier) - 1f / (weaponSpeed * value);
                if (pauseBetweenAttacks >= difference)
                {
                    pauseBetweenAttacks -= difference;
                }
                else
                {
                    difference -= pauseBetweenAttacks;
                    pauseBetweenAttacks = 0f;                    
                    animationSpeed = 1f / (1f / animationSpeed - difference);
                }       
            }
            else // decreasing
            {                
                if (animationSpeed * value / _speedModifier >= 1f)
                {
                    animationSpeed *= value / _speedModifier;
                }
                else
                {
                    float difference = 1f / (weaponSpeed * value) - 1f / (weaponSpeed * _speedModifier);
                    difference -= 1f - 1f / animationSpeed;
                    animationSpeed = 1f;
                    pauseBetweenAttacks = difference;
                }
            }

            _speedModifier = value;
            animator.SetFloat(attackSpeedHash, animationSpeed);
        }
    }
    public float rangeModifier { get; set; } = 1f;
    public float damageModifier { get; set; } = 1f;
    private float _speedModifier = 1f;
    private float animationSpeed = 1f;
    private float pauseBetweenAttacks = 0f;
    private float lastAttackTime = -100f;
    public CharacterAttack(Animator animator, float speedModifier, float rangeModifier, float damageModifier)
    {
        this.animator = animator;
        this.speedModifier = speedModifier;
        this.rangeModifier = rangeModifier;
        this.damageModifier = damageModifier;        
        currentComboCounter = 0;
    }
    public bool CanAttack()
    {
        return (Time.time - lastAttackTime >= 1f / speed + pauseBetweenAttacks);
    }
    public virtual void Attack()
    {
        if (Time.time - lastAttackTime >= 1f / speed + pauseBetweenAttacks + 2f) { Debug.Log("Reset combo"); currentComboCounter = 0; }
        Debug.Log(Time.time - lastAttackTime);
        lastAttackTime = Time.time;

        animator.SetInteger(comboCounterHash, currentComboCounter);
        animator.SetTrigger(attackHash);        
        
        currentComboCounter++;
        if (currentComboCounter >= comboCount) currentComboCounter = 0;
        //animator.SetFloat("AttackSpeed", speedModifier);
    }    
}