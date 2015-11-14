using UnityEngine;
using System.Collections;

public class Character 
{
    public string Name { get; set; }
    public int MaxHp { get; set; }
    public int CurrentHp { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Wisdom { get; set; }
    public int Agility { get; set; }
    public string PortraitPath { get; set; }
    public string ModelPath { get; set; }
    public int NumberOfAttacks { get; set; }
    public float AttackDelay { get; set; }
    public float AttackDistance { get; set; }
    public float SizeOffset { get; set; }

    protected Character()
    {
    }

    public static Character Fighter()
    {
        var random = new System.Random();
        var character = new Character();
        character.Name = "Fighter";
        character.MaxHp = random.Next(500, 700);
        character.CurrentHp = character.MaxHp;
        character.Attack = random.Next(200, 300);
        character.Defense = random.Next(100, 150);
        character.Wisdom = random.Next(30, 80);
        character.Agility = random.Next(50, 100);
        character.PortraitPath = "Fighter/portrait";
        character.ModelPath = "Fighter/model";
        character.NumberOfAttacks = 2;
        character.AttackDelay = 0.2f;
        character.AttackDistance = 1.5f;
        character.SizeOffset = 1f;
        return character;
    }

    public static Character Slime()
    {
        var random = new System.Random();
        var character = new Character();
        character.Name = "Slime";
        character.MaxHp = random.Next(500, 700);
        character.CurrentHp = character.MaxHp;
        character.Attack = random.Next(200, 300);
        character.Defense = random.Next(100, 150);
        character.Wisdom = random.Next(30, 80);
        character.Agility = random.Next(50, 100);
        character.PortraitPath = "Slime/portrait";
        character.ModelPath = "Slime/model";
        character.NumberOfAttacks = 1;
        character.AttackDelay = 0.2f;
        character.AttackDistance = 2.5f;
        character.SizeOffset = 1f;
        return character;
    }
}
