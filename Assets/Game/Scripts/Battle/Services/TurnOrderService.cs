using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnOrderService
{
    public List<BattleCharacter> GetActionOrder(List<BattleCharacter> allCharacters)
    {
        var actionableCharacters = this.GetActionableCharacters(allCharacters);
        if (!this.AnyCharacterReadyToAct(actionableCharacters))
        {
            this.TickTilAllCharactersActionReady(actionableCharacters);
        }
        return this.OrderByAtbPoints(actionableCharacters);
    }

    private void TickTilAllCharactersActionReady(List<BattleCharacter> actionableCharacters)
    {
        var ticks = this.GetTicksTilAllActionReady(actionableCharacters);
        foreach (var character in actionableCharacters)
        {
            character.Tick(ticks);
        }
    }

    private List<BattleCharacter> OrderByAtbPoints(List<BattleCharacter> actionableCharacters)
    {
        return actionableCharacters.OrderByDescending(c => c.AtbPoints).ToList();
    }

    private bool AnyCharacterReadyToAct(List<BattleCharacter> actionableCharacters)
    {
        return actionableCharacters.Find(c => c.AtbPoints > Const.ActionReadyAtbPoints) != null;
    }

    private List<BattleCharacter> GetActionableCharacters(List<BattleCharacter> allCharacters)
    {
        return allCharacters.FindAll(u => !u.IsDead);
    }

    private int GetTicksTilAllActionReady(List<BattleCharacter> actionableCharacters)
    {
        var maxTicks = 0;
        foreach (var character in actionableCharacters)
        {
            maxTicks = Math.Max(maxTicks, character.TicksTilActionReady);
        }
        return maxTicks;
    }
}
