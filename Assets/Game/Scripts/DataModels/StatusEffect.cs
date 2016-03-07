using System;
using System.Collections;

public class StatusEffect  
{
    public Const.StatusEffectTypes StatusEffectType { get; set; }

    public int AffectedFieldId { get; set; }

    public double EffectMagnitude { get; set; }

    public Const.ModifierType ModifierType { get; set; }

    public int EffectRank { get; set; }

    public int EffectDuration { get; set; }

    public static StatusEffect Blind()
    {
        return new StatusEffect() { 
            StatusEffectType = Const.StatusEffectTypes.Blind,
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
            StatusEffectType = Const.StatusEffectTypes.LightningResistanceUp,
            AffectedFieldId = (int)Const.Stats.LightningResistance,
            ModifierType = Const.ModifierType.Addition,
            EffectMagnitude = 0.5d,
            EffectRank = 1,
            EffectDuration = 2
        };
    }
}
