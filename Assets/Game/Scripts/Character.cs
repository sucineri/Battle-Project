using UnityEngine;
using System.Collections;

public class Character 
{
    public int MaxHp { get; set; }
    public int CurrentHp { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Wisdom { get; set; }
    public int Agility { get; set; }

    protected Character () {}

    public static Character Fighter ()
    {
        var character = new Character();
        character.MaxHp = 500;
        character.CurrentHp = 500;
        character.Attack = 250;
        character.Defense = 100;
        character.Wisdom = 50;
        character.Agility = 70;
        return character;
    }

}
