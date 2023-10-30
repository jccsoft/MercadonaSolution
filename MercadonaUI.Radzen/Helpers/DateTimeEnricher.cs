using Serilog.Core;
using Serilog.Events;

namespace MercadonaUI.RadzenServer.Helpers;

public class DateTimeEnricher : ILogEventEnricher
{
    private const string _timeFormat = "HH:mm:ss.fff";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
            "SpainTimeStamp", TimeZoneInfo.ConvertTime(logEvent.Timestamp, TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time")).ToString(_timeFormat)));
    }
}
