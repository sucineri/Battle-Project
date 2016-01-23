using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EffectTarget
{
    public Const.SkillTargetGroup TargetGroup { get; set; }

    public Const.SkillTargetType TargetType { get; set; }

    public Const.TargetSearchRule TargetSearchRule { get; set; }

    public Pattern Pattern { get; set; }

    public static EffectTarget SingleOpponentTarget()
    {
        var effectTarget = new EffectTarget();
        effectTarget.TargetGroup = Const.SkillTargetGroup.Opponent;
        effectTarget.TargetType = Const.SkillTargetType.Unit;
        effectTarget.TargetSearchRule = Const.TargetSearchRule.SelectedTarget;

        effectTarget.Pattern = Pattern.Single();
        return effectTarget;
    }

    public static EffectTarget ChainLightning()
    {
        var effectTarget = new EffectTarget();
        effectTarget.TargetGroup = Const.SkillTargetGroup.Opponent;
        effectTarget.TargetType = Const.SkillTargetType.Unit;
        effectTarget.TargetSearchRule = Const.TargetSearchRule.SelectedTarget;

        effectTarget.Pattern = Pattern.Single();
        return effectTarget;
    }

    public static EffectTarget ChainLightningSecondary()
    {
        var effectTarget = new EffectTarget();
        effectTarget.TargetGroup = Const.SkillTargetGroup.Opponent;
        effectTarget.TargetType = Const.SkillTargetType.Unit;
        effectTarget.TargetSearchRule = Const.TargetSearchRule.Nearest;

        effectTarget.Pattern = Pattern.Single();
        return effectTarget;
    }

    public static EffectTarget CrossOpponentTarget()
    {
        var effectTarget = new EffectTarget();
        effectTarget.TargetGroup = Const.SkillTargetGroup.Opponent;
        effectTarget.TargetType = Const.SkillTargetType.Unit;
        effectTarget.TargetSearchRule = Const.TargetSearchRule.SelectedTarget;

        effectTarget.Pattern = Pattern.Cross();
        return effectTarget;
    }

    public static EffectTarget SquashTarget()
    {
        var effectTarget = new EffectTarget();
        effectTarget.TargetGroup = Const.SkillTargetGroup.Opponent;
        effectTarget.TargetType = Const.SkillTargetType.Unit;
        effectTarget.TargetSearchRule = Const.TargetSearchRule.SelectedTarget;

        effectTarget.Pattern = Pattern.WholeGrid();
        return effectTarget;
    }
}
