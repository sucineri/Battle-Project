using UnityEngine;
using System.Collections;

public class BattleCharacter
{
    public Character BaseCharacter { get; private set; }

    public Const.Team Team { get; private set; }

    public double CurrentHp { get; set; }

    public double CurrentMp { get; set; }

    public double TurnOrderWeight { get; set; }

    public double MaxHp { get { return this.BaseCharacter.MaxHp; } }

    public double MaxMp { get { return this.BaseCharacter.MaxMp; } }

    public bool IsDead { get { return this.CurrentHp <= 0d; } }

    public float HpPercentage { get { return (float)(this.CurrentHp / this.MaxHp); } }

    public float MpPercentage { get { return (float)(this.CurrentMp / this.MaxMp); } }

    public char Postfix { get; set; }

    public MapPosition OccupiedMapPositions { get; set; }

    public Skill SelectedSkill { get; set; }

    public string Name
    {
        get
        { 
            return this.BaseCharacter.Name + " " + Postfix;
        }
    }

    protected BattleCharacter()
    {
    }

    public BattleCharacter(Character baseCharacter, Const.Team team)
    {
        this.BaseCharacter = baseCharacter;
        this.Team = team;
        this.CurrentHp = this.BaseCharacter.MaxHp;
        this.CurrentMp = this.BaseCharacter.MaxMp;
    }
}
