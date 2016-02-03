using UnityEngine;
using System.Collections;

public class Const
{
    public enum Team
    {
        Player = 1,
        Enemy = 2
    }

    public enum SkillTargetGroup
    {
        None,
        Ally,
        Opponent,
        Self
    }

    public enum SkillTargetType
    {
        Unit,
        Tile
    }

    public enum TargetSearchRule
    {
        SelectedTarget,
        Nearest
    }

    public enum BasicStats
    {
        MaxHp,
        MaxMp,
        Attack,
        Defense,
        Agility,
        Wisdom,
        Mind
    }

    public enum ActionType
    {
        Movement,
        Skill,
        Item
    }

    public enum TargetType
    {
        Tile,
        Character
    }

    public enum EnmityTargetType
    {
        Target,
        All
    }

    public const string TileKey = "Tile";

    public const int DefaultSkillRank = 3;

    public static string GetTileKey(Const.Team team, int x, int y)
    {
        return string.Format("{0}_{1}_{2}_{3}", team, Const.TileKey, x, y);
    }
}
