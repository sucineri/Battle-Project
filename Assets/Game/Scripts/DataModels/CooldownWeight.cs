using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CooldownWeight 
{
    public double MinAgility { get; set; }
    public int Weight { get; set; }

    private static List<CooldownWeight> _weightMap = new List<CooldownWeight>(){
        new CooldownWeight() { MinAgility = 170d, Weight = 3 },
        new CooldownWeight() { MinAgility = 98d, Weight = 4 },
        new CooldownWeight() { MinAgility = 62d, Weight = 5 },
        new CooldownWeight() { MinAgility = 44d, Weight = 6 },
        new CooldownWeight() { MinAgility = 35d, Weight = 7 },
        new CooldownWeight() { MinAgility = 29d, Weight = 8 },
        new CooldownWeight() { MinAgility = 23d, Weight = 9 },
        new CooldownWeight() { MinAgility = 19d, Weight = 10 },
        new CooldownWeight() { MinAgility = 17d, Weight = 11 },
        new CooldownWeight() { MinAgility = 15d, Weight = 12 },
        new CooldownWeight() { MinAgility = 12d, Weight = 13 },
        new CooldownWeight() { MinAgility = 10d, Weight = 14 },
        new CooldownWeight() { MinAgility = 7d, Weight = 15 },
        new CooldownWeight() { MinAgility = 5d, Weight = 16 },
        new CooldownWeight() { MinAgility = 4d, Weight = 20 },
        new CooldownWeight() { MinAgility = 3d, Weight = 22 },
        new CooldownWeight() { MinAgility = 2d, Weight = 24 },
        new CooldownWeight() { MinAgility = 1d, Weight = 26 },
        new CooldownWeight() { MinAgility = 0d, Weight = 28 }
    };

    public static int GetWeight(double agility)
    {
        if (agility < 0)
        {
            return 0;
        }
        var weight = _weightMap.First(x => x.MinAgility <= agility);
        return weight.Weight;
    }
}
