using UnityEngine;
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

	public MapPosition MapPosition { get; set; }
	public TileState State { get; private set; }

	public Tile(MapPosition mapPostion)
	{
		this.MapPosition = mapPostion;
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
