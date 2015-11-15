using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SkillComponentBase: MonoBehaviour
{

    [SerializeField] protected int[] _effectKeyFrames;

    public Targetting SkillTarget { get; protected set; }

    public abstract IEnumerator PlaySkillSequence(UnitControllerBase actor, MapTile targetTile);

    protected abstract IEnumerator PlaySkillEffectOnTiles(UnitControllerBase actor, int effectindex, MapTile targetTile, List<MapTile> affectedTiles);

    public List<MapTile> GetAffectedTiles(MapTile targetTile)
    {
        var list = new List<MapTile>();
        foreach (var offset in this.SkillTarget.Pattern)
        {
            var tileCord = new Cordinate(targetTile.X + offset.X, targetTile.Y + offset.Y);
            var tile = BattleManager.Instance.GetTile(targetTile.Team, tileCord.X, tileCord.Y);
            if (tile != null)
            {
                list.Add(tile);
            }
        }
        return list;
    }

    protected IEnumerator PlaySkillEffects(UnitControllerBase actor, MapTile targetTile, List<MapTile> affectedTiles)
    {
        var lastKeyFrame = 0;
        for (int i = 0; i < this._effectKeyFrames.Length; ++i)
        {
            var keyFrame = this._effectKeyFrames[i];
            var diff = keyFrame - lastKeyFrame;
            lastKeyFrame = keyFrame;
            yield return StartCoroutine(this.WaitForFrames(diff));
            if (i == this._effectKeyFrames.Length - 1)
            {
                yield return StartCoroutine(this.PlaySkillEffectOnTiles(actor, i, targetTile, affectedTiles));
            }
            else
            {
                StartCoroutine(this.PlaySkillEffectOnTiles(actor, i, targetTile, affectedTiles));
            }
        }
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
}
