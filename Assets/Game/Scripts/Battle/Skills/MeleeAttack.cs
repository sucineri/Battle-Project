//using UnityEngine;
//using System.Collections;
//
//public class MeleeAttack : SkillController
//{
//
//	public IEnumerator PlaySkillSequence (BattleUnitController actor, TileController targetTile)
//	{
////		if (this._skill == null) {
////			yield break;	
////		}
////
////		var affectedTiles = BattleManager.Instance.GetAffectedTiles (targetTile, this._skill.SkillTarget.Pattern);
////		var affectedUnit = affectedTiles [0].CurrentUnit;
////		if (affectedUnit == null || this._skill == null) {
////			yield break;
////		}
//
////		actor.TurnOrderWeight += BattleActionWeight.GetAttackActionWeight(actor);
//
////		yield return StartCoroutine (actor.MoveToPosition (affectedUnit.transform.position, actor.GetAttackPositionOffset (affectedUnit)));
//
//		StartCoroutine (actor.AnimateAttack ());
////		yield return StartCoroutine (this.PlaySkillEffects (actor, targetTile, affectedTiles));
//
////		yield return StartCoroutine (actor.ReturnToBaseTile ());
//
//		Destroy (this.gameObject);
//		yield return 0;
//	}
//
//	protected override IEnumerator PlaySkillEffectOnTiles (BattleUnitController actor, SkillEffect effect, TileController targetTile, System.Collections.Generic.List<TileController> affectedTiles)
//	{
////		var affectedUnit = targetTile.CurrentUnit;
////		if (affectedUnit != null) {
////			affectedUnit.PlayEffect (effect.AssetPath);
////			var damage = DamageLogic.GetNormalAttackDamage (actor.Character, affectedUnit.Character, effect);
////			yield return StartCoroutine (affectedUnit.TakeDamage (damage, true));
////		}
//		yield return null;
//	}
//
//	
//}
