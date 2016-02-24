using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BattleTurnOrderModel 
{
    public event Action<List<TurnOrderData>> onTurnOrderChanged;

    private List<TurnOrderData> _turnOrder = new List<TurnOrderData>();

    private List<TurnOrderData> _updatedTurnOrder = new List<TurnOrderData>();

    public BattleCharacter GetCurrentActor()
    {
        if (this._turnOrder.Count > 0)
        {
            return this._turnOrder.ElementAt(0).character;
        }
        return null;    
    }

    public BattleCharacter GetNextEnemyActor()
    {
        var first = this._turnOrder.First(x => x.character.Team == Const.Team.Enemy);
        return first != null ? first.character : null;
    }

    public void CalculateActionOrder(List<BattleCharacter> allCharacters)
    {
        this._turnOrder = ServiceFactory.GetTurnOrderService().GetActionOrder(allCharacters, true);
        this._updatedTurnOrder = null;
        this.NotifyTurnOrderChanged();
    }

    public void UpdateTurnOrder(List<BattleCharacter> allCharacters, Queue<BattleActionResult> actionResults)
    {
        if (actionResults != null && actionResults.Count > 0)
        {
            this._updatedTurnOrder = ServiceFactory.GetTurnOrderService().GetActionOrder(allCharacters, false, actionResults);
        }
        else
        {
            this._updatedTurnOrder = null;
        }
        this.NotifyTurnOrderChanged();
    }

    private void NotifyTurnOrderChanged()
    {
        if (this.onTurnOrderChanged != null)
        {
            var orderToShow = (this._updatedTurnOrder != null) ? this._updatedTurnOrder : this._turnOrder;
            this.onTurnOrderChanged(orderToShow);
        }
    }

    public class TurnOrderData
    {
        public BattleCharacter character;
        public int cooldown;

        public TurnOrderData(BattleCharacter battleCharacter, int actionCooldown)
        {
            this.character = battleCharacter;
            this.cooldown = actionCooldown;
        }

        public void Tick(int ticks)
        {
            this.character.Tick(ticks);
            this.cooldown = this.character.ActionCooldown;
        }
    }
}
