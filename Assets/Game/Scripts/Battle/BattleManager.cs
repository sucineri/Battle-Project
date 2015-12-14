using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BattleManager
{
	public enum BattlePhase
	{
		ActionSelect,
		MovementSelect,
		TargetSelect,
		Animation,
		Result,
		NextRound
	}

    #region static instance stuff

    public static BattleManager Instance { get; private set; }

    protected BattleManager()
    {
        // no public default constructor 
    }

	public static void CreateBattleInstance()
    {
        Instance = new BattleManager();
    }

    public static void FinishBattle()
    {
        Instance = null;
    }

    #endregion

    private TurnOrderService _turnOrderService = new TurnOrderService ();

	private Dictionary<string, TileController> _allTiles = new Dictionary<string, TileController>();
	private Dictionary<UnitControllerBase, TileController> _unitsAndTilesDictionary = new Dictionary<UnitControllerBase, TileController>();

	public event Action<BattlePhase> onBattlePhaseChange;
	public event Action<List<UnitControllerBase>> onTurnOrderChanged;

	private BattlePhase _phase;
	public BattlePhase Phase 
	{ 
		get 
		{
			return this._phase;
		}

		set 
		{
			this._phase = value;
			if (this.onBattlePhaseChange != null) {
				this.onBattlePhaseChange (this._phase);
			}
		}
	}

    public bool IsPlayerTurn { get; private set; }
	public bool IsAnimating { get; private set; }
	public UnitControllerBase CurrentActor { get; private set; }
	public bool EnableTileTouch { get { return this.Phase == BattlePhase.TargetSelect || this.Phase == BattlePhase.MovementSelect; } }

    public List<UnitControllerBase> AllUnits
    {
        get
        {
			return this._unitsAndTilesDictionary.Keys.ToList ();
        }
    }

	public void SetMapTiles(Dictionary<string, TileController> tiles)
	{
		this._allTiles = tiles;
	}

    public TileController GetTile(Const.Team team, int x, int y)
    {
		TileController tile = null;
		this._allTiles.TryGetValue(Const.GetTileKey(team, x, y), out tile);
		return tile;
    }

    public UnitControllerBase GetNextActor()
    {
//		var orderList = this._turnOrderService.GetActionOrder (this.AllUnits);
//		if (this.onTurnOrderChanged != null) {
//			this.onTurnOrderChanged (orderList);
//		}
//		if (orderList != null && orderList.Count > 0) {
//			var nextActor = orderList [0];
//			this.IsPlayerTurn = nextActor.Team == Const.Team.Player;
//			return nextActor;
//		}
		return null;
    }

	public bool AllUnitsDefeated(Const.Team team)
	{
		return this.AllUnits.Find (x => x.Team == team && !x.IsDead) == null;
	}

	public List<TileController> GetAffectedTiles(TileController targetTile, List<Cordinate> pattern)
	{
		var list = new List<TileController>();
		foreach (var offset in pattern)
		{
			var tileCord = new Cordinate(targetTile.X + offset.X, targetTile.Y + offset.Y);
			var tile = this.GetTile(targetTile.Team, tileCord.X, tileCord.Y);
			if (tile != null)
			{
				list.Add(tile);
			}
		}
		return list;
	}
		

	public void AssignTileToUnit(UnitControllerBase unit, TileController tile)
	{
		if (this._unitsAndTilesDictionary.ContainsKey (unit)) {
			this._unitsAndTilesDictionary [unit] = tile;
		} 
		else {
			this._unitsAndTilesDictionary.Add (unit, tile);
		}
	}

	public TileController GetUnitOccupiedTile(UnitControllerBase unit)
	{
		foreach (var kv in this._unitsAndTilesDictionary) {
			if (kv.Key == unit) {
				return kv.Value;
			}
		}
		return null;
	}


	private void NextRound()
	{
		if (this.AllUnitsDefeated (Const.Team.Enemy)) {
			// player wins
			return;
		}
		else if (this.AllUnitsDefeated (Const.Team.Player)) {
			// enemy wins
			return;
		}

		this.CurrentActor = this.GetNextActor();

		if (this.CurrentActor.Team == Const.Team.Enemy) {
//			this.RunAI (this.CurrentActor);
		}
		else 
		{
//			this.ProcessPlayerTurn (this.CurrentActor);
		}
	}

	// default AI attack
	public void RunAI(CharacterStats character)
	{
//		var allUnits = BattleManager.Instance.AllUnits;
//		var targetTile = TargetLogic.GetTargetTile(this, this.AllUnits);
//		var skill = actor.GetSelectedSkill ();
//		yield return StartCoroutine(this.GetSelectedSkill().PlaySkillSequence(this, targetTile));
	}
}
