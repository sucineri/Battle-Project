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
    public List<TurnOrderData> GetActionOrder(List<BattleCharacter> allCharacters)
    {
        // first we create an ordered list of temp structures to hold the information
        var orderedList = this.CreateOrderedList(allCharacters);

        // then we tick until the first character is ready for action.
        var minTicks = orderedList.First().cooldown;
        foreach (var characterOrder in orderedList)
        {
            characterOrder.Tick(minTicks);
        }

        // then we predict the future of the next x turns for each character, using default skill rank
        var futureList = this.PredictFuture(orderedList, Const.PredictTurns, Const.DefaultSkillRank);

        orderedList.AddRange(futureList);
        this.OrderByCooldown(orderedList);

        return orderedList.Take(Const.DisplayTurns).ToList();
    }

    private List<TurnOrderData> PredictFuture(List<TurnOrderData> initialList, int iterations, int defaultRank)
    {
        List<TurnOrderData> futureList = new List<TurnOrderData>();

        var newList = new List<TurnOrderData>(initialList);

        foreach (var characterOrder in newList)
        {
            var character = characterOrder.character;
            var cooldown = characterOrder.cooldown;

            for(int i = 0; i < iterations; i++)
            {
                cooldown = cooldown + this.GetAddedCooldown(character, defaultRank);
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
            if(x.character.BaseCharacter.Agility != y.character.BaseCharacter.Agility ) 
            {
                return y.character.BaseCharacter.Agility.CompareTo(x.character.BaseCharacter.Agility);
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
