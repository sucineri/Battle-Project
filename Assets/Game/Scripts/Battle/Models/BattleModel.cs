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
    private List<BattleCharacter> _battleCharacters = new List<BattleCharacter>();

    public event Action<MapPosition, Tile> onTileCreated;
    public event Action<BattleCharacter> onBattleCharacterCreated;
    public event Action<List<BattleCharacter>> onTurnOrderChanged;
    public event Action<BattleCharacter> onActorSelected;
    public event Action<Queue<BattleActionResult>, Action> onProcessActionResult;
    public event Action<BattlePhase> onBattlePhaseChange;
    public event Action<int> onSkillSelected;

    public BattleCharacter CurrentActor { get; private set; }

    public List<MapPosition> CurrentMovablePositions { get; private set; }

    public List<MapPosition> ValidPositionsForSelectedSkill { get; private set; }

    public BattlePhase CurrentPhase { get; private set; }

    public List<BattleCharacter> AllBattleCharacters
    {
        get
        {
            return new List<BattleCharacter>(this._battleCharacters);
        }
    }

    public BattleModel()
    {
        this.CurrentMovablePositions = new List<MapPosition>();
        this.ValidPositionsForSelectedSkill = new List<MapPosition>();
    }

    public void ChangePhase(BattlePhase newPhase)
    {
        this.CurrentPhase = newPhase;

        if (this.onBattlePhaseChange != null)
        {
            this.onBattlePhaseChange(newPhase);
        }

        switch (this.CurrentPhase)
        {
            case BattlePhase.NextRound:
                this.NextRound();
                break;
            default: 
                break;
        }
    }

    public void CreateBattleMap(int numberOfRows, int numberOfColumns)
    {
        this.CreateBattleGrid(numberOfRows, numberOfColumns, Const.Team.Player);
        this.CreateBattleGrid(numberOfRows, numberOfColumns, Const.Team.Enemy);
    }

    public void SpawnCharactersOnMap()
    {
        // TODO: real character data
        var layout = MapLayout.BossLayout();

        foreach (var position in layout.positions)
        {
            var team = position.Team;
            var character = team == Const.Team.Player ? Character.Fighter() : Character.SlimeKing();
            var battleCharacter = new BattleCharacter(character, team);

            ServiceFactory.GetTurnOrderService().AssignDefaultTurnOrderWeight(battleCharacter);
            battleCharacter.Postfix = ServiceFactory.GetUnitNameService().GetPostfix(battleCharacter.BaseCharacter.Name);

            var mapService = ServiceFactory.GetMapService();
            var availableTiles = mapService.GetUnoccupiedTiles(this._battleCharacters, this._mapTiles);
            var tilesRequired = mapService.RequestPositions(battleCharacter.BaseCharacter.PatternShape.Shape, this._mapTiles, position, availableTiles);

            if (tilesRequired.Count == battleCharacter.BaseCharacter.PatternShape.Shape.Count)
            {
                battleCharacter.OccupiedMapPositions = tilesRequired;

                this._battleCharacters.Add(battleCharacter);
                if (this.onBattleCharacterCreated != null)
                {
                    this.onBattleCharacterCreated(battleCharacter);
                }
            }
        }
    }

    public void SelectSkill(int skillIndex)
    {
        if (this.CurrentActor != null)
        {
            var skill = this.CurrentActor.BaseCharacter.Skills.ElementAt(skillIndex);
            if (skill != null)
            {
                this.CurrentActor.SelectedSkill = skill;
                this.ChangePhase(BattlePhase.TargetSelect);

                this.ValidPositionsForSelectedSkill = ServiceFactory.GetMapService().GetValidMapPositionsForSkill(skill, this.CurrentActor, this._mapTiles, this._battleCharacters);
                this.SetTileStateAtPositions(this.ValidPositionsForSelectedSkill, Tile.TileState.SkillRadius, true);
            }
        }
    }

    public void CancelLastSelection()
    {
        // TODO: this might require some major refactoring to know what the previous action was
        this.SetTileStateAtPositions(this.ValidPositionsForSelectedSkill, Tile.TileState.SkillRadius, false);
        this.ValidPositionsForSelectedSkill.Clear();
        this.ChangePhase(BattlePhase.ActionSelect);
    }

    public void CurrentCharacterMoveAction(MapPosition targetPosition)
    {
        if (this.CurrentActor != null && this.CurrentMovablePositions != null)
        {
            if (this.CurrentMovablePositions.Contains(targetPosition))
            {
                var action = new BattleAction(this.CurrentActor, Const.ActionType.Movement, Const.TargetType.Tile, targetPosition, null);
                var actionQueue = new Queue<BattleAction>();
                actionQueue.Enqueue(action);
                var resultQueue = ServiceFactory.GetBattleService().ProcessActionQueue(actionQueue, this._mapTiles, this._battleCharacters);
                this.ProcessOutcome(resultQueue, BattlePhase.ActionSelect);
            }
        }
    }

    public void CurrentCharacterSkillAction(MapPosition targetPosition)
    {
        if (!this.ValidPositionsForSelectedSkill.Contains(targetPosition))
        {
            return;
        }

        if(this.CurrentActor != null && this.CurrentActor.SelectedSkill != null)
        {
            var selectedSkill = this.CurrentActor.SelectedSkill;

            //TODO: check skill target
            var affectedPositions = this.GetMapPositionsForPattern(selectedSkill.Effects[0].EffectTarget.Pattern, targetPosition);

            var action = new BattleAction(this.CurrentActor, Const.ActionType.Skill, Const.TargetType.Tile, targetPosition, selectedSkill);
            var actionQueue = new Queue<BattleAction>();
            actionQueue.Enqueue(action);

            var isValidAction = false;
            var resultQueue = ServiceFactory.GetBattleService().ProcessActionQueue(actionQueue, this._mapTiles, this._battleCharacters);

            foreach (var result in resultQueue)
            {
                if (result.HasResult)
                {
                    isValidAction = true;
                    break;
                }
            }

            if (isValidAction)
            {
                this.SetTileStateAtPositions(affectedPositions, Tile.TileState.SkillHighlight, true);
                this.SetTileStateAtPositions(this.ValidPositionsForSelectedSkill, Tile.TileState.SkillRadius, false);
                this.ValidPositionsForSelectedSkill.Clear();

                this.ProcessOutcome(resultQueue, BattlePhase.NextRound, () =>{
                    this.SetTileStateAtPositions(affectedPositions, Tile.TileState.SkillHighlight, false);
                });
            }
        }
    }

    public List<MapPosition> GetMapPositionsForPattern(Pattern pattern, MapPosition basePosition)
    {
        return ServiceFactory.GetMapService().GeMapPositionsForPattern(pattern, this._mapTiles, basePosition);
    }

    private bool AllCharactersDefeated(Const.Team team)
    {
        return this.AllBattleCharacters.Find(x => x.Team == team && !x.IsDead) == null;
    }

    private void NextRound()
    {
        if (this.CurrentActor != null)
        {
            this.CurrentActor.SelectedSkill = null;
        }

        this.SetTileStateAtPositions(this.CurrentMovablePositions, Tile.TileState.MovementHighlight, false);

        if (this.AllCharactersDefeated(Const.Team.Enemy))
        {
            Debug.LogWarning("Player Wins");
            return;
        }
        else if (this.AllCharactersDefeated(Const.Team.Player))
        {
            Debug.LogWarning("Enemy Wins");
            return;
        }

        this.CurrentActor = this.GetNextActor();
        this.CurrentMovablePositions = ServiceFactory.GetMapService().GetMovablePositions(this.CurrentActor, this._battleCharacters, this._mapTiles);

        if (this.onActorSelected != null)
        {
            this.onActorSelected(this.CurrentActor);
        }

        this.SetTileStateAtPositions(this.CurrentMovablePositions, Tile.TileState.MovementHighlight, true);

        if (this.CurrentActor.Team == Const.Team.Enemy)
        {
//            this.RunAI(this.CurrentActor);
            this.ChangePhase(BattlePhase.ActionSelect);
        }
        else
        {
            this.ChangePhase(BattlePhase.ActionSelect);
        }
    }

    private BattleCharacter GetNextActor()
    {
        var orderList = ServiceFactory.GetTurnOrderService().GetActionOrder(this.AllBattleCharacters);
        if (this.onTurnOrderChanged != null)
        {
            this.onTurnOrderChanged(orderList);
        }
        if (orderList != null && orderList.Count > 0)
        {
            return orderList[0];
        }
        return null;
    }

    private void CreateBattleGrid(int numberOfRows, int numberOfColumns, Const.Team team)
    {
        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                var mapPosition = new MapPosition(j, i, team);
                var tile = new Tile();
                this._mapTiles.Add(mapPosition, tile);
                if (this.onTileCreated != null)
                {
                    this.onTileCreated(mapPosition, tile);
                }
            }
        }
    }

    private void ProcessOutcome(Queue<BattleActionResult> outcomeQueue, BattlePhase nextPhase, Action callback = null)
    {
        Action onComplete = () =>{
            this.ChangePhase(nextPhase);
            if (callback != null)
            {
                callback();
            }
        };

        if (this.onProcessActionResult != null)
        {
            this.ChangePhase(BattlePhase.Animation);
            this.onProcessActionResult(outcomeQueue, onComplete);
        }
        else
        {
            onComplete();
        }
    }

    private void SetTileStateAtPositions(List<MapPosition> positions, Tile.TileState state, bool flag)
    {
        foreach (var position in positions)
        {
            var tile = this._mapTiles[position];
            tile.AddOrRemoveState(state, flag);
        }
    }

    private void RunAI(BattleCharacter actor)
    {
        var actionQueue = ServiceFactory.GetAIService().RunAI(actor, this._mapTiles, this._battleCharacters);
        var outcomeQueue = ServiceFactory.GetBattleService().ProcessActionQueue(actionQueue, this._mapTiles, this._battleCharacters);

        this.ProcessOutcome(outcomeQueue, BattlePhase.NextRound);
    }
}
