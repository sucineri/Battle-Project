using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SkillComponentBase: MonoBehaviour
{

	[SerializeField] protected int[] _effectKeyFrames;

	protected Skill _skill;

	public abstract IEnumerator PlaySkillSequence (UnitControllerBase actor, TileController targetTile);

	protected abstract IEnumerator PlaySkillEffectOnTiles (UnitControllerBase actor, SkillEffect effect, TileController targetTile, List<TileController> affectedTiles);

	public void InitWithSkill (Skill skill)
	{
		this._skill = skill;
	}

	public Skill GetSkill()
	{
		return this._skill;
	}

	protected IEnumerator PlaySkillEffects (UnitControllerBase actor, TileController targetTile, List<TileController> affectedTiles)
	{
		if (this._skill == null) {
			yield break;
		}

		var lastKeyFrame = 0;
		for (int i = 0; i < this._skill.Effects.Count; ++i) {
			var effect = this._skill.Effects [i];
			var keyFrame = i < this._effectKeyFrames.Length ? this._effectKeyFrames[i] : 0;
			var diff = Mathf.Max(0, keyFrame - lastKeyFrame);
			lastKeyFrame = keyFrame;
			yield return StartCoroutine (this.WaitForFrames (diff));
			if (i == this._skill.Effects.Count - 1) {
				yield return StartCoroutine (this.PlaySkillEffectOnTiles (actor, effect, targetTile, affectedTiles));
			} else {
				StartCoroutine (this.PlaySkillEffectOnTiles (actor, effect, targetTile, affectedTiles));
			}
		}
	}

	protected IEnumerator WaitForFrames (int frames)
	{
		int counter = 0;
		while (counter < frames) {
			counter++;
			yield return null;
		}
	}
}
