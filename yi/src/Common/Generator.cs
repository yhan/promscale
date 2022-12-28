using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
// using Index = blazor_pivottable.Pages.Index;

namespace Common;

public class Generator
{
    private static string[] topLevelStrategyOptions = { "VWAP", "TWAP", "WVOL", "ECLIPSE" };
    private static string[] strategyOptions = { "Hit", "Quote", "DualQuote", "Fixing" };
    private static Random rand = new();
    private static string[] wayOptions = { "Buy", "Sell" };
    private static string[] instanceOptions = { "vm-1", "vm-paris", "vm-london", "vm-hongkong" };
    private static string[] venueOptions = { "ChiX", "ENX", "ENA-main", "GER-main" };
    private static string[] counterpartyOptions = { "cli-a", "cli-b", "cli-c" };
    private readonly int size;
    private readonly List<string> idBuffer;
    private Dictionary<double, double> distributionFactors;
    private int factorWalker;
    private int preferIndexVenueCategoryCountDown;
    private int preferIndexTopLevelStrategyCountDown;
    private int preferIndexInstrumentCountDown;
    private int preferIndexWayCountDown;
    private int preferVenueCountDown;
    private int preferIndexVenueTypeCountDown;

    public Generator(IConfiguration config = null)
    {
        this.size = config == null ? 1 : int.Parse(config["dataSize"]);
        // 30% de orders *0.2
        // 10% de orders *1.3
        distributionFactors = new()
        {
            [0.2] = size * 0.3,
            [1.3] = size * 1.3
        };
        preferIndexVenueCategoryCountDown = size * 40;
        preferIndexTopLevelStrategyCountDown = size * 55;
        preferIndexInstrumentCountDown = size * 65;
        preferIndexWayCountDown = size * 70;
        preferVenueCountDown = size * 45;
        preferIndexVenueTypeCountDown = size * 40;
        idBuffer = new List<string>();
        for (int i = 0; i < size; i++)
        {
            idBuffer.Add(Guid.NewGuid().ToString());
        }
    }

    public List<NominalMetric> Metrics()
    {
        var mkt = Execute();
        return mkt.MapToMetrics();
    }

    public List<MarketOrderVm> Execute()
    {
        var collection = new List<MarketOrderVm>();
        for (int i = 0; i < size; i++)
        {
            double coef = (double)(i+1) / size;
            
            var id = i%7==0 ? Guid.NewGuid().ToString(): idBuffer[i];
            var marketOrderVm = OneMarketOrderVm(id, coef);
            
            collection.Add(marketOrderVm);
        }

        return collection;
    }

    private MarketOrderVm OneMarketOrderVm(string id, double coef)
    {
        double factor = 1;
        var keys = distributionFactors.Keys.ToList();
        if (distributionFactors[keys[factorWalker]] == 0)
            factorWalker++;

        if(factorWalker <= keys.Count - 1)
        {
            factor = distributionFactors[keys[factorWalker]];
            distributionFactors[keys[factorWalker]] -= 1;
        }
        
        double execNom = Math.Round(rand.NextDouble() * 10_000, MidpointRounding.ToZero) * coef * factor;
        return new MarketOrderVm(
        id,
        Select(topLevelStrategyOptions, PreferredIndex(preferIndexTopLevelStrategyCountDown, 0)),
        Select(strategyOptions, PreferredIndex(preferIndexInstrumentCountDown, 1, 2)),
        Select(wayOptions, PreferredIndex(preferIndexWayCountDown, 0)),
        execNom: execNom,
        Select(instanceOptions),
        Select(counterpartyOptions),
        Select(Enum.GetValues<InstrumentType>(), PreferredIndex(preferIndexInstrumentCountDown, 0)),
        Select(Enum.GetValues<VenueCategory>(), PreferredIndex(preferIndexVenueCategoryCountDown, 0,3)),
        Select(venueOptions, PreferredIndex(preferVenueCountDown, 0,2)),
        Select(Enum.GetValues<VenueType>(), PreferredIndex(preferIndexVenueTypeCountDown, 0, 2)),
        RandomDateTimeOffset());
    }
    
    private int? PreferredIndex(int countDown, params int[] preferredIndexes)
    {
        if (countDown > 0)
        {
            var preferred = rand.Next(0, preferredIndexes.Length);
            preferIndexVenueCategoryCountDown--;
            return preferred;
        }

        return null;
    }
    
    private DateTimeOffset RandomDateTimeOffset()
    {
        return DateTimeOffset.UtcNow.AddSeconds(-Math.Round(rand.NextDouble() * 1_000_000, MidpointRounding.ToZero));
    }

    private static T Select<T>(T[] array, int? preferIndex = null)
    {
        if (preferIndex != null)
            return array[preferIndex.Value];
        
        return array[rand.Next(0, array.Length)];
    }
}

public enum InstrumentType
{
    Equity,
    Future,
    FutureSpread
}

public enum VenueCategory
{
    LIT,
    DARK,
    DAR_AUCTION
}

public enum VenueType
{
    Main,
    Secondary,
    InternalMarket,
    DarkPool
}