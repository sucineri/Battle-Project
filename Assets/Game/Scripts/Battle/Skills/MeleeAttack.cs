using UnityEngine;
using System.Collections;

public class MeleeAttack : SkillComponentBase
{

	public override IEnumerator PlaySkillSequence (UnitControllerBase actor, MapTile targetTile)
	{
		if (this._skill == null) {
			yield break;	
		}

		var affectedTiles = BattleManager.Instance.GetAffectedTiles (targetTile, this._skill.SkillTarget.Pattern);
		var affectedUnit = affectedTiles [0].CurrentUnit;
		if (affectedUnit == null || this._skill == null) {
			yield break;
		}

		actor.TurnOrderWeight += BattleActionWeight.GetAttackActionWeight(actor);

		yield return StartCoroutine (actor.MoveToPosition (affectedUnit.transform.position, actor.GetAttackPositionOffset (affectedUnit)));

		StartCoroutine (actor.AnimateAttack ());
		yield return StartCoroutine (this.PlaySkillEffects (actor, targetTile, affectedTiles));

		yield return StartCoroutine (actor.ReturnToBaseTile ());

		Destroy (this.gameObject);
	}

	protected override IEnumerator PlaySkillEffectOnTiles (UnitControllerBase actor, SkillEffect effect, MapTile targetTile, System.Collections.Generic.List<MapTile> affectedTiles)
	{
		var affectedUnit = targetTile.CurrentUnit;
//		if (affectedUnit != null) {
//			affectedUnit.PlayEffect (effect.AssetPath);
//			var damage = DamageLogic.GetNormalAttackDamage (actor.Character, affectedUnit.Character, effect);
//			yield return StartCoroutine (affectedUnit.TakeDamage (damage, true));
//		}
		yield return null;
	}

	
}
