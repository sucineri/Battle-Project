using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill
{
	public string Name { get; set; }

	public List<SkillEffect> Effects = new List<SkillEffect> ();

	public int NumberOfAttacks { get; set; }

	public Targetting SkillTarget { get; set; }

	public string PrefabPath { get; set; }

	protected Skill ()
	{
		// no public default constructor
	}



	public static Skill MeleeAttack ()
	{
		var skill = new Skill ();
		skill.Name = "Attack";
		skill.Effects.Add (SkillEffect.MeleeAttackEffect ());
		skill.NumberOfAttacks = 1;
		skill.PrefabPath = "Skills/MeleeAttack";
		skill.SkillTarget = Targetting.SingleOpponentTarget ();
		return skill;
	}

	public static Skill CrossSlash ()
	{
		var skill = new Skill ();
		skill.Name = "Cross Slash";
		skill.Effects.Add (SkillEffect.CrossSlashEffect ());
		skill.NumberOfAttacks = 1;
		skill.PrefabPath = "Skills/CrossSlash";
		skill.SkillTarget = Targetting.CrossOpponentTarget ();
		return skill;
	}
}
