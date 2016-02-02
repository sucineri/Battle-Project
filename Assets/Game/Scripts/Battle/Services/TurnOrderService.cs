using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnOrderService
{
    public List<BattleCharacter> GetActionOrder(List<BattleCharacter> allCharacters)
    {
        var actionableCharacters = this.GetActionableCharacters(allCharacters);
        this.OrderByCooldown(actionableCharacters);

        var minTicks = actionableCharacters.First().ActionCooldown;
        foreach (var character in actionableCharacters)
        {
            character.Tick(minTicks);
        }

        return actionableCharacters;
    }

    public void ApplySkillCooldownToCharacter(BattleCharacter character, Skill skillUsed)
    {
        character.ActionCooldown = character.Speed * skillUsed.Rank;
    }

    public void ApplyDefaultCooldownToCharacter(BattleCharacter character)
    {
        character.ActionCooldown = character.Speed * Const.DefaultSkillRank;
    }

    private void OrderByCooldown(List<BattleCharacter> actionableCharacters)
    {
        actionableCharacters.Sort((x, y) =>{
            if(x.ActionCooldown != y.ActionCooldown)
            {
                return x.ActionCooldown.CompareTo(y.ActionCooldown);
            }
            if(x.BaseCharacter.Agility != y.BaseCharacter.Agility ) 
            {
                return y.BaseCharacter.Agility.CompareTo(x.BaseCharacter.Agility);
            }
            return y.BattleCharacterId.CompareTo(x.BattleCharacterId);
        });
    }

    private List<BattleCharacter> GetActionableCharacters(List<BattleCharacter> allCharacters)
    {
        return allCharacters.FindAll(u => !u.IsDead);
    }
}
