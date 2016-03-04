﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character
{
    public BasicStats BasicStats { get; set; }

    public Affinities Resistances { get; set; }

    public string Name { get; set; }

    public string PortraitPath { get; set; }

    public string ModelPath { get; set; }

    public float AttackDistance { get; set; }

    public float SizeOffset { get; set; }

    public double MaxHp { get { return this.BasicStats.GetStats(Const.Stats.MaxHp); } }

    public double MaxMp { get { return this.BasicStats.GetStats(Const.Stats.MaxMp); } }

    public double Attack { get { return this.BasicStats.GetStats(Const.Stats.Attack); } }

    public double Defense { get { return this.BasicStats.GetStats(Const.Stats.Defense); } }

    public double Wisdom{ get { return this.BasicStats.GetStats(Const.Stats.Wisdom); } }

    public double Agility { get { return this.BasicStats.GetStats(Const.Stats.Agility); } }

    public double Mind { get { return this.BasicStats.GetStats(Const.Stats.Mind); } }

    public double Critical { get { return this.BasicStats.GetStats(Const.Stats.Critical); } }

    public double Accuracy { get { return this.BasicStats.GetStats(Const.Stats.Accuracy); } }

    public double Evasion { get { return this.BasicStats.GetStats(Const.Stats.Evasion); } }

    public int Movement { get; set; }

    public List<Skill> Skills = new List<Skill>();

    public Pattern PatternShape { get; set; }

    public static Character Fighter()
    {
        var random = new System.Random();
        var character = new Character();
        var maxHp = random.Next(500, 700);
        var maxMp = random.Next(100, 150);
        var atk = random.Next(200, 300);
        var def = random.Next(100, 150);
        var agi = random.Next(50, 100);
        var wis = random.Next(30, 80);
        var mnd = random.Next(30, 50);
        var crit = 0.05d;
        var acc = 1d;
        var eva = 0.1d;
        character.Name = "Fighter";
        character.BasicStats = new BasicStats(maxHp, maxMp, atk, def, agi, wis, mnd, crit, acc, eva);
        character.Resistances = new Affinities(0d, 2d, 0d);
        character.PortraitPath = "Characters/Fighter/portrait";
        character.ModelPath = "Characters/Fighter/model";
        character.AttackDistance = 1.5f;
        character.SizeOffset = 1.5f;
        character.Skills.Add(Skill.MeleeAttack());
        character.Skills.Add(Skill.CrossSlash());
        character.Skills.Add(Skill.MinorHeal());
        character.Movement = 2;

        character.PatternShape = Pattern.Single();

        return character;
    }

    public static Character Slime()
    {
        var random = new System.Random();
        var character = new Character();
        var maxHp = random.Next(500, 700);
        var maxMp = random.Next(100, 150);
        var atk = random.Next(200, 300);
        var def = random.Next(100, 150);
        var agi = random.Next(50, 100);
        var wis = random.Next(30, 80);
        var mnd = random.Next(30, 50);
        var crit = 0.05d;
        var acc = 0.8d;
        var eva = 0.1d;
        character.Name = "Slime";
        character.BasicStats = new BasicStats(maxHp, maxMp, atk, def, agi, wis, mnd, crit, acc, eva);
        character.Resistances = new Affinities(0d, 2d, 0d);
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
        var random = new System.Random();
        var character = new Character();
        var maxHp = random.Next(5000, 7000);
        var maxMp = random.Next(1000, 1500);
        var atk = random.Next(500, 700);
        var def = random.Next(100, 150);
        var agi = random.Next(50, 100);
        var wis = random.Next(200, 250);
        var mnd = random.Next(30, 50);
        var crit = 0.05d;
        var acc = 1d;
        var eva = 0.1d;
        character.Name = "Slime King";
        character.BasicStats = new BasicStats(maxHp, maxMp, atk, def, agi, wis, mnd, crit, acc, eva);
        character.Resistances = new Affinities(0d, 2d, 0d);
        character.PortraitPath = "Characters/SlimeKing/portrait";
        character.ModelPath = "Characters/SlimeKing/model";
        character.AttackDistance = 2.5f;
        character.SizeOffset = 1.5f;
        character.Skills.Add(Skill.ChainLightning());
        character.Skills.Add(Skill.Squash());
        character.Movement = 2;

        character.PatternShape = Pattern.Square();
        return character;
    }

    public static Character ZombieSlimeKing()
    {
        var random = new System.Random();
        var character = new Character();
        var maxHp = random.Next(5000, 7000);
        var maxMp = random.Next(1000, 1500);
        var atk = random.Next(500, 700);
        var def = random.Next(100, 150);
        var agi = random.Next(50, 100);
        var wis = random.Next(200, 250);
        var mnd = random.Next(30, 50);
        var crit = 0.05d;
        var acc = 1d;
        var eva = 0.1d;
        character.Name = "Slime King";
        character.BasicStats = new BasicStats(maxHp, maxMp, atk, def, agi, wis, mnd, crit, acc, eva);
        character.Resistances = new Affinities(1d, -2d, 0d);
        character.PortraitPath = "Characters/ZombieSlimeKing/portrait";
        character.ModelPath = "Characters/ZombieSlimeKing/model";
        character.AttackDistance = 2.5f;
        character.SizeOffset = 1.5f;
        character.Skills.Add(Skill.ChainLightning());
        character.Skills.Add(Skill.Squash());
        character.Movement = 2;

        character.PatternShape = Pattern.Square();
        return character;
    }
}
