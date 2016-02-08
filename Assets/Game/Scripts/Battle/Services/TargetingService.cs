using UnityEngine;
using System.Collections;

public class TargetingService 
{
    public bool IsInTargetGroup(Const.SkillTargetGroup targetGroup, Const.Team source, Const.Team target)
    {   
        switch (targetGroup)
        {
            case Const.SkillTargetGroup.All:
                return true;
            case Const.SkillTargetGroup.Ally:
            case Const.SkillTargetGroup.Self:
                return source == target;
            case Const.SkillTargetGroup.Opponent:
                return source != target;
            default:
                return false;
        }
    }
}