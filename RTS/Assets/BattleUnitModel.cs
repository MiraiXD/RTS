using System;

public class BattleUnitModel
{
    public Team Team { get { return team; }set { team = value; onTeamChanged?.Invoke(value); } }
    public int MaxHealth { get { return maxHealth; } set { maxHealth = value; onMaxHealthChanged?.Invoke(CurrentHealth, MaxHealth); } }
    public int CurrentHealth { get { return currentHealth; } set { currentHealth = value; onCurrentHealthChanged?.Invoke(CurrentHealth, MaxHealth); } }
    public float MaxMovementSpeed { get { return maxMovementSpeed; } set { maxMovementSpeed = value; onMaxMovementSpeedChanged?.Invoke(CurrentMovementSpeed, DefaultMovementSpeed, MaxMovementSpeed); } }
    public float DefaultMovementSpeed { get { return defaultMovementSpeed; } set { defaultMovementSpeed = value; onDefaultMovementSpeedChanged?.Invoke(CurrentMovementSpeed, DefaultMovementSpeed, MaxMovementSpeed); } }
    public float CurrentMovementSpeed
    {
        get { return currentMovementSpeed; }
        set
        {

            if (value > MaxMovementSpeed) currentMovementSpeed = MaxMovementSpeed;
            else if (value < 0f) currentMovementSpeed = 0f;
            else currentMovementSpeed = value;

            onCurrentMovementSpeedChanged?.Invoke(CurrentMovementSpeed, DefaultMovementSpeed, MaxMovementSpeed);
        }
    }
    public float WeaponAttackSpeed { get { return weaponAttackSpeed; } set { weaponAttackSpeed = value; onWeaponAttackSpeedChanged?.Invoke(weaponAttackSpeed); } }
    public float AttackSpeedModifier { get { return attackSpeedModifier; } set { attackSpeedModifier = value; onAttackSpeedModifierChanged?.Invoke(AttackSpeedModifier); } }
    public float RangeModifier { get { return rangeModifier; } set { rangeModifier = value; onRangeModifierChanged?.Invoke(RangeModifier); } }
    public float DamageModifier { get { return damageModifier; } set { damageModifier = value; onDamageModifierChanged?.Invoke(DamageModifier); } }

    public Action<Team> onTeamChanged;

    public Action<int, int> onMaxHealthChanged;
    public Action<int, int> onCurrentHealthChanged;

    public Action<float, float, float> onMaxMovementSpeedChanged;
    public Action<float, float, float> onDefaultMovementSpeedChanged;
    public Action<float, float, float> onCurrentMovementSpeedChanged;

    public Action<float> onWeaponAttackSpeedChanged;

    public Action<float> onAttackSpeedModifierChanged;
    public Action<float> onRangeModifierChanged;
    public Action<float> onDamageModifierChanged;

    private Team team;

    private int maxHealth;
    private int currentHealth;

    private float maxMovementSpeed;
    private float defaultMovementSpeed;
    private float currentMovementSpeed;

    private float weaponAttackSpeed;

    private float attackSpeedModifier;
    private float rangeModifier;
    private float damageModifier;

    public BattleUnitModel(Team team, int maxHealth, float defaultMovementSpeed, float maxMovementSpeed, float attackSpeedModifier, float rangeModifier, float damageModifier)
    {
        Team = team;

        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;

        DefaultMovementSpeed = defaultMovementSpeed;
        MaxMovementSpeed = maxMovementSpeed;
        CurrentMovementSpeed = DefaultMovementSpeed;

        WeaponAttackSpeed = 0f; // no weapon

        AttackSpeedModifier = attackSpeedModifier;
        RangeModifier = rangeModifier;
        DamageModifier = damageModifier;
    }

}
