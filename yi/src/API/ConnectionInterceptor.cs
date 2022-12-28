using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class ConnectionInterceptor : DbConnectionInterceptor
{
    public override InterceptionResult ConnectionClosing(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
    {
        return base.ConnectionClosing(connection, eventData, result);
    }
    public override ValueTask<InterceptionResult> ConnectionClosingAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
    {
        return base.ConnectionClosingAsync(connection, eventData, result);
    }

    public override InterceptionResult ConnectionDisposing(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
    {
        return base.ConnectionDisposing(connection, eventData, result);
    }
    public override void ConnectionDisposed(DbConnection connection, ConnectionEndEventData eventData)
    {
        base.ConnectionDisposed(connection, eventData);
    }
}
