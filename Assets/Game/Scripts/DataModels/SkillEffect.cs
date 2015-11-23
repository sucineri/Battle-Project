using UnityEngine;
using System.Collections;

public class SkillEffect 
{
	public BasicStats StatsModifiers { get; set; }
	public string AssetPath { get; set; }

	protected SkillEffect ()
	{
		// no public default constructor
	}

	public static SkillEffect MeleeAttackEffect ()
	{
		var skillEffect = new SkillEffect ();
		skillEffect.StatsModifiers = new BasicStats (0d, 0d, 1d, 0d, 0d, 0d);
		skillEffect.AssetPath = "Effects/Explosion";
		return skillEffect;
	}

	public static SkillEffect CrossSlashEffect ()
	{
		var skillEffect = new SkillEffect ();
		skillEffect.StatsModifiers = new BasicStats (0d, 0d, 2d, 0d, 0d, 0d);
		skillEffect.AssetPath = "Effects/Explosion";
		return skillEffect;
	}
}
