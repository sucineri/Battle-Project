using System.Collections;
using System.Collections.Generic;

public class Pattern 
{
    public List<Cordinate> Shape { get; set; }
	
    public bool IsWholeGrid { get; set; }

    public List<int> WholeRows { get; set; }

    public List<int> WholeColumns { get; set; }

    public Pattern()
    {
        this.IsWholeGrid = false;
        this.WholeRows = new List<int>();
        this.WholeColumns = new List<int>();
        this.Shape = new List<Cordinate>();
    }

    public static Pattern Single()
    {
        var pattern = new Pattern();
        pattern.Shape.Add(new Cordinate(0, 0));
        return pattern;
    }

    public static Pattern Cross()
    {
        var pattern = new Pattern();
        pattern.Shape.Add(new Cordinate(0, 0));
        pattern.Shape.Add(new Cordinate(1, 0));
        pattern.Shape.Add(new Cordinate(-1, 0));
        pattern.Shape.Add(new Cordinate(0, 1));
        pattern.Shape.Add(new Cordinate(0, -1)); 
        return pattern;
    }

    public static Pattern Square()
    {
        var pattern = new Pattern();
        pattern.Shape.Add(new Cordinate(0, 0));
        pattern.Shape.Add(new Cordinate(1, 0));
        pattern.Shape.Add(new Cordinate(0, -1));
        pattern.Shape.Add(new Cordinate(1, -1));
        return pattern;
    }

    public static Pattern WholeGrid()
    {
        var pattern = new Pattern();
        pattern.IsWholeGrid = true;
        return pattern;
    }

    public static Pattern TwoColumns()
    {
        var pattern = new Pattern();
        pattern.WholeColumns.Add(1);
        pattern.WholeColumns.Add(2);
        return pattern;
    }
}
