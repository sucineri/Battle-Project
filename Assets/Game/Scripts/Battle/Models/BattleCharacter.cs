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

    public Dictionary<StatusEffect.Type, CharacterStatusEffectStatModifier> StatusEffectStatModifiers { get; private set; }

    public Dictionary<StatusEffect.Type, CharacterStatusEffectStatModifier> SpecialStateModifiers { get; private set; }

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

    public bool IsPoisoned
    {
        get
        {
            return this.HasSpecialState(StatusEffect.Type.Poison);
        }
    }

    public BattleCharacter()
    {
        this.ModifierStats = new CharacterStats();
        this.StatusEffectStatModifiers = new Dictionary<StatusEffect.Type, CharacterStatusEffectStatModifier>();
        this.SpecialStateModifiers = new Dictionary<StatusEffect.Type, CharacterStatusEffectStatModifier>();
        this.RecalculateModifiedStats();
    }

    public BattleCharacter(Character baseCharacter, Const.Team team)
    {
        this.BaseCharacter = baseCharacter;
        this.StatusEffectStatModifiers = new Dictionary<StatusEffect.Type, CharacterStatusEffectStatModifier>();
        this.SpecialStateModifiers = new Dictionary<StatusEffect.Type, CharacterStatusEffectStatModifier>();
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

    public double GetStatusEffectResistance(StatusEffect.Type statusEffectType)
    {
        return this.GetResistance((int)statusEffectType);
    }

    public void Tick(int ticks)
    {
        this.ActionCooldown -= ticks;
    }

    public void ApplyStatusEffect(StatusEffect statusEffect)
    {
        if (this.CanApplyStatusEffect(statusEffect))
        {
            var type = statusEffect.StatusEffectType;
            var newMod = new CharacterStatusEffectStatModifier(statusEffect);
            var dictionary = statusEffect.StatusEffectCategory == StatusEffect.Category.CharacterStatChange ? this.StatusEffectStatModifiers : this.SpecialStateModifiers;

            if (dictionary.ContainsKey(type))
            {
                dictionary[type] = newMod;
            }
            else
            {
                dictionary.Add(type, newMod);
            }
            this.RecalculateModifiedStats();
        }
    }

    public bool CanApplyStatusEffect(StatusEffect statusEffect)
    {
        var type = statusEffect.StatusEffectType;
        var newMod = new CharacterStatusEffectStatModifier(statusEffect);

        var dictionary = statusEffect.StatusEffectCategory == StatusEffect.Category.CharacterStatChange ? this.StatusEffectStatModifiers : this.SpecialStateModifiers;

        if (dictionary.ContainsKey(type))
        {
            return newMod.Rank >= dictionary[type].Rank;
        }

        return true;
    }

    public void UpateStatusEffectTurns()
    {
        var hasExpiredStatEffect = this.UpdateStatusEffectTurns(this.StatusEffectStatModifiers);
        var hasExpiredSpecialEffect = this.UpdateStatusEffectTurns(this.SpecialStateModifiers);
        if (hasExpiredStatEffect || hasExpiredSpecialEffect)
        {
            this.RecalculateModifiedStats();
        }
    }

    private bool UpdateStatusEffectTurns(Dictionary<StatusEffect.Type, CharacterStatusEffectStatModifier> effects)
    {
        var expiredEffects = new List<StatusEffect.Type>();
        foreach (var kv in effects)
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
                effects.Remove(expiredEffect);
            }
        }
        return expiredEffects.Count > 0;
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

    public CharacterStatusEffectStatModifier GetSpecialStateModifier(StatusEffect.Type type)
    {
        if (this.SpecialStateModifiers.ContainsKey(type))
        {
            return this.SpecialStateModifiers[type];
        }
        return null;
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

    private bool HasSpecialState(StatusEffect.Type type)
    {
        return this.SpecialStateModifiers.ContainsKey(type);
    }
}
