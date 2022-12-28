namespace Common;

public class MarketOrderVm
{
    public MarketOrderVm(string id, string topLevelStrategyName, string strategyName, string way, double execNom,
        string instanceId,
        string counterparty, InstrumentType instrumentType, VenueCategory venueCategory, string venueId
        , VenueType venueType, DateTimeOffset timestamp)
    {
        Id = id;
        TopLevelStrategyName = topLevelStrategyName;
        StrategyName = strategyName;
        Way = way;
        ExecNom = execNom;
        InstanceId = instanceId;
        Counterparty = counterparty;
        InstrumentType = instrumentType;
        VenueCategory = venueCategory;
        VenueId = venueId;
        VenueType = venueType;
        Timestamp = timestamp;
        TimestampES = Timestamp.ToString("yyyyMMdd'T'hhmmssZ");
        EpochSeconds = timestamp.ToUnixTimeSeconds();
    }
    public long EpochSeconds { get; set; }
    public string TimestampES { get; set; }

    public MarketOrderVm()
    {
    }
    public  DateTimeOffset Timestamp { get; set; }

    public string StrategyName { get; set; }
    public string Way { get;  set;}
    public double ExecNom { get;  set;}
    public string InstanceId { get;  set;}
    public string Counterparty { get;  set;}
    public InstrumentType InstrumentType { get;  set;}
    public VenueCategory VenueCategory { get;  set;}
    public string VenueId { get;  set;}
    public VenueType VenueType { get;  set;}

    public string Id { get; set; }
    public string TopLevelStrategyName { get; set; }
}
