using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

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
            var baseStat = this.GetBaseStat(stat);

            var statModifiers = new Dictionary<Const.ModifierType, double>();
            statModifiers.Add(modifier.StatModifier.Type, modifier.StatModifier.Magnitude);

            var modifiedValue = this.CalculateStat(baseStat, statModifiers);
            stats.SetStat(stat, modifiedValue);
        }
        this.ModifierStats = stats;
    }

    public double GetStatWithModifiers(Const.Stats stat, List<StatModifier> modifiers)
    {
        var baseStat = this.GetStat(stat);
        var statModifiers = this.GetModifiersForStat(modifiers, stat);
        return this.CalculateStat(baseStat, statModifiers);
    }

    private double CalculateStat(double baseValue, Dictionary<Const.ModifierType, double> statModifiers)
    {
        if (statModifiers.Count == 0)
        {
            return baseValue;
        }

        var value = baseValue;

        if (statModifiers.ContainsKey(Const.ModifierType.Absolute))
        {
            return statModifiers[Const.ModifierType.Absolute];
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

            return value;
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
