using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public enum CharacterStatBuffState
{
    UnBuffed,
    Debuffed,
    Buffed
}

public class BattleCharacter
{
    public Character BaseCharacter { get; private set; }

    public int BattleCharacterId { get; set; }

    public Const.Team Team { get; private set; }

    public int Weight { get; private set; }

    public double CurrentHp { get; set; }

    public double CurrentMp { get; set; }

    public int ActionCooldown { get; set; }

    public double MaxHp { get { return this.GetStat(Const.Stats.MaxHp); } }

    public double MaxMp { get { return this.GetStat(Const.Stats.MaxMp); } }

    public bool IsDead { get { return this.CurrentHp <= 0d; } }

    public float HpPercentage { get { return (float)(this.CurrentHp / this.MaxHp); } }

    public float MpPercentage { get { return (float)(this.CurrentMp / this.MaxMp); } }

    public char Postfix { get; set; }

    public List<MapPosition> OccupiedMapPositions { get; set; }

    public Skill SelectedSkill { get; set; }

    public BattleEnmity Enmity { get; set; }

    public MapPosition SkillTargetPosition { get; set; }

    public Dictionary<Const.StatusEffectTypes, CharacterStatusEffectStatModifier> StatusEffectStatModifiers { get; set; }

    public CharacterStats ModifierStats { get; set; }

    public string Name
    {
        get
        { 
            return this.BaseCharacter.Name + " " + Postfix;
        }
    }

    public MapPosition BasePosition
    {
        get
        {
            return this.OccupiedMapPositions[0];
        } 
    }

    public BattleCharacter()
    {
        this.ModifierStats = new CharacterStats();
        this.StatusEffectStatModifiers = new Dictionary<Const.StatusEffectTypes, CharacterStatusEffectStatModifier>();
    }

    public BattleCharacter(Character baseCharacter, Const.Team team)
    {
        this.BaseCharacter = baseCharacter;
        this.StatusEffectStatModifiers = new Dictionary<Const.StatusEffectTypes, CharacterStatusEffectStatModifier>();
        this.RecalculateModifiedStats();

        this.Team = team;
        this.CurrentHp = this.MaxHp;
        this.CurrentMp = this.MaxMp;
        this.Weight = CooldownWeight.GetWeight(this.GetStat(Const.Stats.Agility));
        this.Enmity = new BattleEnmity();
    }

    public double GetBaseStat(Const.Stats stat)
    {
        return this.BaseCharacter.BaseStats.GetStat(stat);
    }

    public double GetStat(Const.Stats stat)
    {
        return this.ModifierStats.GetStat(stat);
    }

    public double GetAffinityResistance(Const.Affinities affinity)
    {
        return this.GetResistance((int)affinity);
    }

    public double GetStatusEffectResistance(Const.StatusEffectTypes statusEffectType)
    {
        return this.GetResistance((int)statusEffectType);
    }

    public void Tick(int ticks)
    {
        this.ActionCooldown -= ticks;
    }

    public bool ApplyStatusEffect(StatusEffect statusEffect)
    {
        var type = statusEffect.StatusEffectType;
        var newMod = new CharacterStatusEffectStatModifier(statusEffect);
        if (this.StatusEffectStatModifiers.ContainsKey(type))
        {
            if (newMod.Rank < this.StatusEffectStatModifiers[type].Rank)
            {
                return false;
            }
            else
            {
                this.StatusEffectStatModifiers[type] = newMod;
                this.RecalculateModifiedStats();
                return true;
            }
        }
        else
        {
            this.StatusEffectStatModifiers.Add(type, newMod);
            this.RecalculateModifiedStats();
            return true;
        }
    }

    public void UpateStatusEffectTurns()
    {
        var expiredEffects = new List<Const.StatusEffectTypes>();
        foreach (var kv in this.StatusEffectStatModifiers)
        {
            kv.Value.UpdateTurn();
            if (kv.Value.Expired)
            {
                expiredEffects.Add(kv.Key);
            }
        }

        if (expiredEffects.Count > 0)
        {
            foreach (var expiredEffect in expiredEffects)
            {
                this.StatusEffectStatModifiers.Remove(expiredEffect);
            }
            this.RecalculateModifiedStats();
        }
    }

    public CharacterStatBuffState GetStatBuffState(Const.Stats stat)
    {
        var baseValue = this.GetBaseStat(stat);
        var actualValue = this.GetStat(stat);

        if (actualValue > baseValue)
        {
            return CharacterStatBuffState.Buffed;
        }
        else if (actualValue < baseValue)
        {
            return CharacterStatBuffState.Debuffed;
        }

        return CharacterStatBuffState.UnBuffed;
    }

    private double GetResistance(int enumValue)
    {
        if (Enum.IsDefined(typeof (Const.Stats), enumValue))
        {
            return this.GetStat((Const.Stats)enumValue);
        }
        return 0d;
    }

    private void RecalculateModifiedStats()
    {
        var stats = this.BaseCharacter.BaseStats.Clone();
        foreach (var modifier in this.StatusEffectStatModifiers.Values)
        {
            var stat = modifier.StatModifier.Stat;
            var baseStatValue = this.GetBaseStat(stat);

            var statModifiers = new Dictionary<Const.ModifierType, double>();
            statModifiers.Add(modifier.StatModifier.Type, modifier.StatModifier.Magnitude);

            var modifiedValue = this.CalculateStat(stat, baseStatValue, statModifiers);
            stats.SetStat(stat, modifiedValue.Value);
        }
        this.ModifierStats = stats;
    }

    public ModifiedStat GetStatWithModifiers(Const.Stats stat, List<StatModifier> modifiers)
    {
        var baseStat = this.GetStat(stat);
        var statModifiers = this.GetModifiersForStat(modifiers, stat);
        return this.CalculateStat(stat, baseStat, statModifiers);
    }

    private ModifiedStat CalculateStat(Const.Stats stat, double baseValue, Dictionary<Const.ModifierType, double> statModifiers)
    {
        if (statModifiers.Count == 0)
        {
            return new ModifiedStat(stat, baseValue, false);
        }

        var value = baseValue;

        if (statModifiers.ContainsKey(Const.ModifierType.Absolute))
        {
            return new ModifiedStat(stat, statModifiers[Const.ModifierType.Absolute], true);
        }
        else
        {
            if (statModifiers.ContainsKey(Const.ModifierType.Multiply))
            {
                value *= statModifiers[Const.ModifierType.Multiply];
            }

            if (statModifiers.ContainsKey(Const.ModifierType.Addition))
            {
                value += statModifiers[Const.ModifierType.Addition];
            }

            return new ModifiedStat(stat, value, false);
        }
    }

    private Dictionary<Const.ModifierType, double> GetModifiersForStat(List<StatModifier> modifiers, Const.Stats stat)
    {
        var dict = new Dictionary<Const.ModifierType, double>();
        foreach (var bonus in modifiers)
        {       
            if (bonus.Stat == stat)
            {
                var type = bonus.Type;  
                if(dict.ContainsKey(type))
                {
                    dict[type] += bonus.Magnitude;
                }
                else
                {
                    dict.Add(type, bonus.Magnitude);
                }
            }
        }
        return dict;
    }
}
