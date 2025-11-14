using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace Automation.Core.Logging
{
    public class LoggingProvider
    {
        private static Serilog.ILogger _logger = null!;
        private static ILoggerFactory _loggerFactory = null!;

        public static LoggingProvider Instance { get; } = new();

        public Serilog.ILogger Logger { get; }
        public ILoggerFactory LoggerFactory { get; }

        private LoggingProvider()
        {
            _logger ??= ConfigureLogger;
            _loggerFactory ??= new SerilogLoggerFactory(_logger);
            Logger = _logger;
            LoggerFactory = _loggerFactory;
        }

        private static Serilog.ILogger ConfigureLogger => new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .CreateLogger();
    }
}