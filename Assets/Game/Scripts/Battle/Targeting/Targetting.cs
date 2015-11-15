using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Targetting 
{
	public Const.SkillTargetGroup TargetGroup { get; set; }

	public Const.SkillTargetType TargetType { get; set; }

	public List<Cordinate> Pattern { get; set; }

	protected Targetting ()
	{
		// no public default constructor
	}

	public static Targetting SingleOpponentTarget ()
	{
		var pattern = new Targetting ();
		pattern.TargetGroup = Const.SkillTargetGroup.Opponent;
		pattern.TargetType = Const.SkillTargetType.Unit;

		pattern.Pattern = new List<Cordinate> ();
		pattern.Pattern.Add (new Cordinate (0, 0));
		return pattern;
	}
}
