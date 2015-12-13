using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleActionOutcome 
{
	public class OutcomePerTrigger
	{
		public List<OutcomePerTarget> OutcomeForTargets = new List<OutcomePerTarget>();

		public void AddTargetOutcome(OutcomePerTarget targetOutcome)
		{
			this.OutcomeForTargets.Add (targetOutcome);
		}
	}

	public class OutcomePerTarget
	{
		public BattleCharacter Target;
		public double HpChange;
		public MapPosition PositionChangeTo;
	}

	public Const.ActionType Type;
	public OutcomePerTarget ActorOutcome;
	public List<OutcomePerTrigger> AllOutcomes = new List<OutcomePerTrigger>();

	public void AddTriggerOutcome(OutcomePerTrigger triggerOutcome)
	{
		this.AllOutcomes.Add (triggerOutcome);
	}
}
