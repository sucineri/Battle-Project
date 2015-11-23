using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterStats 
{
	public BasicStats BasicStats { get; private set; }

    public string Name { get; private set; }
	public string PortraitPath { get; private set; }
	public string ModelPath { get; private set; }
	public float AttackDistance { get; private set; }
	public float SizeOffset { get; private set; }
	public double CurrentHp { get; set; }
	public double CurrentMp { get; set; }

	public double MaxHp { get { return this.BasicStats.GetStats (Const.BasicStats.MaxHp); } }
	public double MaxMp { get { return this.BasicStats.GetStats (Const.BasicStats.MaxMp); } }
	public double Attack { get { return this.BasicStats.GetStats (Const.BasicStats.Attack); } }
	public double Defense { get { return this.BasicStats.GetStats (Const.BasicStats.Defense); } }
	public double Wisdom{ get { return this.BasicStats.GetStats (Const.BasicStats.Wisdom); } }
	public double Agility { get { return this.BasicStats.GetStats (Const.BasicStats.Agility); } }

	public float HpPercentage { get { return (float)(this.CurrentHp / this.MaxHp); } }
	public float MpPercentage { get { return (float)(this.CurrentMp / this.MaxMp); } }

	public List<Skill> Skills = new List<Skill>();

    protected CharacterStats()
    {
    }

    public static CharacterStats Fighter()
    {
        var random = new System.Random();
        var character = new CharacterStats();
		var maxHp = random.Next(500, 700);
		var maxMp = random.Next (100, 150);
		var atk = random.Next(200, 300);
		var def = random.Next(100, 150);
		var agi = random.Next(50, 100);
		var wis = random.Next(30, 80);
        character.Name = "Fighter";
		character.BasicStats = new BasicStats (maxHp, maxMp, atk, def, agi, wis);
		character.CurrentHp = character.MaxHp;
		character.CurrentMp = character.MaxMp;
        character.PortraitPath = "Fighter/portrait";
        character.ModelPath = "Fighter/model";
        character.AttackDistance = 1.5f;
        character.SizeOffset = 1.5f;
		character.Skills.Add (Skill.MeleeAttack ());
		character.Skills.Add (Skill.CrossSlash ());
        return character;
    }

    public static CharacterStats Slime()
    {
		var random = new System.Random();
		var character = new CharacterStats();
		var maxHp = random.Next(500, 700);
		var maxMp = random.Next (100, 150);
		var atk = random.Next(200, 300);
		var def = random.Next(100, 150);
		var agi = random.Next(50, 100);
		var wis = random.Next(30, 80);
		character.Name = "Slime";
		character.BasicStats = new BasicStats (maxHp, maxMp, atk, def, agi, wis);
		character.CurrentHp = character.MaxHp;
		character.CurrentMp = character.MaxMp;
		character.PortraitPath = "Slime/portrait";
		character.ModelPath = "Slime/model";
		character.AttackDistance = 2.5f;
		character.SizeOffset = 1.5f;
		character.Skills.Add (Skill.MeleeAttack ());
		return character;
    }
}
