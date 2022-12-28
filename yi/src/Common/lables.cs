namespace Common;

public class lables
{
    public string __name__ { get; set; }
    public string Id { get; set; }
    public string InstrumentType { get; set; }
    public string Counterparty { get; set; }
    public string VenueCategory { get; set; }
    public string Way { get; set; }
    public string StrategyName { get; set; }
    public string VenueType { get; set; }
    public string Venue { get; set; }
    public string TopLevelStrategyName { get; set; }
}

public class NominalMetric
{
    public lables labels;
    public object[] samples;
}
