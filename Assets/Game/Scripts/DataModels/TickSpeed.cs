using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TickSpeed 
{
    public double MinAgility { get; set; }
    public int Speed { get; set; }

    private static List<TickSpeed> _tickSpeedMap = new List<TickSpeed>(){
        new TickSpeed() { MinAgility = 170d, Speed = 3 },
        new TickSpeed() { MinAgility = 98d, Speed = 4 },
        new TickSpeed() { MinAgility = 62d, Speed = 5 },
        new TickSpeed() { MinAgility = 44d, Speed = 6 },
        new TickSpeed() { MinAgility = 35d, Speed = 7 },
        new TickSpeed() { MinAgility = 29d, Speed = 8 },
        new TickSpeed() { MinAgility = 23d, Speed = 9 },
        new TickSpeed() { MinAgility = 19d, Speed = 10 },
        new TickSpeed() { MinAgility = 17d, Speed = 11 },
        new TickSpeed() { MinAgility = 15d, Speed = 12 },
        new TickSpeed() { MinAgility = 12d, Speed = 13 },
        new TickSpeed() { MinAgility = 10d, Speed = 14 },
        new TickSpeed() { MinAgility = 7d, Speed = 15 },
        new TickSpeed() { MinAgility = 5d, Speed = 16 },
        new TickSpeed() { MinAgility = 4d, Speed = 20 },
        new TickSpeed() { MinAgility = 3d, Speed = 22 },
        new TickSpeed() { MinAgility = 2d, Speed = 24 },
        new TickSpeed() { MinAgility = 1d, Speed = 26 },
        new TickSpeed() { MinAgility = 0d, Speed = 28 }
    };

    public static int GetTickSpeed(double agility)
    {
        if (agility < 0)
        {
            return 0;
        }
        var tickSpeed = _tickSpeedMap.First(x => x.MinAgility <= agility);
        return tickSpeed.Speed;
    }
}
