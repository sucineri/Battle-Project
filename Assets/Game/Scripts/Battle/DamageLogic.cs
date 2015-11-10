using UnityEngine;
using System.Collections;

public class DamageLogic  
{
    public static int GetNormalAttackDamage(Character attacker, Character defender)
    {
        return attacker.Attack - defender.Defense;
    }
}
