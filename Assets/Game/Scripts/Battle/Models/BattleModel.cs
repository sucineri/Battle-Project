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

    public BattleTurnOrderModel turnOrderModel;

    public event Action<MapPosition, Tile> onTileCreated;
    public event Action<BattleCharacter> onBattleCharacterCreated;

    public event Action<BattleCharacter> onNextEnemyActorChanged;
    public event Action<Queue<BattleActionResult>, Action> onProcessActionResult;
    public event Action<BattlePhase> onBattlePhaseChange;

    public BattleCharacter CurrentActor { get; private set; }

    // For now this is only used for updating the enmity view
    private BattleCharacter _nextEnemyActor;

    public List<MapPosition> CurrentMovablePositions { get; private set; }

    public List<MapPosition> ValidPositionsForSelectedSkill { get; private set; }

    public BattlePhase CurrentPhase { get; private set; }

    public List<BattleCharacter> AllBattleCharacters
    {
        get
        {
            // returns a copy to avoid bad stuff from happening
            return new List<BattleCharacter>(this._battleCharacters);
        }
    }

    public BattleModel()
    {
        this.CurrentMovablePositions = new List<MapPosition>();
        this.ValidPositionsForSelectedSkill = new List<MapPosition>();
        this.turnOrderModel = new BattleTurnOrderModel();
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
        var layout = MapLayout.TwoSlimesLayout();
        var id = 0;

        foreach (var spawn in layout.spawns)
        {
            var position = spawn.Position;
            var team = position.Team;
            var character = spawn.Character;
            var battleCharacter = new BattleCharacter(character, team);
            battleCharacter.BattleCharacterId = ++id;

            battleCharacter.Postfix = ServiceFactory.GetUnitNameService().GetPostfix(battleCharacter.BaseCharacter.Name);

            var mapService = ServiceFactory.GetMapService();
            var availableTiles = mapService.GetUnoccupiedTiles(this._battleCharacters, this._mapTiles);
            var tilesRequired = mapService.RequestPositionsForCharacter(battleCharacter, this._mapTiles, position, availableTiles);

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

        var turnOrderService = ServiceFactory.GetTurnOrderService();
        foreach (var character in this._battleCharacters)
        {
            turnOrderService.ApplyDefaultCooldownToCharacter(character);
        }

        ServiceFactory.GetEnmityService().InitEnmity(this._battleCharacters);
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
                var resultQueue = ServiceFactory.GetBattleService().GetActionResults(actionQueue, this._mapTiles, this._battleCharacters);
                this.ProcessActionResult(resultQueue, BattlePhase.ActionSelect);
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
            var targeting = selectedSkill.Effects[0].EffectTarget;
            var affectedPositions = this.GetMapPositionsForPattern(targeting.Pattern, targeting.TargetGroup, this.CurrentActor.Team, targetPosition);

            var action = new BattleAction(this.CurrentActor, Const.ActionType.Skill, Const.TargetType.Tile, targetPosition, selectedSkill);
            var actionQueue = new Queue<BattleAction>();
            actionQueue.Enqueue(action);

            var isValidAction = false;
            var resultQueue = ServiceFactory.GetBattleService().GetActionResults(actionQueue, this._mapTiles, this._battleCharacters);

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

                this.ProcessActionResult(resultQueue, BattlePhase.NextRound, () =>{
                    this.SetTileStateAtPositions(affectedPositions, Tile.TileState.SkillHighlight, false);
                });
            }
        }
    }

    public List<MapPosition> GetMapPositionsForPattern(Pattern pattern, Const.SkillTargetGroup targetGroup, Const.Team sourceTeam, MapPosition basePosition)
    {
        return ServiceFactory.GetMapService().GeMapPositionsForPattern(pattern, targetGroup, sourceTeam, this._mapTiles, basePosition);
    }

    private bool AllCharactersDefeated(Const.Team team)
    {
        return this.AllBattleCharacters.Find(x => x.Team == team && !x.IsDead) == null;
    }

    private void NextRound()
    {
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

        this.RefreshActionOrder();

        this.CurrentMovablePositions = ServiceFactory.GetMapService().GetMovablePositions(this.CurrentActor, this._battleCharacters, this._mapTiles);

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

    private void RefreshActionOrder()
    {
        this.turnOrderModel.CalculateActionOrder(this.AllBattleCharacters);
        this.CurrentActor = this.turnOrderModel.GetCurrentActor();
        this._nextEnemyActor = this.turnOrderModel.GetNextEnemyActor();

        if (this.onNextEnemyActorChanged != null)
        {
            this.onNextEnemyActorChanged(this._nextEnemyActor);
        }
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

    private void ProcessActionResult(Queue<BattleActionResult> resultQueue, BattlePhase nextPhase, Action callback = null)
    {
        foreach (var result in resultQueue)
        {
            this.ApplyActionResult(result);
        }

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
            this.onProcessActionResult(resultQueue, onComplete);
        }
        else
        {
            onComplete();
        }
    }

    private void ApplyActionResult(BattleActionResult result)
    {
        switch (result.type)
        {
            case Const.ActionType.Movement:
                this.ApplyMovementActionResult(result);
                break;
            case Const.ActionType.Skill:
                this.ApplySkillActionResult(result);
                break;
            default:
                break;
        }
    }

    private void ApplyMovementActionResult(BattleActionResult movementResult)
    {
        foreach (var effectResult in movementResult.allSkillEffectResult)
        {
            foreach (var effectOnTarget in effectResult.effectsOnTarget)
            {
                if (effectOnTarget.isSuccess)
                {
                    var actor = effectOnTarget.target;
                    var moveTo = effectOnTarget.positionChangeTo;
                    Debug.LogWarning(actor.Name + " moves to " + moveTo.ToString());

                    var mapService = ServiceFactory.GetMapService();
                    var unOccupiedPositions = mapService.GetUnoccupiedTiles(this._battleCharacters, this._mapTiles);
                    var newOccupiedPositions = mapService.RequestPositionsForCharacter(actor, this._mapTiles, moveTo, unOccupiedPositions);

                    // update character position
                    actor.OccupiedMapPositions = newOccupiedPositions;
                }
            }
        }
    }

    private void ApplySkillActionResult(BattleActionResult skillActionResult)
    {
        Debug.LogWarning(skillActionResult.actor.Name + " uses " + skillActionResult.skill.Name);

        foreach (var effectResult in skillActionResult.allSkillEffectResult)
        {
            foreach (var effectOnTarget in effectResult.effectsOnTarget)
            {
                if (effectOnTarget.isSuccess)
                {
                    var shouldCritical = effectOnTarget.isCritical;
                    var target = effectOnTarget.target;
                    // Deduct Hp
                    target.CurrentHp = Math.Min(target.MaxHp, Math.Max(0d, target.CurrentHp + effectOnTarget.hpChange));

                    Debug.LogWarning(string.Format("{0}{1} takes {2} damage", shouldCritical ? "Critical! " : "", target.Name,  effectOnTarget.hpChange));
                    Debug.LogWarning(target.Name + " remaining hp " + target.CurrentHp);
                }
            }
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
        var outcomeQueue = ServiceFactory.GetBattleService().GetActionResults(actionQueue, this._mapTiles, this._battleCharacters);

        this.ProcessActionResult(outcomeQueue, BattlePhase.NextRound);
    }
}
