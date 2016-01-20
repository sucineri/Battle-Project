using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EffectTarget
{
    public Const.SkillTargetGroup TargetGroup { get; set; }

    public Const.SkillTargetType TargetType { get; set; }

    public Const.TargetSearchRule TargetSearchRule { get; set; }

    public List<Cordinate> Pattern { get; set; }

    public static EffectTarget SingleOpponentTarget()
    {
        var pattern = new EffectTarget();
        pattern.TargetGroup = Const.SkillTargetGroup.Opponent;
        pattern.TargetType = Const.SkillTargetType.Unit;
        pattern.TargetSearchRule = Const.TargetSearchRule.SelectedTarget;

        pattern.Pattern = new List<Cordinate>();
        pattern.Pattern.Add(new Cordinate(0, 0));
        return pattern;
    }

    public static EffectTarget ChainLightning()
    {
        var pattern = new EffectTarget();
        pattern.TargetGroup = Const.SkillTargetGroup.Opponent;
        pattern.TargetType = Const.SkillTargetType.Unit;
        pattern.TargetSearchRule = Const.TargetSearchRule.SelectedTarget;

        pattern.Pattern = new List<Cordinate>();
        pattern.Pattern.Add(new Cordinate(0, 0));
        return pattern;
    }

    public static EffectTarget ChainLightningSecondary()
    {
        var pattern = new EffectTarget();
        pattern.TargetGroup = Const.SkillTargetGroup.Opponent;
        pattern.TargetType = Const.SkillTargetType.Unit;
        pattern.TargetSearchRule = Const.TargetSearchRule.Nearest;

        pattern.Pattern = new List<Cordinate>();
        pattern.Pattern.Add(new Cordinate(0, 0));
        return pattern;
    }

    public static EffectTarget CrossOpponentTarget()
    {
        var pattern = new EffectTarget();
        pattern.TargetGroup = Const.SkillTargetGroup.Opponent;
        pattern.TargetType = Const.SkillTargetType.Unit;
        pattern.TargetSearchRule = Const.TargetSearchRule.SelectedTarget;

        pattern.Pattern = new List<Cordinate>();
        pattern.Pattern.Add(new Cordinate(0, 0));
        pattern.Pattern.Add(new Cordinate(1, 0));
        pattern.Pattern.Add(new Cordinate(-1, 0));
        pattern.Pattern.Add(new Cordinate(0, 1));
        pattern.Pattern.Add(new Cordinate(0, -1));
        return pattern;
    }

    public static EffectTarget SquashTarget()
    {
        var pattern = new EffectTarget();
        pattern.TargetGroup = Const.SkillTargetGroup.Opponent;
        pattern.TargetType = Const.SkillTargetType.Unit;
        pattern.TargetSearchRule = Const.TargetSearchRule.SelectedTarget;

        pattern.Pattern = AllCordinates();
        return pattern;
    }

    private static List<Cordinate> AllCordinates()
    {
        var pattern = new HashSet<Cordinate>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                pattern.Add(new Cordinate(i, j));
                pattern.Add(new Cordinate(-i, j));
                pattern.Add(new Cordinate(i, -j));
                pattern.Add(new Cordinate(-i, -j));
            }
        }
        return pattern.ToList();
    }
}
