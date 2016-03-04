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

    public BattleCharacter() {}

    public BattleCharacter(Character baseCharacter, Const.Team team)
    {
        this.BaseCharacter = baseCharacter;
        this.Team = team;
        this.CurrentHp = this.MaxHp;
        this.CurrentMp = this.MaxMp;
        this.Weight = CooldownWeight.GetWeight(this.GetStat(Const.Stats.Agility));
        this.Enmity = new BattleEnmity();
    }

    public double GetStat(Const.Stats stat)
    {
        // TODO: buffs
        return this.BaseCharacter.BaseStats.GetStats(stat);
    }

    public double GetAffinityResistance(Const.Affinities affinity)
    {
        return this.GetResistance((int)affinity);
    }

    public double GetStatusEffectResistance()
    {
        return 0;
    }

    public void Tick(int ticks)
    {
        this.ActionCooldown -= ticks;
    }

    private double GetResistance(int enumValue)
    {
        if (Enum.IsDefined(typeof (Const.Stats), enumValue))
        {
            return this.GetStat((Const.Stats)enumValue);
        }
        return 0d;
    }
}
