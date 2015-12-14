using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrossSlash : SkillComponentBase
{

	public override IEnumerator PlaySkillSequence (UnitControllerBase actor, TileController targetTile)
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

	protected override IEnumerator PlaySkillEffectOnTiles (UnitControllerBase actor, SkillEffect effect, TileController targetTile, List<TileController> affectedTiles)
	{
		var affectedUnits = new List<UnitControllerBase> ();
		foreach (var tile in affectedTiles) {
			if (tile.CurrentUnit != null && !tile.CurrentUnit.IsDead) {
				affectedUnits.Add (tile.CurrentUnit);
			}
		}

//		for (int i = 0; i < affectedUnits.Count; ++i) {
//			var affectedUnit = affectedUnits [i];
//			var damage = DamageLogic.GetNormalAttackDamage (actor.Character, affectedUnit.Character, effect);
//			affectedUnit.PlayEffect (Skill.AssetPath);
//			if (i != affectedUnits.Count - 1) {
//				StartCoroutine (affectedUnit.TakeDamage (damage, true));			
//			} 
//			else {
//				yield return StartCoroutine (affectedUnit.TakeDamage (damage, true));
//			}
//		}
		yield return null;
	}
}

