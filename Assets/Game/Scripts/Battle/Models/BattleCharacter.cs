using System;
using System.Collections;
using System.Collections.Generic;

public class BattleCharacter
{
    public Character BaseCharacter { get; private set; }

    public int BattleCharacterId { get; set; }

    public Const.Team Team { get; private set; }

    public double CurrentHp { get; set; }

    public double CurrentMp { get; set; }

    public double AtbPoints { get; private set; }

    public double MaxHp { get { return this.BaseCharacter.MaxHp; } }

    public double MaxMp { get { return this.BaseCharacter.MaxMp; } }

    public bool IsDead { get { return this.CurrentHp <= 0d; } }

    public float HpPercentage { get { return (float)(this.CurrentHp / this.MaxHp); } }

    public float MpPercentage { get { return (float)(this.CurrentMp / this.MaxMp); } }

    public char Postfix { get; set; }

    public List<MapPosition> OccupiedMapPositions { get; set; }

    public Skill SelectedSkill { get; set; }

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

    public int TicksTilActionReady
    {
        get
        {
            double pointsRequired = Math.Max(0d, (double)Const.ActionReadyAtbPoints - this.AtbPoints);
            return Convert.ToInt32(Math.Ceiling(pointsRequired / this.BaseCharacter.Agility));
        }
    }

    public BattleCharacter(Character baseCharacter, Const.Team team)
    {
        this.BaseCharacter = baseCharacter;
        this.Team = team;
        this.CurrentHp = this.BaseCharacter.MaxHp;
        this.CurrentMp = this.BaseCharacter.MaxMp;
    }

    public void Tick(int ticks)
    {
        this.AtbPoints += this.BaseCharacter.Agility * ticks;
    }

    public void FinishAction()
    {
        this.SelectedSkill = null;
        this.ConsumeAtbPoints(Const.ActionReadyAtbPoints);
    }

    public void ConsumeAtbPoints(int consumedPoints)
    {
        this.AtbPoints -= consumedPoints;
    }
}
