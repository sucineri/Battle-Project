using UnityEngine;
using System.Collections;

public class ServiceFactory
{
    private static AIService _aiService = new AIService();
    private static MapService _mapService = new MapService();
    private static BattleService _battleService = new BattleService();
    private static TurnOrderService _turnOrderService = new TurnOrderService();
    private static UnitNameService _unitNameService = new UnitNameService();
    private static EnmityService _enmityService = new EnmityService();

    public static AIService GetAIService()
    {
        return _aiService;
    }

    public static MapService GetMapService()
    {
        return _mapService;
    }

    public static BattleService GetBattleService()
    {
        return _battleService;
    }

    public static TurnOrderService GetTurnOrderService()
    {
        return _turnOrderService;
    }

    public static UnitNameService GetUnitNameService()
    {
        return _unitNameService;
    }

    public static EnmityService GetEnmityService()
    {
        return _enmityService;
    }
}
