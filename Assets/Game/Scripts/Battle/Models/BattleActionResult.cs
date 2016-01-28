using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO: rewrite these crap
public class BattleActionResult
{
    public class ActionEffectResult
    {
        public List<EffectOnTarget> effectsOnTarget = new List<EffectOnTarget>();

        public void AddEffectOnTarget(EffectOnTarget effectOnTarget)
        {
            this.effectsOnTarget.Add(effectOnTarget);
        }
    }

    public class EffectOnTarget
    {
        public BattleCharacter target;
        public double hpChange;
        public MapPosition positionChangeTo;
        public string effectPrefabPath;
    }

    public Const.ActionType type;
    public MapPosition targetPosition;
    public BattleCharacter targetCharacter;
    public BattleCharacter actor;
    public Skill skill;
    public List<ActionEffectResult> allSkillEffectResult = new List<ActionEffectResult>();

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

    public bool IsCharacterAffected(BattleCharacter character)
    {
        foreach (var effectResult  in this.allSkillEffectResult)
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
}
