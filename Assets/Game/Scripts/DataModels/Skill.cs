using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill 
{
	public List<SkillEffect> Effects = new List<SkillEffect>();

	public int NumberOfAttacks { get; set; }

	public Targetting SkillTarget { get; set; }

	public string PrefabPath { get; set; }

	protected Skill()
	{
		// no public default constructor
	}



	public static Skill MeleeAttack ()
	{
		var skill = new Skill ();
		skill.Effects.Add (SkillEffect.MeleeAttackEffect ());
		skill.NumberOfAttacks = 1;
		skill.PrefabPath = "Skills/MeleeAttack";
		skill.SkillTarget = Targetting.SingleOpponentTarget ();
		return skill;
	}
}
