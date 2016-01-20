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

        var deadUnits = new List<BattleUnitController>();

        for (int i = 0; i < actionResult.allSkillEffectResult.Count; ++i)
        {
            var delay = this.GetDelayedKeyFrames(i);
            yield return StartCoroutine(this.WaitForFrames(delay));

            var skillEffectResult = actionResult.allSkillEffectResult[i];

            for (int j = 0; j < skillEffectResult.effectsOnTarget.Count; ++j)
            {
                var effectOnTarget = skillEffectResult.effectsOnTarget[j];
                var unit = battleView.GetBattleUnit(effectOnTarget.target);

                if (effectOnTarget.target.CurrentHp <= 0)
                {
                    deadUnits.Add(unit);
                }

                if (j != skillEffectResult.effectsOnTarget.Count - 1)
                {
                    StartCoroutine(PlayTargetOutcome(unit, effectOnTarget));
                }
                else
                {
                    yield return StartCoroutine(PlayTargetOutcome(unit, effectOnTarget));
                }
            }
        }

        foreach (var du in deadUnits)
        {
            StartCoroutine(du.Die());
        }

        yield return StartCoroutine(actor.ReturnToPosition(actorOrigPosition));

        Destroy(this.gameObject);
    }

    protected IEnumerator PlayTargetOutcome(BattleUnitController unit, BattleActionResult.EffectOnTarget effectOnTarget)
    {
        unit.PlayEffect(effectOnTarget.effectPrefabPath);
        var hpPercentage = effectOnTarget.target.HpPercentage;
        yield return StartCoroutine(unit.TakeDamage(effectOnTarget.hpChange, hpPercentage));
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
