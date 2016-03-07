public struct ModifiedStat
{
    public Const.Stats Stat { get; private set; }
    public double Value { get; private set; }
    public bool IsAbsolute { get; private set; }

    public ModifiedStat(Const.Stats stat, double value, bool isAbsolute)
    {
        this.Value = value;
        this.IsAbsolute = isAbsolute;
        this.Stat = stat;
    }
}
