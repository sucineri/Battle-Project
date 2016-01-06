using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Targetting
{
    public Const.SkillTargetGroup TargetGroup { get; set; }

    public Const.SkillTargetType TargetType { get; set; }

    public List<Cordinate> Pattern { get; set; }

    protected Targetting()
    {
        // no public default constructor
    }

    public static Targetting SingleOpponentTarget()
    {
        var pattern = new Targetting();
        pattern.TargetGroup = Const.SkillTargetGroup.Opponent;
        pattern.TargetType = Const.SkillTargetType.Unit;

        pattern.Pattern = new List<Cordinate>();
        pattern.Pattern.Add(new Cordinate(0, 0));
        return pattern;
    }

    public static Targetting CrossOpponentTarget()
    {
        var pattern = new Targetting();
        pattern.TargetGroup = Const.SkillTargetGroup.Opponent;
        pattern.TargetType = Const.SkillTargetType.Unit;

        pattern.Pattern = new List<Cordinate>();
        pattern.Pattern.Add(new Cordinate(0, 0));
        pattern.Pattern.Add(new Cordinate(1, 0));
        pattern.Pattern.Add(new Cordinate(-1, 0));
        pattern.Pattern.Add(new Cordinate(0, 1));
        pattern.Pattern.Add(new Cordinate(0, -1));
        return pattern;
    }

    public static Targetting SquashTarget()
    {
        var pattern = new Targetting();
        pattern.TargetGroup = Const.SkillTargetGroup.Opponent;
        pattern.TargetType = Const.SkillTargetType.Unit;

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
