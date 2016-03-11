using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character
{
    public CharacterStats BaseStats { get; set; }

    public string Name { get; set; }

    public string PortraitPath { get; set; }

    public string ModelPath { get; set; }

    public float AttackDistance { get; set; }

    public float SizeOffset { get; set; }

    public int Movement { get; set; }

    public List<Skill> Skills = new List<Skill>();

    public Pattern PatternShape { get; set; }

    public static Character Fighter()
    {
        var character = new Character();

        var baseStats = new CharacterStats();
        baseStats.SetStat(Const.Stats.MaxHp, 700d);
        baseStats.SetStat(Const.Stats.MaxMp, 150d);
        baseStats.SetStat(Const.Stats.Attack, 300d);
        baseStats.SetStat(Const.Stats.Defense, 150d);
        baseStats.SetStat(Const.Stats.Wisdom, 70d);
        baseStats.SetStat(Const.Stats.Agility, 100d);
        baseStats.SetStat(Const.Stats.Mind, 50d);
        baseStats.SetStat(Const.Stats.Critical, 0.05d);
        baseStats.SetStat(Const.Stats.Accuracy, 1d);
        baseStats.SetStat(Const.Stats.Evasion, 0.1d);

        character.Name = "Fighter";
        character.BaseStats = baseStats;

        character.PortraitPath = "Characters/Fighter/portrait";
        character.ModelPath = "Characters/Fighter/model";
        character.AttackDistance = 1.5f;
        character.SizeOffset = 1.5f;
        character.Skills.Add(Skill.MeleeAttack());
        character.Skills.Add(Skill.CrossSlash());
        character.Skills.Add(Skill.MinorHeal());
        character.Skills.Add(Skill.Wait());
        character.Movement = 2;

        character.PatternShape = Pattern.Single();

        return character;
    }

    public static Character Slime()
    {
        var character = new Character();

        var baseStats = new CharacterStats();
        baseStats.SetStat(Const.Stats.MaxHp, 700d);
        baseStats.SetStat(Const.Stats.MaxMp, 150d);
        baseStats.SetStat(Const.Stats.Attack, 300d);
        baseStats.SetStat(Const.Stats.Defense, 150d);
        baseStats.SetStat(Const.Stats.Wisdom, 70d);
        baseStats.SetStat(Const.Stats.Agility, 100d);
        baseStats.SetStat(Const.Stats.Mind, 50d);
        baseStats.SetStat(Const.Stats.Critical, 0.05d);
        baseStats.SetStat(Const.Stats.Accuracy, 0.8d);
        baseStats.SetStat(Const.Stats.Evasion, 0.1d);

        character.Name = "Slime";
        character.BaseStats = baseStats;

        character.PortraitPath = "Characters/Slime/portrait";
        character.ModelPath = "Characters/Slime/model";
        character.AttackDistance = 2.5f;
        character.SizeOffset = 1.5f;
        character.Skills.Add(Skill.MeleeAttack());
        character.Movement = 2;

        character.PatternShape = Pattern.Single();

        return character;
    }

    public static Character SlimeKing()
    {
        var character = new Character();

        var baseStats = new CharacterStats();
        baseStats.SetStat(Const.Stats.MaxHp, 6000d);
        baseStats.SetStat(Const.Stats.MaxMp, 1500d);
        baseStats.SetStat(Const.Stats.Attack, 600d);
        baseStats.SetStat(Const.Stats.Defense, 150d);
        baseStats.SetStat(Const.Stats.Wisdom, 250d);
        baseStats.SetStat(Const.Stats.Agility, 110d);
        baseStats.SetStat(Const.Stats.Mind, 50d);
        baseStats.SetStat(Const.Stats.Critical, 0.05d);
        baseStats.SetStat(Const.Stats.Accuracy, 1d);
        baseStats.SetStat(Const.Stats.Evasion, 0.1d);

        character.Name = "Slime King";
        character.BaseStats = baseStats;

        character.PortraitPath = "Characters/SlimeKing/portrait";
        character.ModelPath = "Characters/SlimeKing/model";
        character.AttackDistance = 2.5f;
        character.SizeOffset = 1.5f;
        character.Skills.Add(Skill.ChainLightning());
        character.Skills.Add(Skill.Squash());
        character.Skills.Add(Skill.Wait());
        character.Movement = 2;

        character.PatternShape = Pattern.Square();
        return character;
    }

    public static Character ZombieSlimeKing()
    {
        var character = new Character();

        var baseStats = new CharacterStats();
        baseStats.SetStat(Const.Stats.MaxHp, 6000d);
        baseStats.SetStat(Const.Stats.MaxMp, 1500d);
        baseStats.SetStat(Const.Stats.Attack, 600d);
        baseStats.SetStat(Const.Stats.Defense, 150d);
        baseStats.SetStat(Const.Stats.Wisdom, 250d);
        baseStats.SetStat(Const.Stats.Agility, 110d);
        baseStats.SetStat(Const.Stats.Mind, 50d);
        baseStats.SetStat(Const.Stats.Critical, 0.05d);
        baseStats.SetStat(Const.Stats.Accuracy, 1d);
        baseStats.SetStat(Const.Stats.Evasion, 0.1d);

        baseStats.SetStat(Const.Stats.HealingResistance, -2d);
        baseStats.SetStat(Const.Stats.PhysicalResistance, 1d);

        character.Name = "Zombie Slime King";
        character.BaseStats = baseStats;

        character.PortraitPath = "Characters/ZombieSlimeKing/portrait";
        character.ModelPath = "Characters/ZombieSlimeKing/model";
        character.AttackDistance = 2.5f;
        character.SizeOffset = 1.5f;
        character.Skills.Add(Skill.ChainLightning());
        character.Skills.Add(Skill.Squash());
        character.Skills.Add(Skill.Wait());
        character.Movement = 2;

        character.PatternShape = Pattern.Square();
        return character;
    }
}
