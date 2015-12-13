using System;
using System.Collections;

public class DamageLogic  
{
	public static double GetNormalAttackDamage(CharacterStats attacker, CharacterStats defender, SkillEffect effect)
    {
		var attack = attacker.Attack * effect.StatsModifiers.GetStats (Const.BasicStats.Attack);

		return System.Math.Floor (attack - defender.Defense);
    }

	public static double GetNormalAttackDamage(BattleCharacter attacker, BattleCharacter defender, SkillEffect effect)
	{
		var attack = attacker.BaseCharacter.Attack * effect.StatsModifiers.GetStats (Const.BasicStats.Attack);
		return Math.Floor (attack - defender.BaseCharacter.Defense);
	}
}
