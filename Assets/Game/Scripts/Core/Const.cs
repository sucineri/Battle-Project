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
        Self,
        All
    }

    public enum SkillEffectType
    {
        Attack,
        Heal,
        Buff,
        Debuff
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

    public enum Stats
    {
        MaxHp = 1,
        MaxMp = 2,
        Attack = 3,
        Defense = 4,
        Agility = 5,
        Wisdom = 6,
        Mind = 7,
        Critical = 8,
        Accuracy = 9,
        Evasion = 10,
        PhysicalResistance = 101,
        HealingResistance = 102,
        FireResistance = 103,
        IceResistance = 104,
        WaterResistance = 105,
        WindResistance = 106,
        EarthResistance = 107,
        LightningResistance = 108,
        HolyResistance = 109,
        DarkResistance = 110,
        BlindResistance = 201
    }

    public enum Affinities
    {
        Physical = 101,
        Healing = 102,
        Fire = 103,
        Ice = 104,
        Water = 105,
        Wind = 106,
        Earth = 107,
        Lightning = 108,
        Holy = 109,
        Dark = 110
    }

    public enum StatusEffectTypes
    {
        Blind = 201,
        LightningResistanceUp = 301,
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

    public enum ModifierType
    {
        Addition,
        Multiply,
        Absolute
    }

    public const string TileKey = "Tile";

    public const int DefaultSkillRank = 3;

    public const double CriticalDamageMultiplier = 1.5d;

    public const string MissedText = "Missed";

    public const int PredictTurns = 5;

    public const int DisplayTurns = 10;

    public static string GetTileKey(Const.Team team, int x, int y)
    {
        return string.Format("{0}_{1}_{2}_{3}", team, Const.TileKey, x, y);
    }
}
