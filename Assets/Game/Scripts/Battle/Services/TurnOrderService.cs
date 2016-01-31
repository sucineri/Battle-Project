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
        var orderedList = this.OrderByCooldown(actionableCharacters);

        var minTicks = orderedList.First().ActionCooldown;
        foreach (var character in actionableCharacters)
        {
            character.Tick(minTicks);
        }

        return orderedList;
    }

    public void ApplySkillCooldownToCharacter(BattleCharacter character, Skill skillUsed)
    {
        character.ActionCooldown = character.Speed * skillUsed.Rank;
    }

    public void ApplyDefaultCooldownToCharacter(BattleCharacter character)
    {
        character.ActionCooldown = character.Speed * Const.DefaultSkillRank;
    }

    private List<BattleCharacter> OrderByCooldown(List<BattleCharacter> actionableCharacters)
    {
        return actionableCharacters.OrderBy(c => c.ActionCooldown)
            .ThenByDescending(c => c.BaseCharacter.Agility)
            .ThenByDescending(c => c.BattleCharacterId)
            .ToList();
    }

    private List<BattleCharacter> GetActionableCharacters(List<BattleCharacter> allCharacters)
    {
        return allCharacters.FindAll(u => !u.IsDead);
    }
}
