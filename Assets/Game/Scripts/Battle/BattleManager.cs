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
//        Instance._battleController = battleController;
//        Instance._mapController = mapController;
    }

    public static void FinishBattle()
    {
        Instance = null;
    }

    #endregion

//    private BattleController _battleController;
//    private MapController _mapController;
    private TurnOrderService _turnOrderService = new TurnOrderService ();
//    private UnitNameService _unitNameService = new UnitNameService();
    private List<UnitControllerBase> _playerUnits = new List<UnitControllerBase>();
    private List<UnitControllerBase> _enemyUnits = new List<UnitControllerBase>();

	private Dictionary<string, MapTile> _allTiles = new Dictionary<string, MapTile>();
	private Dictionary<UnitControllerBase, MapTile> _unitsAndTilesDictionary = new Dictionary<UnitControllerBase, MapTile>();

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
	public bool EnableTileTouch { get { return this.Phase == BattlePhase.TargetSelect || this.Phase == BattlePhase.MovementSelect; } }

    public List<UnitControllerBase> AllUnits
    {
        get
        {
			return this._unitsAndTilesDictionary.Keys.ToList ();
        }
    }

	public void SetMapTiles(Dictionary<string, MapTile> tiles)
	{
		this._allTiles = tiles;
	}

    public MapTile GetTile(Const.Team team, int x, int y)
    {
		MapTile tile = null;
		this._allTiles.TryGetValue(Const.GetTileKey(team, x, y), out tile);
		return tile;
    }

    public void AssignTileToUnit(Const.Team team, UnitControllerBase unit)
    {
        var list = team == Const.Team.Player ? _playerUnits : _enemyUnits;
        list.Add(unit);
    }

    public UnitControllerBase GetNextActor()
    {
		var orderList = this._turnOrderService.GetActionOrder (this.AllUnits);
		if (this.onTurnOrderChanged != null) {
			this.onTurnOrderChanged (orderList);
		}
		if (orderList != null && orderList.Count > 0) {
			var nextActor = orderList [0];
			this.IsPlayerTurn = nextActor.Team == Const.Team.Player;
			return nextActor;
		}
		return null;
    }

//    public List<UnitControllerBase> GetOpponentList(Const.Team actorTeam)
//    {
//        return actorTeam == Const.Team.Player ? this._enemyUnits : this._playerUnits;
//    }

//    public bool AllOpponentsDefeated(Const.Team actorTeam)
//    {
//        var list = GetOpponentList(actorTeam);
//        return list.Find(x => !x.IsDead) == null;
//    }

	public bool AllUnitsDefeated(Const.Team team)
	{
		return this.AllUnits.Find (x => x.Team == team && !x.IsDead) == null;
	}

//    public char GetUnitPostfix(string characterName)
//    {
//        return this._unitNameService.GetPostfix(characterName);
//    }
//
	public List<MapTile> GetAffectedTiles(MapTile targetTile, List<Cordinate> pattern)
	{
		var list = new List<MapTile>();
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
		

	public void AssignTileToUnit(UnitControllerBase unit, MapTile tile)
	{
		if (this._unitsAndTilesDictionary.ContainsKey (unit)) {
			this._unitsAndTilesDictionary [unit] = tile;
		} 
		else {
			this._unitsAndTilesDictionary.Add (unit, tile);
		}
	}

	public MapTile GetUnitOccupiedTile(UnitControllerBase unit)
	{
		foreach (var kv in this._unitsAndTilesDictionary) {
			if (kv.Key == unit) {
				return kv.Value;
			}
		}
		return null;
	}
}
