using System;
using System.Collections;

public class StatusEffect  
{
    public enum Type
    {
        Blind = 201,
        LightningResistanceUp = 301,
        Poison = 401
    }

    public enum Category
    {
        CharacterStatChange,
        SpecialState
    }

    public Category StatusEffectCategory { get; set; }

    public Type StatusEffectType { get; set; }

    public int AffectedFieldId { get; set; }

    public double EffectMagnitude { get; set; }

    public Const.ModifierType ModifierType { get; set; }

    public int EffectRank { get; set; }

    public int EffectDuration { get; set; }

    public static StatusEffect Blind()
    {
        return new StatusEffect() { 
            StatusEffectCategory = Category.CharacterStatChange,
            StatusEffectType = Type.Blind,
            AffectedFieldId = (int)Const.Stats.Accuracy,
            ModifierType = Const.ModifierType.Multiply,
            EffectMagnitude = 0.5d,
            EffectRank = 1,
            EffectDuration = 2
        };
    }

    public static StatusEffect LightningResistanceUp()
    {
        return new StatusEffect() { 
            StatusEffectCategory = Category.CharacterStatChange,
            StatusEffectType = Type.LightningResistanceUp,
            AffectedFieldId = (int)Const.Stats.LightningResistance,
            ModifierType = Const.ModifierType.Addition,
            EffectMagnitude = 0.5d,
            EffectRank = 1,
            EffectDuration = 2
        };
    }

    public static StatusEffect Poison()
    {
        return new StatusEffect() { 
            StatusEffectCategory = Category.SpecialState,
            StatusEffectType = Type.Poison,
            AffectedFieldId = (int)Const.SpecialState.Poison,
            ModifierType = Const.ModifierType.Addition,
            EffectMagnitude = 0.05d,
            EffectRank = 1,
            EffectDuration = 2
        };
    }
}
