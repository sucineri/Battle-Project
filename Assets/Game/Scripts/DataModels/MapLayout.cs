using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapLayout
{
    public class Spawn
    {
        public MapPosition Position { get; private set; }
        public Character Character { get; private set; }

        public Spawn(MapPosition position, Character character)
        {
            Position = position;
            Character = character;
        }
    }
    public List<Spawn> spawns = new List<Spawn>();

    public static MapLayout GetDefaultLayout()
    {
        var layout = new MapLayout();
        layout.spawns.Add(new Spawn(new MapPosition(1, 0, Const.Team.Player), Character.Fighter()));
        layout.spawns.Add(new Spawn(new MapPosition(1, 1, Const.Team.Player), Character.Fighter()));
        layout.spawns.Add(new Spawn(new MapPosition(1, 2, Const.Team.Player), Character.Fighter()));
        layout.spawns.Add(new Spawn(new MapPosition(1, 3, Const.Team.Player), Character.Fighter()));

        layout.spawns.Add(new Spawn(new MapPosition(1, 0, Const.Team.Enemy), Character.Slime()));
        layout.spawns.Add(new Spawn(new MapPosition(1, 1, Const.Team.Enemy), Character.Slime()));
        layout.spawns.Add(new Spawn(new MapPosition(1, 2, Const.Team.Enemy), Character.Slime()));
        layout.spawns.Add(new Spawn(new MapPosition(1, 3, Const.Team.Enemy), Character.Slime()));
        return layout;
    }

    public static MapLayout BossLayout()
    {
        var layout = new MapLayout();
        layout.spawns.Add(new Spawn(new MapPosition(1, 0, Const.Team.Player), Character.Fighter()));
        layout.spawns.Add(new Spawn(new MapPosition(0, 1, Const.Team.Player), Character.Fighter()));
        layout.spawns.Add(new Spawn(new MapPosition(0, 2, Const.Team.Player), Character.Fighter()));
        layout.spawns.Add(new Spawn(new MapPosition(2, 3, Const.Team.Player), Character.Fighter()));

        layout.spawns.Add(new Spawn(new MapPosition(1, 1, Const.Team.Enemy), Character.SlimeKing()));
        return layout;
    }

    public static MapLayout TwoSlimesLayout()
    {
        var layout = new MapLayout();
        layout.spawns.Add(new Spawn(new MapPosition(1, 0, Const.Team.Player), Character.Fighter()));
        layout.spawns.Add(new Spawn(new MapPosition(0, 1, Const.Team.Player), Character.Fighter()));
        layout.spawns.Add(new Spawn(new MapPosition(0, 2, Const.Team.Player), Character.Fighter()));
        layout.spawns.Add(new Spawn(new MapPosition(2, 3, Const.Team.Player), Character.Fighter()));

        layout.spawns.Add(new Spawn(new MapPosition(0, 1, Const.Team.Enemy), Character.SlimeKing()));
        layout.spawns.Add(new Spawn(new MapPosition(0, 3, Const.Team.Enemy), Character.ZombieSlimeKing()));
        return layout;
    }
}
