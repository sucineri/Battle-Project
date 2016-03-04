﻿using UnityEngine;
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

    public enum SkillType
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
        MaxHp,
        MaxMp,
        Attack,
        Defense,
        Agility,
        Wisdom,
        Mind,
        Critical,
        Accuracy,
        Evasion
    }

    public enum Affinities
    {
        Physical,
        Healing,
        Lightning
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
