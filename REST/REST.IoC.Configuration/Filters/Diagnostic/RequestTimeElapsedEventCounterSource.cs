using System.Diagnostics.Tracing;

namespace REST.IoC.Configuration.Filters.Diagnostic
{
    [EventSource(Name = "RequestTimeElapsedEventCounterSource")]
    public sealed class RequestTimeElapsedEventCounterSource : EventSource
    {
        public static readonly RequestTimeElapsedEventCounterSource logger = new RequestTimeElapsedEventCounterSource();

        private EventCounter _requestTimeCounter;

        private RequestTimeElapsedEventCounterSource() => _requestTimeCounter = new EventCounter("request-time", this)
        {
            DisplayName = "Requesrt processing name",
            DisplayUnits = "ms",
        };

        public void Request(string url, float timeElapsedMilliseconds)
        {
            WriteEvent(1, url, timeElapsedMilliseconds);
            _requestTimeCounter?.WriteMetric(timeElapsedMilliseconds);
        }

        protected override void Dispose(bool disposing)
        {
            _requestTimeCounter?.Dispose();
            _requestTimeCounter = null;
            base.Dispose(disposing);
        }
    }
}
