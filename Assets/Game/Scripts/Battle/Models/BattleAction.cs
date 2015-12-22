using UnityEngine;
using System.Collections;

public class BattleAction
{

    public BattleCharacter Actor { get; private set; }

    public Const.ActionType ActionType { get; private set; }

    public Const.TargetType TargetType { get; private set; }

    public MapPosition TargetPosition { get; private set; }

    public Skill SelectedSkill { get; private set; }

    public BattleAction(BattleCharacter actor, Const.ActionType actionType, Const.TargetType targetType, MapPosition targetPosition, Skill selectedSkill)
    {
        this.Actor = actor;
        this.ActionType = actionType;
        this.TargetType = targetType;
        this.TargetPosition = targetPosition;
        this.SelectedSkill = selectedSkill;
    }
}
