using UnityEngine;
using System.Collections;

public class MeleeAttack : SkillComponentBase {

    void Awake()
    {
        this.SkillTarget = Targetting.SingleOpponentTarget ();
    }

	public override IEnumerator PlaySkillSequence (UnitControllerBase actor, MapTile targetTile)
	{
        var affectedTiles = this.GetAffectedTiles(targetTile);
        var affectedUnit = affectedTiles[0].CurrentUnit;
        if (affectedUnit == null)
        {
            yield break;
        }

        yield return StartCoroutine(actor.MoveToPosition(affectedUnit.transform.position, actor.GetAttackPositionOffset(affectedUnit)));

        StartCoroutine(actor.AnimateAttack());
        yield return StartCoroutine(this.PlaySkillEffects(actor, targetTile, affectedTiles));

        yield return StartCoroutine(actor.ReturnToBaseTile());
    }

    protected override IEnumerator PlaySkillEffectOnTiles(UnitControllerBase actor, int effectindex, MapTile targetTile, System.Collections.Generic.List<MapTile> affectedTiles)
    {
        var affectedUnit = targetTile.CurrentUnit;
        if (affectedUnit != null)
        {
            var damage = DamageLogic.GetNormalAttackDamage(actor.Character, affectedUnit.Character);
            yield return StartCoroutine(affectedUnit.TakeDamage(damage, true));
        }
    }

	
}
