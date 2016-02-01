using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnmityService  
{
    private const int InitialEnmityLevel = 0;

    public void InitEnmity(List<BattleCharacter> allCharacters)
    {
        var playerCharacters = allCharacters.FindAll(x => x.Team == Const.Team.Player);
        var enemyCharacters = allCharacters.Except(playerCharacters).ToList();

        foreach (var enemyCharacter in enemyCharacters)
        {
            enemyCharacter.Enmity.InitEnmityList(playerCharacters, InitialEnmityLevel);
        }

        foreach (var playerCharacter in playerCharacters)
        {
            playerCharacter.Enmity.InitEnmityList(enemyCharacters, InitialEnmityLevel);
        }
    }

    public void ApplyEnmityForSkill(BattleCharacter actor, Skill skill, List<BattleCharacter> affectedCharacters, List<BattleCharacter> allCharacters)
    {
        switch (skill.EnmityType)
        {
            case Const.EnmityTargetType.Target:
                this.ApplyEnmityToCharacters(actor, skill, affectedCharacters);
                break;
            case Const.EnmityTargetType.All:
                var targets = allCharacters.FindAll(x => x.Team != actor.Team);
                this.ApplyEnmityToCharacters(actor, skill, targets);
                break;
        }
    }

    private void ApplyEnmityToCharacters(BattleCharacter actor, Skill skill, List<BattleCharacter> targets)
    {
        foreach (var target in targets)
        {
            this.ApplyEnmityToCharacter(actor, skill, target);
        }
    }

    private void ApplyEnmityToCharacter(BattleCharacter actor, Skill skill, BattleCharacter target)
    {
        // TODO: better enmity calculation logic
        var enmityDelta = skill.BaseEnmity;
        target.Enmity.ChangeEnmityLevel(actor, enmityDelta);
    }
}
