using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO: rewrite these crap
public class BattleActionResult
{
    public enum StatusEffectResultType
    {
        None,
        Landed,
        Resisted,
        Ineffetive
    }

    public class ActionEffectResult
    {
        public List<EffectOnTarget> effectsOnTarget = new List<EffectOnTarget>();

        public void AddEffectOnTarget(EffectOnTarget effectOnTarget)
        {
            this.effectsOnTarget.Add(effectOnTarget);
        }

        public double GetTotalHpChangeOnTarget(BattleCharacter target)
        {
            var total = 0d;
            foreach (var effectOnTarget in this.effectsOnTarget)
            {
                if (effectOnTarget.target == target)
                {
                    total += effectOnTarget.hpChange;
                }
            }
            return total;
        }
    }

    public class EffectOnTarget
    {
        public BattleCharacter target;
        public double hpChange;
        public MapPosition positionChangeTo;
        public SkillEffect skillEffect;
        public string effectPrefabPath;
        public bool isSuccess = true;
        public bool isCritical = false;
        public bool isEmptyEffect = false;
        public StatusEffectResult statusEffectResult;
    }

    public class StatusEffectResult
    {
        public StatusEffectResultType resultType;
        public List<StatusEffect> landedEffects;
    }

    public Const.ActionType type;
    public MapPosition targetPosition;
    public BattleCharacter targetCharacter;
    public BattleCharacter actor;
    public Skill skill;
    public List<ActionEffectResult> allSkillEffectResult = new List<ActionEffectResult>();

    public ActionEffectResult PostActionEffectResult { get; private set; }

    public bool HasResult
    {
        get
        {
            return allSkillEffectResult.Count > 0;
        }
    }

    public void AddSkillEffectResult(ActionEffectResult effectResult)
    {
        this.allSkillEffectResult.Add(effectResult);
    }

    public void AddPostActionEffectResult(EffectOnTarget postActionEffectOnTarget)
    {
        if (this.PostActionEffectResult == null)
        {
            this.PostActionEffectResult = new ActionEffectResult();
        }
        this.PostActionEffectResult.AddEffectOnTarget(postActionEffectOnTarget);
    }

    public bool IsCharacterAffected(BattleCharacter character)
    {
        foreach (var effectResult in this.allSkillEffectResult)
        {
            foreach (var targetResult in effectResult.effectsOnTarget)
            {
                if (targetResult.target == character)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public double GetTotalHpChangeOnTarget(BattleCharacter target)
    {
        var total = 0d;
        foreach (var skillEffect in this.allSkillEffectResult)
        {
            total += skillEffect.GetTotalHpChangeOnTarget(target);
        }

        if (this.PostActionEffectResult != null)
        {
            total += this.PostActionEffectResult.GetTotalHpChangeOnTarget(target);
        }
        return total;
    }
}
