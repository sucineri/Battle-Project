using System;
using System.Collections;

public class DamageLogic  
{
	public static double GetNormalAttackDamage(BattleCharacter attacker, BattleCharacter defender, SkillEffect effect)
	{
		var attack = attacker.BaseCharacter.Attack * effect.StatsModifiers.GetStats (Const.BasicStats.Attack);
		return Math.Floor (attack - defender.BaseCharacter.Defense);
	}
}
