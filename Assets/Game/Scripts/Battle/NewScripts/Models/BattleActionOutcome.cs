using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO: rewrite these crap
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
		public BattleCharacter target;
		public double hpChange;
		public MapPosition positionChangeTo;
		public string effectPrefabPath;
	}

	public Const.ActionType type;
	public OutcomePerTarget actorOutcome;
	public MapPosition targetPosition;
	public BattleCharacter targeteCharacter;
	public BattleCharacter actor;
	public List<OutcomePerTrigger> allOutcomes = new List<OutcomePerTrigger>();

	public void AddTriggerOutcome(OutcomePerTrigger triggerOutcome)
	{
		this.allOutcomes.Add (triggerOutcome);
	}
}
