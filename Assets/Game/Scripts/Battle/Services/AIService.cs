using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIService
{
    public Queue<BattleAction> RunAI(BattleCharacter actor, Dictionary<MapPosition, Tile> map, List<BattleCharacter> allCharacters)
    {
        Queue<BattleAction> queue = new Queue<BattleAction>();
        queue.Enqueue(this.RandomMovement(actor, map, allCharacters));
        queue.Enqueue(this.RandomTarget(actor, map, allCharacters));
        return queue;
    }

    private BattleAction RandomMovement(BattleCharacter actor, Dictionary<MapPosition, Tile> map, List<BattleCharacter> allCharacters)
    {
        var movablePosition = ServiceFactory.GetMapService().GetMovablePositions(actor, allCharacters, map);

        if (movablePosition.Count <= 0)
        {
            return null;
        }
			
        var targetPosition = this.GetRandomElement<MapPosition>(movablePosition);

        return new BattleAction(actor, Const.ActionType.Movement, Const.TargetType.Tile, targetPosition, null);
    }

    private BattleAction RandomTarget(BattleCharacter actor, Dictionary<MapPosition, Tile> map, List<BattleCharacter> allCharacters)
    {
        var allOpponents = allCharacters.FindAll(x => x.Team != actor.Team);
        if (allOpponents.Count <= 0)
        {
            return null;
        }

        var targetCharacter = this.GetRandomElement<BattleCharacter>(allOpponents);
        var targetPosition = targetCharacter.OccupiedMapPositions;

        var randomSkill = this.GetRandomElement<Skill>(actor.BaseCharacter.Skills);
        return new BattleAction(actor, Const.ActionType.Skill, Const.TargetType.Tile, targetPosition, randomSkill);
    }

    private T GetRandomElement<T>(List<T> list) where T : class
    {
        Random random = new Random();
        var index = random.Next(0, list.Count - 1);
        return list[index];
    }
}
