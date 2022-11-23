using System.Diagnostics;
using Demo.Util.Logging;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Demo.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TrackActionPerformance : ActionFilterAttribute
    {
        private Stopwatch _timer;
        private readonly ILogger<TrackActionPerformance> _logger;

        public TrackActionPerformance(ILogger<TrackActionPerformance> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _timer = new Stopwatch();
            _timer.Start();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _timer.Stop();
            if (context.Exception == null)
            {
                _logger.LogRoutePerformance(context.HttpContext.Request.Method, context.HttpContext.Request.Path,
                    _timer.ElapsedMilliseconds);
            }
        }
    }
}