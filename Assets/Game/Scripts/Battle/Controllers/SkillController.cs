using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SkillController: MonoBehaviour
{

    [SerializeField] protected int[] _effectKeyFrames;

    public virtual IEnumerator PlaySkillSequence(BattleUnitController actor, BattleView battleView, BattleActionOutcome outcome)
    {
        Vector3 targetPosition = new Vector3();
        BattleUnitController targetedUnit = null;
        TileController targetedTile = null;

        if (outcome.targeteCharacter != null)
        {
            targetedUnit = battleView.GetBattleUnit(outcome.targeteCharacter);
            targetPosition = targetedUnit.transform.position;
        }
        else
        {
            targetedTile = battleView.GetTileAtMapPosition(outcome.targetPosition);
            targetPosition = targetedTile.transform.position;
        }

        var actorOrigPosition = actor.transform.position;

        yield return StartCoroutine(actor.MoveToAttackPosition(targetedUnit, targetPosition));

        StartCoroutine(actor.AnimateAttack());

        var deadUnits = new List<BattleUnitController>();

        for (int i = 0; i < outcome.allOutcomes.Count; ++i)
        {
            var delay = this.GetDelayedKeyFrames(i);
            yield return StartCoroutine(this.WaitForFrames(delay));

            var triggerOutcome = outcome.allOutcomes[i];

            for (int j = 0; j < triggerOutcome.OutcomeForTargets.Count; ++j)
            {
                var targetOutcome = triggerOutcome.OutcomeForTargets[j];
                var unit = battleView.GetBattleUnit(targetOutcome.target);

                if (targetOutcome.target.CurrentHp <= 0)
                {
                    deadUnits.Add(unit);
                }

                if (j != triggerOutcome.OutcomeForTargets.Count - 1)
                {
                    StartCoroutine(PlayTargetOutcome(unit, targetOutcome));
                }
                else
                {
                    yield return StartCoroutine(PlayTargetOutcome(unit, targetOutcome));
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

    protected IEnumerator PlayTargetOutcome(BattleUnitController unit, BattleActionOutcome.OutcomePerTarget targetOutcome)
    {
        unit.PlayEffect(targetOutcome.effectPrefabPath);
        var hpPercentage = targetOutcome.target.HpPercentage;
        yield return StartCoroutine(unit.TakeDamage(targetOutcome.hpChange, hpPercentage));
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
