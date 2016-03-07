using System;
using System.Collections;

public class CharacterStatusEffectStatModifier  
{
    public int RemainingTurns { get; set; }
    public int Rank { get; set; }
    public Const.StatusEffectTypes StatusEffectType { get; set; }
    public StatModifier StatModifier { get; set; }

    public CharacterStatusEffectStatModifier(StatusEffect statusEffect)
    {
        this.RemainingTurns = statusEffect.EffectDuration;
        this.Rank = statusEffect.EffectRank;
        this.StatusEffectType = statusEffect.StatusEffectType;
        this.StatModifier = new StatModifier((Const.Stats)statusEffect.AffectedFieldId, statusEffect.EffectMagnitude, statusEffect.ModifierType); 
    }
}
