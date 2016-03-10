using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SkillController: MonoBehaviour
{

    [SerializeField] protected int[] _effectKeyFrames;

    public virtual IEnumerator PlaySkillSequence(BattleUnitController actor, BattleView battleView, BattleActionResult actionResult)
    {
        Vector3 targetPosition = new Vector3();
        BattleUnitController targetedUnit = null;
        TileController targetedTile = null;

        if (actionResult.targetCharacter != null)
        {
            targetedUnit = battleView.GetBattleUnit(actionResult.targetCharacter);
            targetPosition = targetedUnit.transform.position;
        }
        else
        {
            targetedTile = battleView.GetTileAtMapPosition(actionResult.targetPosition);
            targetPosition = targetedTile.transform.position;
        }

        var actorOrigPosition = actor.transform.position;

        yield return StartCoroutine(actor.MoveToAttackPosition(targetedUnit, targetPosition));

        StartCoroutine(actor.AnimateAttack());

        for (int i = 0; i < actionResult.allSkillEffectResult.Count; ++i)
        {
            var delay = this.GetDelayedKeyFrames(i);
            yield return StartCoroutine(this.WaitForFrames(delay));

            var skillEffectResult = actionResult.allSkillEffectResult[i];

            yield return this.PlayEffects(skillEffectResult.effectsOnTarget, battleView);
//            for (int j = 0; j < skillEffectResult.effectsOnTarget.Count; ++j)
//            {
//                var effectOnTarget = skillEffectResult.effectsOnTarget[j];
//                var unit = battleView.GetBattleUnit(effectOnTarget.target);
//
//                if (j != skillEffectResult.effectsOnTarget.Count - 1)
//                {
//                    StartCoroutine(PlayEffectOnTarget(unit, effectOnTarget));
//                }
//                else
//                {
//                    yield return StartCoroutine(PlayEffectOnTarget(unit, effectOnTarget));
//                }
//            }
        }

        yield return StartCoroutine(actor.ReturnToPosition(actorOrigPosition));

        Destroy(this.gameObject);
    }

    public IEnumerator PlayEffects(List<BattleActionResult.EffectOnTarget> effectsOnTarget, BattleView battleView)
    {
        for (int i = 0; i < effectsOnTarget.Count; ++i)
        {
            var effectOnTarget = effectsOnTarget[i];
            var unit = battleView.GetBattleUnit(effectOnTarget.target);

            if (i != effectsOnTarget.Count - 1)
            {
                StartCoroutine(PlayEffectOnTarget(unit, effectOnTarget));
            }
            else
            {
                yield return StartCoroutine(PlayEffectOnTarget(unit, effectOnTarget));
            }
        }
    }

    protected IEnumerator PlayEffectOnTarget(BattleUnitController unit, BattleActionResult.EffectOnTarget effectOnTarget)
    {
        unit.PlayEffect(effectOnTarget.effectPrefabPath);
        var hpPercentage = effectOnTarget.target.HpPercentage;
        yield return StartCoroutine(unit.TakeEffect(effectOnTarget, hpPercentage));
    }

    protected IEnumerator WaitForFrames(int frames)
    {
        int counter = 0;
        while (counter < frames)
        {
            counter++;
            yield return null;
        }
    }

    protected int GetDelayedKeyFrames(int index)
    {
        if (index >= 0 && index < this._effectKeyFrames.Length)
        {
            return this._effectKeyFrames[index];
        }
        return 0;
    }
}
