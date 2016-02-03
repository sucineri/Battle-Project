using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Targeting
{
    public Const.SkillTargetGroup TargetGroup { get; set; }

    public Const.SkillTargetType TargetType { get; set; }

    public Const.TargetSearchRule TargetSearchRule { get; set; }

    public Pattern Pattern { get; set; }

    #region Skill Effect Target 
    public static Targeting SingleOpponentTarget()
    {
        var targeting = new Targeting();
        targeting.TargetGroup = Const.SkillTargetGroup.Opponent;
        targeting.TargetType = Const.SkillTargetType.Unit;
        targeting.TargetSearchRule = Const.TargetSearchRule.SelectedTarget;

        targeting.Pattern = Pattern.Single();
        return targeting;
    }

    public static Targeting SingleAllyTarget()
    {
        var targeting = new Targeting();
        targeting.TargetGroup = Const.SkillTargetGroup.Ally;
        targeting.TargetType = Const.SkillTargetType.Unit;
        targeting.TargetSearchRule = Const.TargetSearchRule.SelectedTarget;

        targeting.Pattern = Pattern.Single();
        return targeting;
    }

    public static Targeting ChainLightning()
    {
        var targeting = new Targeting();
        targeting.TargetGroup = Const.SkillTargetGroup.Opponent;
        targeting.TargetType = Const.SkillTargetType.Unit;
        targeting.TargetSearchRule = Const.TargetSearchRule.SelectedTarget;

        targeting.Pattern = Pattern.Single();
        return targeting;
    }

    public static Targeting ChainLightningSecondary()
    {
        var targeting = new Targeting();
        targeting.TargetGroup = Const.SkillTargetGroup.Opponent;
        targeting.TargetType = Const.SkillTargetType.Unit;
        targeting.TargetSearchRule = Const.TargetSearchRule.Nearest;

        targeting.Pattern = Pattern.Single();
        return targeting;
    }

    public static Targeting CrossOpponentTarget()
    {
        var targeting = new Targeting();
        targeting.TargetGroup = Const.SkillTargetGroup.Opponent;
        targeting.TargetType = Const.SkillTargetType.Unit;
        targeting.TargetSearchRule = Const.TargetSearchRule.SelectedTarget;

        targeting.Pattern = Pattern.Cross();
        return targeting;
    }

    public static Targeting SquashTarget()
    {
        var targeting = new Targeting();
        targeting.TargetGroup = Const.SkillTargetGroup.Opponent;
        targeting.TargetType = Const.SkillTargetType.Unit;
        targeting.TargetSearchRule = Const.TargetSearchRule.SelectedTarget;

        targeting.Pattern = Pattern.WholeGrid();
        return targeting;
    }
    #endregion


    #region Skill Targeting Area
    public static Targeting MeleeTargetArea()
    {
        var targeting = new Targeting();
        targeting.TargetGroup = Const.SkillTargetGroup.Opponent;
        targeting.TargetType = Const.SkillTargetType.Tile;
        
        targeting.Pattern = Pattern.TwoColumns();
        return targeting;
    }

    public static Targeting HealTargetArea()
    {
        var targeting = new Targeting();
        targeting.TargetGroup = Const.SkillTargetGroup.Ally;
        targeting.TargetType = Const.SkillTargetType.Tile;

        targeting.Pattern = Pattern.Cross();
        return targeting;
    }
    #endregion
}
