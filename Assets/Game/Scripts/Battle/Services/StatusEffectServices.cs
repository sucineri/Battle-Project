using System;
using System.Collections;
using System.Collections.Generic;

public class StatusEffectService  
{
    private Random _random = new Random();

    public BattleActionResult.StatusEffectResult GetStatusEffectResult(BattleCharacter target, List<StatusEffect> allStatusEffects)
    {
        var result = new BattleActionResult.StatusEffectResult();

        var landedEffects = new List<StatusEffect>();
        var resistedCount = 0;
        var ineffectiveCount = 0;
        foreach (var statusEffect in allStatusEffects)
        {
            var resistance = target.GetStatusEffectResistance(statusEffect.StatusEffectType);
            if (!this.IsResisted(resistance))
            {
                // BUG: Should not apply here. just check!!!
                if (target.ApplyStatusEffect(statusEffect))
                {
                    landedEffects.Add(statusEffect);
                }
                else
                {
                    ineffectiveCount++;
                }
            }
            else
            {
                resistedCount++;
            }
        }

        result.landedEffects = landedEffects;
        result.resultType = this.GetStatusResultType(allStatusEffects.Count, resistedCount, ineffectiveCount);

        return result;
    }

    public void AddPostActionEffect(BattleActionResult actionResult, BattleCharacter target)
    {
        foreach (var specialStateEffect in target.SpecialStateModifiers)
        {
            var totalHpChangeOnTarget = actionResult.GetTotalHpChangeOnTarget(target);
            var isTargetAlive = (target.CurrentHp + totalHpChangeOnTarget) > 0;
            switch (specialStateEffect.Key)
            {
                case StatusEffect.Type.Poison:
                    if (isTargetAlive)
                    {
                        actionResult.AddPostActionEffectResult(this.GetPoisonEffect(target, specialStateEffect.Value.StatModifier.Magnitude));
                    }
                    break;
                default:
                    break;
                        
            }
        }
    }

    private BattleActionResult.EffectOnTarget GetPoisonEffect(BattleCharacter target, double magnitude)
    {
        var effectOnTarget = new BattleActionResult.EffectOnTarget();
        effectOnTarget.target = target;
        effectOnTarget.isSuccess = true;
        effectOnTarget.hpChange = -target.CurrentHp * magnitude;
        return effectOnTarget;
    }

    private BattleActionResult.StatusEffectResultType GetStatusResultType(int totalEffectCount, int resistedCount, int ineffectiveCount)
    {
        if (totalEffectCount == 0)
        {
            return BattleActionResult.StatusEffectResultType.None;
        }
        if (resistedCount + ineffectiveCount < totalEffectCount)
        {
            return BattleActionResult.StatusEffectResultType.Landed;
        }
        if (resistedCount > 0)
        {
            return BattleActionResult.StatusEffectResultType.Resisted;
        }
        return BattleActionResult.StatusEffectResultType.Ineffetive;
    }

    private bool IsResisted(double resistance)
    {
        // range of random.NextDouble() is [0, 0.99999999999999978];
        return _random.NextDouble() >= (1d - resistance);
    }
}
