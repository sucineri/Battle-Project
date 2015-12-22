using UnityEngine;
using System;
using System.Collections;

public class Tile  
{
	public enum TileState
	{
		None = 0,
		Selected = 1,
		SkillHighlight = 2,
		MovementHighlight = 4
	}

	private TileState _state;
	public TileState State 
	{ 
		get { 
			return this._state;
		} 
		set {
			this._state = value;
			if (this.onStateChange != null) {
				this.onStateChange (this._state);
			}
		}
	}

	public event Action<TileState> onStateChange;

	public Tile()
	{
		this.State = TileState.None;
	}

	public void AddOrRemoveState(TileState newState, bool isAdd)
	{
		if (isAdd) 
		{
			this.State = this.State | newState;
		} 
		else 
		{
			this.State = this.State ^ (this.State & newState);
		}
	}
}
