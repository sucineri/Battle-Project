using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleModel 
{
	public enum BattlePhase
	{
		ActionSelect,
		TargetSelect,
		Animation,
		Result,
		NextRound
	}

	private Dictionary<MapPosition, Tile> _mapTiles = new Dictionary<MapPosition, Tile>();
	private Dictionary<BattleCharacter, MapPosition> _battleCharactersPositions = new Dictionary<BattleCharacter, MapPosition>();

	public event Action<MapPosition, Tile> onTileCreated;
	public event Action<MapPosition, BattleCharacter> onBattleCharacterCreated;
	public event Action<List<BattleCharacter>> onTurnOrderChanged;
	public event Action<BattleCharacter> onActorSelected;
	public event Action<Queue<BattleActionOutcome>, Action> onProcessOutcome;
	public event Action<BattlePhase> onBattlePhaseChange;
	public event Action<int> onSkillSelected;

	public BattleCharacter CurrentActor { get; private set; }
	public List<MapPosition> CurrentMovablePositions { get; private set; }
	public Skill CurrentSelectedSkill { get; private set; }
	public BattlePhase CurrentPhase { get; private set; }

	public Vector2 MapSize { get; private set; }

	public List<BattleCharacter> AllBattleCharacters
	{
		get
		{
			return this._battleCharactersPositions.Keys.ToList ();
		}
	}

	public BattleModel()
	{
		this.CurrentMovablePositions = new List<MapPosition> ();
	}

	public void ChangePhase(BattlePhase newPhase)
	{
		this.CurrentPhase = newPhase;

		if (this.onBattlePhaseChange != null) {
			this.onBattlePhaseChange (newPhase);
		}

		switch (this.CurrentPhase) {
		case BattlePhase.NextRound:
			this.NextRound ();
			break;
		default: 
			break;
		}
	}
		
	public void CreateBattleMap(int numberOfRows, int numberOfColumns)
	{
		this.CreateBattleGrid (numberOfRows, numberOfColumns, Const.Team.Player);
		this.CreateBattleGrid (numberOfRows, numberOfColumns, Const.Team.Enemy);
		this.MapSize = new Vector2 (numberOfColumns, numberOfRows);
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
	}

	public void SelecteSkill(int skillIndex)
	{
		if (this.CurrentActor != null) {
			var skills = this.CurrentActor.BaseCharacter.Skills;
			CurrentSelectedSkill = skills.ElementAt (skillIndex);
			if (CurrentSelectedSkill != null) {
				this.ChangePhase (BattlePhase.TargetSelect);
			}
		}
	}

	public void CancelLastSelection()
	{
		if (this.CurrentSelectedSkill != null) {
			this.CurrentSelectedSkill = null;
			this.ChangePhase (BattlePhase.ActionSelect);
		}
	}

	public void CurrentCharacterMoveAction(MapPosition targetPosition)
	{
		if (this.CurrentActor != null && this.CurrentMovablePositions != null) {
			if (this.CurrentMovablePositions.Contains (targetPosition)) {
				var action = new BattleAction (this.CurrentActor, Const.ActionType.Movement, Const.TargetType.Tile, targetPosition, null);
				var actionQueue = new Queue<BattleAction> ();
				actionQueue.Enqueue (action);
				var outcomeQueue = ServiceFactory.GetBattleService ().ProcessActionQueue (actionQueue, this._mapTiles, this._battleCharactersPositions, this.MapSize);
				this.ProcessOutcome (outcomeQueue, BattlePhase.ActionSelect);
			}
		}
	}

	public void CurrentCharacterSkillAction(MapPosition targetPosition)
	{
		if (this.CurrentSelectedSkill != null) {
			var affectedPositions = ServiceFactory.GetMapService ().GeAffectedMapPositions (this.CurrentSelectedSkill.SkillTarget.Pattern, 
				this._mapTiles, targetPosition, this.MapSize);

			var affectedCharacters = ServiceFactory.GetBattleService ().GetAffectdCharacters (this._battleCharactersPositions, affectedPositions);

			if (affectedCharacters.Count > 0) {
				this.SetTileStateAtPositions (affectedPositions, Tile.TileState.SkillHighlight, true);

				var action = new BattleAction (this.CurrentActor, Const.ActionType.Skill, Const.TargetType.Tile, targetPosition, this.CurrentSelectedSkill);
				var actionQueue = new Queue<BattleAction> ();
				actionQueue.Enqueue (action);
				var outcomeQueue = ServiceFactory.GetBattleService ().ProcessActionQueue (actionQueue, this._mapTiles, this._battleCharactersPositions, this.MapSize);
				this.ProcessOutcome (outcomeQueue, BattlePhase.NextRound, () => {
					this.SetTileStateAtPositions (affectedPositions, Tile.TileState.SkillHighlight, false);
				});
			}
		}
	}

	private bool AllCharactersDefeated(Const.Team team)
	{
		return this.AllBattleCharacters.Find (x => x.Team == team && !x.IsDead) == null;
	}

	private void NextRound()
	{
		this.SetTileStateAtPositions (this.CurrentMovablePositions, Tile.TileState.MovementHighlight, false);

		if (this.AllCharactersDefeated (Const.Team.Enemy)) {
			Debug.LogWarning("Player Wins");
			return;
		}
		else if (this.AllCharactersDefeated (Const.Team.Player)) {
			Debug.LogWarning("Enemy Wins");
			return;
		}

		this.CurrentActor = this.GetNextActor();
		this.CurrentMovablePositions = ServiceFactory.GetMapService ().GetMovablePositions (this.CurrentActor, this._battleCharactersPositions, this._mapTiles);

		if (this.onActorSelected != null) {
			this.onActorSelected (this.CurrentActor);
		}

		this.SetTileStateAtPositions (this.CurrentMovablePositions, Tile.TileState.MovementHighlight, true);

		if (this.CurrentActor.Team == Const.Team.Enemy) {
			this.RunAI (this.CurrentActor);
		}
		else 
		{
			this.ChangePhase (BattlePhase.ActionSelect);
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

	private void ProcessOutcome(Queue<BattleActionOutcome> outcomeQueue, BattlePhase nextPhase, Action callback = null)
	{
		Action onComplete = () => {
			this.ChangePhase (nextPhase);
			if(callback != null)
			{
				callback();
			}
		};

		if (this.onProcessOutcome != null) {
			this.ChangePhase (BattlePhase.Animation);
			this.onProcessOutcome (outcomeQueue, onComplete);
		} 
		else {
			onComplete ();
		}
	}

	private void SetTileStateAtPositions(List<MapPosition> positions, Tile.TileState state, bool flag)
	{
		foreach (var position in positions) {
			var tile = this._mapTiles [position];
			tile.AddOrRemoveState (state, flag);
		}
	}
		
	private void RunAI(BattleCharacter actor)
	{
		var actionQueue = ServiceFactory.GetAIService ().RunAI (actor, this._mapTiles, this._battleCharactersPositions);
		var outcomeQueue = ServiceFactory.GetBattleService ().ProcessActionQueue (actionQueue, this._mapTiles, this._battleCharactersPositions, this.MapSize);

		this.ProcessOutcome (outcomeQueue, BattlePhase.NextRound);
	}
}
