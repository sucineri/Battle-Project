using System;
using System.Collections;

public class CharacterStatusEffectStatModifier  
{
    public int RemainingTurns { get; private set; }
    public int Rank { get; private set; }
    public StatusEffect.Category StatusEffectCategory { get; private set; }
    public StatusEffect.Type StatusEffectType { get; private set; }
    public StatModifier StatModifier { get; private set; }

    public bool Expired { get { return this.RemainingTurns <= 0; } }

    public CharacterStatusEffectStatModifier(StatusEffect statusEffect)
    {
        this.RemainingTurns = statusEffect.EffectDuration;
        this.Rank = statusEffect.EffectRank;
        this.StatusEffectCategory = statusEffect.StatusEffectCategory;
        this.StatusEffectType = statusEffect.StatusEffectType;
        this.StatModifier = new StatModifier((Const.Stats)statusEffect.AffectedFieldId, statusEffect.EffectMagnitude, statusEffect.ModifierType); 
    }

    public void UpdateTurn()
    {
        this.RemainingTurns --;
    }
}
