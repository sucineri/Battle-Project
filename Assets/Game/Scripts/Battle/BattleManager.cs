using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BattleManager
{
    #region static instance stuff

    public static BattleManager Instance { get; private set; }

    protected BattleManager()
    {
        // no public default constructor 
    }

    public static void CreateBattleInstance(BattleController battleController, MapController mapController)
    {
        Instance = new BattleManager();
        Instance._battleController = battleController;
        Instance._mapController = mapController;
		Instance._mapController.onAnimationStateChange += Instance.OnAnimationStateChange;
    }

    public static void FinishBattle()
    {
        Instance = null;
    }

    #endregion

    private BattleController _battleController;
    private MapController _mapController;
    private TurnOrderService _turnOrderService = new TurnOrderService ();
    private UnitNameService _unitNameService = new UnitNameService();
    private List<UnitControllerBase> _playerUnits = new List<UnitControllerBase>();
    private List<UnitControllerBase> _enemyUnits = new List<UnitControllerBase>();

    public bool IsPlayerTurn { get; private set; }
	public bool IsAnimating { get; private set; }
    public bool EnableInput { get { return !this.IsAnimating && this.IsPlayerTurn; } }

    public List<UnitControllerBase> AllUnits
    {
        get
        {
            return this._playerUnits.Concat(this._enemyUnits).ToList();
        }
    }

    public MapTile GetTile(Const.Team team, int x, int y)
    {
        if (this._mapController != null)
        {
            return this._mapController.GetTile(team, x, y);
        }
        return null;
    }

    public void AddUnit(Const.Team team, UnitControllerBase unit)
    {
        var list = team == Const.Team.Player ? _playerUnits : _enemyUnits;
        list.Add(unit);
    }

    public void InitTurnOrder()
    {
        _turnOrderService.Init(this.AllUnits, this._battleController.UpdateTurnOrderView);
    }

    public UnitControllerBase GetNextActor()
    {
        var nextActor = this._turnOrderService.GetNextActor();
        this.IsPlayerTurn = nextActor.Team == Const.Team.Player;
        return nextActor;
    }

    public List<UnitControllerBase> GetOpponentList(Const.Team actorTeam)
    {
        return actorTeam == Const.Team.Player ? this._enemyUnits : this._playerUnits;
    }

    public bool AllOpponentsDefeated(Const.Team actorTeam)
    {
        var list = GetOpponentList(actorTeam);
        return list.Find(x => !x.IsDead) == null;
    }

    public char GetUnitPostfix(string characterName)
    {
        return this._unitNameService.GetPostfix(characterName);
    }

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

	private void OnAnimationStateChange(bool isAnimating)
	{
		this.IsAnimating = isAnimating;
	}
}
