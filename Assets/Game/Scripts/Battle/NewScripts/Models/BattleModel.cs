using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleModel 
{
	private Dictionary<MapPosition, Tile> _mapTiles = new Dictionary<MapPosition, Tile>();
	private Dictionary<BattleCharacter, MapPosition> _battleCharactersPositions = new Dictionary<BattleCharacter, MapPosition>();

	public event Action<MapPosition, Tile> onTileCreated;
	public event Action<MapPosition, BattleCharacter> onBattleCharacterCreated;
	public event Action<List<BattleCharacter>> onTurnOrderChanged;
	public event Action<BattleCharacter> onActorSelected;
	public event Action<Queue<BattleActionOutcome>, Action> onProcessOutcome;

	public BattleCharacter CurrentActor { get; private set; }

	public List<BattleCharacter> AllBattleCharacters
	{
		get
		{
			return this._battleCharactersPositions.Keys.ToList ();
		}
	}

	public void StartSimulation()
	{
		this.NextRound ();
	}

	public void CreateBattleMap(int numberOfRows, int numberOfColumns)
	{
		this.CreateBattleGrid (numberOfRows, numberOfColumns, Const.Team.Player);
		this.CreateBattleGrid (numberOfRows, numberOfColumns, Const.Team.Enemy);
	}
		
	public void SpawnCharactersOnMap()
	{
		// TODO: real character data
		var layout = MapLayout.GetDefaultLayout();
		var enemyCount = 0;
		var playerCount = 0;
		foreach (var position in layout.positions)
		{
			var team = position.Team;
			var character = team == Const.Team.Player ? Character.Fighter () : Character.Slime ();
			var battleCharacter = new BattleCharacter (character, team);

			if (team == Const.Team.Enemy) {
				enemyCount++;
			} else {
				playerCount++;
			}

			ServiceFactory.GetTurnOrderService ().AssignDefaultTurnOrderWeight (battleCharacter);
			battleCharacter.Postfix = ServiceFactory.GetUnitNameService ().GetPostfix (battleCharacter.BaseCharacter.Name);

			this._battleCharactersPositions.Add (battleCharacter, position);
			if (this.onBattleCharacterCreated != null) {
				this.onBattleCharacterCreated (position, battleCharacter);
			}
		}
		Debug.LogWarning (playerCount + " VS " + enemyCount);
	}

	public bool AllCharactersDefeated(Const.Team team)
	{
		return this.AllBattleCharacters.Find (x => x.Team == team && !x.IsDead) == null;
	}

	private void NextRound()
	{
		if (this.AllCharactersDefeated (Const.Team.Enemy)) {
			// player wins
			Debug.LogWarning("Player Wins");
			return;
		}
		else if (this.AllCharactersDefeated (Const.Team.Player)) {
			// enemy wins
			Debug.LogWarning("Enemy Wins");
			return;
		}

		this.CurrentActor = this.GetNextActor();

		if (this.onActorSelected != null) {
			this.onActorSelected (this.CurrentActor);
		}
			
		var movablePositions = ServiceFactory.GetMapService ().GetMoveablePositions (this.CurrentActor, this._battleCharactersPositions, this._mapTiles);

		this.SetTileStateAtPositions (movablePositions, Tile.TileState.MovementHighlight, true);

		if (this.CurrentActor.Team == Const.Team.Enemy) {
			this.RunAI (this.CurrentActor);
		}
		else 
		{
			this.RunAI (this.CurrentActor);
		}
	}

	private void RunAI(BattleCharacter actor)
	{
		var actionQueue = ServiceFactory.GetAIService ().RunAI (actor, this._mapTiles, this._battleCharactersPositions);
		var outcomeQueue = ServiceFactory.GetBattleService ().ProcessActionQueue (actionQueue, this._mapTiles, this._battleCharactersPositions);

		if (this.onProcessOutcome != null) {
			this.onProcessOutcome (outcomeQueue, this.NextRound);
		} 
		else {
			this.NextRound ();
		}
	}

	private void SetTileStateAtPositions(List<MapPosition> positions, Tile.TileState state, bool flag)
	{
		foreach (var position in positions) {
			var tile = this._mapTiles [position];
			tile.AddOrRemoveState (state, flag);
		}
	}

	private BattleCharacter GetNextActor()
	{
		var orderList = ServiceFactory.GetTurnOrderService().GetActionOrder (this.AllBattleCharacters);
		if (this.onTurnOrderChanged != null) {
			this.onTurnOrderChanged (orderList);
		}
		if (orderList != null && orderList.Count > 0) {
			return orderList [0];
		}
		return null;
	}

	private void CreateBattleGrid(int numberOfRows, int numberOfColumns, Const.Team team)
	{
		for (int i = 0; i < numberOfRows; i++) {
			for (int j = 0; j < numberOfColumns; j++) {
				var mapPosition = new MapPosition (j, i, team);
				var tile = new Tile ();
				this._mapTiles.Add (mapPosition, tile);
				if (this.onTileCreated != null) {
					this.onTileCreated (mapPosition, tile);
				}
			}
		}
	}
}
