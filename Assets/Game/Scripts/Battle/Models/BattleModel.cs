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

    // stores the pre calculated action result when a skill is first selected and use for both determining turn order updates and the actual character update when the action is confirmed
    private Queue<BattleActionResult> _currentActionResults = new Queue<BattleActionResult>();

    private List<MapPosition> _currentMovablePositions;

    private List<MapPosition> _validPositionsForSelectedSkill;

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
        this._currentMovablePositions = new List<MapPosition>();
        this._validPositionsForSelectedSkill = new List<MapPosition>();
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

                this._validPositionsForSelectedSkill = ServiceFactory.GetMapService().GetValidMapPositionsForSkill(skill, this.CurrentActor, this._mapTiles, this._battleCharacters);
                this.SetTileStateAtPositions(this._validPositionsForSelectedSkill, Tile.TileState.SkillRadius, true);

                // selects default target if there's any
                var defaultTargetPosition = ServiceFactory.GetSkillService().SelectDefaultTargetForSkill(this.CurrentActor, skill, this._validPositionsForSelectedSkill, this._battleCharacters, this._mapTiles);
                if (defaultTargetPosition != null)
                {
                    this.PreCalculateActionResults(defaultTargetPosition);
                }
            }
        }
    }

    public void CancelLastSelection()
    {
        // TODO: this might require some major refactoring to know what the previous action was
        var allMapPositions = this._mapTiles.Keys.ToList();

        this.SetTileStateAtPositions(allMapPositions, Tile.TileState.SkillRadius, false);
        this.SetTileStateAtPositions(allMapPositions, Tile.TileState.SkillHighlight, false);
        this.UnsetCharacterSelectedSkillTarget(this.CurrentActor);
        this._validPositionsForSelectedSkill.Clear();
        this._currentActionResults.Clear();
        this.ChangePhase(BattlePhase.ActionSelect);
        this.turnOrderModel.UpdateTurnOrder(this.AllBattleCharacters, null);
    }

    public void MoveCurrentCharacter(MapPosition targetPosition)
    {
        if (this.CurrentActor != null && this._currentMovablePositions != null)
        {
            if (this._currentMovablePositions.Contains(targetPosition))
            {
                var action = new BattleAction(this.CurrentActor, Const.ActionType.Movement, Const.TargetType.Tile, targetPosition, null);
                var actionQueue = new Queue<BattleAction>();
                actionQueue.Enqueue(action);
                var resultQueue = ServiceFactory.GetBattleService().GetActionResults(actionQueue, this._mapTiles, this._battleCharacters);
                this.ProcessActionResult(resultQueue, BattlePhase.ActionSelect);
            }
        }
    }

    public void TriggerCurrentSelectedSkill(MapPosition targetPosition)
    {
        if (!this._validPositionsForSelectedSkill.Contains(targetPosition))
        {
            return;
        }

        if(this.CurrentActor != null && this.CurrentActor.SelectedSkill != null)
        {
            if (targetPosition.Equals(this.CurrentActor.SkillTargetPosition))
            {
                // fire off skill
                var isValidAction = false;
                foreach (var result in this._currentActionResults)
                {
                    if (result.HasResult)
                    {
                        isValidAction = true;
                        break;
                    }
                }

                if (isValidAction)
                {
                    this.SetTileStateAtPositions(this._validPositionsForSelectedSkill, Tile.TileState.SkillRadius, false);
                    this._validPositionsForSelectedSkill.Clear();

                    this.ProcessActionResult(this._currentActionResults, BattlePhase.NextRound, () => {
                        this.UnsetCharacterSelectedSkillTarget(this.CurrentActor);
                        this.CurrentActor.UpateStatusEffectTurns();
                    });
                }
            }
            else
            {
                // update the target position and visual
                this.PreCalculateActionResults(targetPosition);
            }
        }
    }

    private void PreCalculateActionResults(MapPosition targetPosition)
    {
        if(this.CurrentActor != null && this.CurrentActor.SelectedSkill != null)
        {
            var selectedSkill = this.CurrentActor.SelectedSkill;

            var action = new BattleAction(this.CurrentActor, Const.ActionType.Skill, Const.TargetType.Tile, targetPosition, selectedSkill);
            var actionQueue = new Queue<BattleAction>();
            actionQueue.Enqueue(action);

            this._currentActionResults = ServiceFactory.GetBattleService().GetActionResults(actionQueue, this._mapTiles, this._battleCharacters);
            this.turnOrderModel.UpdateTurnOrder(this.AllBattleCharacters, this._currentActionResults);

            var isValidResult = false;
            foreach (var result in this._currentActionResults)
            {
                if (result.HasResult)
                {
                    isValidResult = true;
                    break;
                }
            }

            if (isValidResult)
            {
                this.SetCharacterSelectedSkillTarget(this.CurrentActor, targetPosition);
            }
            else
            {
                this.UnsetCharacterSelectedSkillTarget(this.CurrentActor);
            }
        }
    }

    public List<MapPosition> GetMapPositionsForPattern(Pattern pattern, Const.SkillTargetGroup targetGroup, Const.Team sourceTeam, MapPosition basePosition)
    {
        return ServiceFactory.GetMapService().GeMapPositionsForPattern(pattern, targetGroup, sourceTeam, this._mapTiles, basePosition);
    }

    public BattleCharacter GetCharacterAtPosition(MapPosition position)
    {
        return ServiceFactory.GetBattleService().GetCharacterAtPosition(this._battleCharacters, position);
    }

    private List<MapPosition> GetPositionsAffectsByCharacterSelectedSkill(BattleCharacter character)
    {
        var result = new List<MapPosition>();
        if (character != null)
        {
            var skill = character.SelectedSkill;
            var targetPosition = character.SkillTargetPosition;
            if (skill != null && targetPosition != null)
            {
                var targeting = skill.Effects[0].EffectTarget;
                result = this.GetMapPositionsForPattern(targeting.Pattern, targeting.TargetGroup, character.Team, targetPosition);
            }
        }
        return result;
    }

    private bool AllCharactersDefeated(Const.Team team)
    {
        return this.AllBattleCharacters.Find(x => x.Team == team && !x.IsDead) == null;
    }

    private void NextRound()
    {
        this.SetTileStateAtPositions(this._currentMovablePositions, Tile.TileState.MovementHighlight, false);

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

        this._currentMovablePositions = ServiceFactory.GetMapService().GetMovablePositions(this.CurrentActor, this._battleCharacters, this._mapTiles);

        this.SetTileStateAtPositions(this._currentMovablePositions, Tile.TileState.MovementHighlight, true);

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
            if (callback != null)
            {
                callback();
            }
            this.ChangePhase(nextPhase);
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
        var actor = skillActionResult.actor;
        var skill = skillActionResult.skill;
        Debug.LogWarning(actor.Name + " uses " + skill.Name);

        foreach (var effectResult in skillActionResult.allSkillEffectResult)
        {
            var affectedCharacters = this.ApplyActionEffectResult(effectResult);

            var effect = effectResult.effectsOnTarget.ElementAt(0).skillEffect;
            ServiceFactory.GetEnmityService().ApplyEnmityForSkillEffect(actor, effect, affectedCharacters, this._battleCharacters);
        }

        if (skillActionResult.PostActionEffectResult != null)
        {
            this.ApplyActionEffectResult(skillActionResult.PostActionEffectResult);
        }

        ServiceFactory.GetTurnOrderService().ApplySkillCooldownToCharacter(actor, skill);
    }

    private List<BattleCharacter> ApplyActionEffectResult(BattleActionResult.ActionEffectResult actionEffectResult)
    {
        var affectedCharacters = new List<BattleCharacter>();
        foreach (var effectOnTarget in actionEffectResult.effectsOnTarget)
        {
            var target = effectOnTarget.target;

            if (effectOnTarget.isEmptyEffect)
            {
                affectedCharacters.Add(target);
                continue;
            }

            if (effectOnTarget.isSuccess)
            {
                var shouldCritical = effectOnTarget.isCritical;

                // Deduct Hp
                target.CurrentHp = Math.Min(target.MaxHp, Math.Max(0d, target.CurrentHp + effectOnTarget.hpChange));

                Debug.LogWarning(string.Format("{0}{1} takes {2} damage", shouldCritical ? "Critical! " : "", target.Name,  effectOnTarget.hpChange));
                Debug.LogWarning(target.Name + " remaining hp " + target.CurrentHp);

                if (effectOnTarget.statusEffectResult != null)
                {
                    foreach (var statusEffect in effectOnTarget.statusEffectResult.landedEffects)
                    {
                        target.ApplyStatusEffect(statusEffect);
                    }
                }

                affectedCharacters.Add(target);
            }
        }
        return affectedCharacters;
    }

    private void SetTileStateAtPositions(List<MapPosition> positions, Tile.TileState state, bool flag)
    {
        foreach (var position in positions)
        {
            this.SetTileStateAtPosition(position, state, flag);
        }
    }

    private void SetTileStateAtPosition(MapPosition position, Tile.TileState state, bool flag)
    {
        if (position != null && this._mapTiles.ContainsKey(position))
        {
            this._mapTiles[position].AddOrRemoveState(state, flag);
        }
    }

    private void ShowSelectedSkillTarget(BattleCharacter character, bool show)
    {
        this.SetTileStateAtPositions(this.GetPositionsAffectsByCharacterSelectedSkill(character), Tile.TileState.SkillHighlight, show);
        this.SetTileStateAtPosition(character.SkillTargetPosition, Tile.TileState.Target, show);
    }

    private void SetCharacterSelectedSkillTarget(BattleCharacter character, MapPosition target)
    {
        this.ShowSelectedSkillTarget(character, false);
        character.SkillTargetPosition = target;
        this.ShowSelectedSkillTarget(character, true);
    }

    private void UnsetCharacterSelectedSkillTarget(BattleCharacter character)
    {
        this.ShowSelectedSkillTarget(character, false);
        character.SkillTargetPosition = null;
    }

    private void RunAI(BattleCharacter actor)
    {
        var actionQueue = ServiceFactory.GetAIService().RunAI(actor, this._mapTiles, this._battleCharacters);
        var outcomeQueue = ServiceFactory.GetBattleService().GetActionResults(actionQueue, this._mapTiles, this._battleCharacters);

        this.ProcessActionResult(outcomeQueue, BattlePhase.NextRound);
    }
}
