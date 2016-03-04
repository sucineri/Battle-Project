using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TurnOrderData = BattleTurnOrderModel.TurnOrderData;

public class TurnOrderService
{
    /// <summary>
    /// Gets the action order.
    /// </summary>
    /// <returns>The action order. With prediction of 2 turns in the future</returns>
    /// <param name="allCharacters">a copied list of characters to sort</param>
    public List<TurnOrderData> GetActionOrder(List<BattleCharacter> allCharacters, bool tickTilReady = true, Queue<BattleActionResult> predictedResults = null)
    {
        // first we create an ordered list of temp structures to hold the information
        var orderedList = this.CreateOrderedList(allCharacters);

        // then we tick until the first character is ready for action.
        if (tickTilReady)
        {
            var minTicks = orderedList.First().cooldown;
            foreach (var characterOrder in orderedList)
            {
                characterOrder.Tick(minTicks);
            }
        }

        // then we predict the future of the next x turns for each character, using default skill rank
        var futureList = this.PredictFuture(orderedList, Const.PredictTurns, Const.DefaultSkillRank, predictedResults);

        orderedList.AddRange(futureList);

        this.OrderByCooldown(orderedList);

        return orderedList.Take(Const.DisplayTurns).ToList();
    }

    public List<TurnOrderData> PredictFuture(List<TurnOrderData> initialList, int iterations, int defaultRank, Queue<BattleActionResult> predictedResults)
    {
        List<TurnOrderData> futureList = new List<TurnOrderData>();

        // rank of the skill the actor is going to use. 
        var actorActionRank = defaultRank;

        // we use the rank of the selected skill if there's predicted result, otherwise we assume the actor will use a default rank skill
        if (predictedResults != null && predictedResults.Count > 0)
        {
            var firstResult = predictedResults.Peek();
            if (firstResult.skill != null && firstResult.actor != null && firstResult.actor.BattleCharacterId == initialList[0].character.BattleCharacterId)
            {
                actorActionRank = firstResult.skill.Rank;
            }
        }

        var newList = new List<TurnOrderData>(initialList);
        var actor = newList.ElementAt(0).character;

        foreach (var characterOrder in newList)
        {
            var character = characterOrder.character;
            var cooldown = characterOrder.cooldown;

            for(int i = 0; i < iterations; i++)
            {
                var skillRank = defaultRank;

                // we use the selected action for the first iteration for the actor
                if (i == 0 && actor == character)
                {
                    skillRank = actorActionRank;
                }

                cooldown = cooldown + this.GetAddedCooldown(character, skillRank);
                futureList.Add(new TurnOrderData(character, cooldown));
            }
        }
        return futureList;
    }

    public void ApplySkillCooldownToCharacter(BattleCharacter character, Skill skillUsed)
    {
        character.ActionCooldown = this.GetAddedCooldown(character, skillUsed.Rank);
    }

    public void ApplyDefaultCooldownToCharacter(BattleCharacter character)
    {
        character.ActionCooldown = this.GetAddedCooldown(character, Const.DefaultSkillRank);
    }

    private int GetAddedCooldown(BattleCharacter character, int skillRank)
    {
        return character.Weight * skillRank;
    }

    private void OrderByCooldown(List<TurnOrderData> turnOrder)
    {
        turnOrder.Sort((x, y) =>{
            if(x.cooldown != y.cooldown)
            {
                return x.cooldown.CompareTo(y.cooldown);
            }

            var xAgility = x.character.GetStat(Const.Stats.Agility);
            var yAgility = y.character.GetStat(Const.Stats.Agility);
            if(xAgility != yAgility) 
            {
                return yAgility.CompareTo(xAgility);
            }
            return y.character.BattleCharacterId.CompareTo(x.character.BattleCharacterId);
        });
    }

    private List<TurnOrderData> CreateOrderedList(List<BattleCharacter> allCharacters)
    {
        var list = new List<TurnOrderData>();
        foreach (var character in allCharacters.FindAll(u => !u.IsDead))
        {
            list.Add(new TurnOrderData(character, character.ActionCooldown));
        }

        this.OrderByCooldown(list);

        return list;
    }
}
