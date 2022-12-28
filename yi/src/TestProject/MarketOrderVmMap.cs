using System.Globalization;
using Common;
using CsvHelper.Configuration;

namespace TestProject;

public sealed class MarketOrderVmMap : ClassMap<MarketOrderVm>
{
    public MarketOrderVmMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
        Map(m => m.Timestamp).Ignore();
        Map(m => m.TimestampES).Ignore();
    }
}
