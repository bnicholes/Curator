using curator.Logic;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CuratorService.Data;
using CuratorService.Extensions;
using CuratorService.Database;

namespace CuratorService
{
    internal class CuratorService
    {
        private readonly IWebHost _host;

        public CuratorService()
        {
            var webSslCert = CheckDefaultCertificate();

            if (webSslCert == null)
            {
                Log.Logger.Error("Failed to find or change the default SSL certificate.");
                Environment.Exit(1);
            }

            Log.Logger.Information($"Thumbprint for {webSslCert.Subject}: {webSslCert.Thumbprint}");
            Log.Logger.Information(webSslCert.ToPemFormat());

            Log.Logger.Information($"Configuration file location: {Path.Combine(WellKnownData.ServiceDirPath, WellKnownData.AppSettings)}.json");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"{Path.Combine(WellKnownData.ServiceDirPath, WellKnownData.AppSettings)}.json",
                    optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var httpsPort = configuration["HttpsPort"] ?? "443";
            var logLevel = configuration["LogLevel"];

            if (logLevel != null)
            {
                if (Enum.TryParse(logLevel, out CuratorLogLevel level))
                {
                    var logLevelSwitch = LogLevelSwitcher.Instance.LogLevelSwitch;

                    switch (level)
                    {
                        case CuratorLogLevel.Information:
                            logLevelSwitch.MinimumLevel = LogEventLevel.Information;
                            break;

                        case CuratorLogLevel.Debug:
                            logLevelSwitch.MinimumLevel = LogEventLevel.Debug;
                            break;
                        case CuratorLogLevel.Error:
                            logLevelSwitch.MinimumLevel = LogEventLevel.Error;
                            break;
                        case CuratorLogLevel.Warning:
                            logLevelSwitch.MinimumLevel = LogEventLevel.Warning;
                            break;
                        case CuratorLogLevel.Fatal:
                            logLevelSwitch.MinimumLevel = LogEventLevel.Fatal;
                            break;
                        case CuratorLogLevel.Verbose:
                            logLevelSwitch.MinimumLevel = LogEventLevel.Verbose;
                            break;
                    }
                }
                else
                {
                    Log.Logger.Error($"{logLevel} is not not a recognized log level. Continuing to use the default log level.");
                }
            }


            _host = new WebHostBuilder()
                .UseSerilog()
                .UseKestrel(options =>
                {
                    if (int.TryParse(httpsPort, out var port) == false)
                        port = 443;
                    Log.Logger.Information($"Binding web server to port: {port}.");
                    options.ListenAnyIP(port, listenOptions =>
                    {
                        listenOptions.UseHttps(webSslCert);
                    });
                })
                .ConfigureServices(services => services.AddAutofac())
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

        }

        public void Start()
        {
            _host.RunAsync();
        }

        public void Stop()
        {
            _host.StopAsync().Wait();
        }

        private X509Certificate2 CheckDefaultCertificate()
        {
            using var db = new CuratorDb();

            var webSslCert = db.WebSslCertificate;
            if (webSslCert != null)
                return webSslCert;

            webSslCert = CertificateHelper.CreateDefaultSslCertificate();
            db.WebSslCertificate = webSslCert;
            Log.Logger.Information("Created and installed a default web ssl certificate.");

            // Need to make sure that we return a db instance of the certificate rather than a local instance
            //  So rather than just returning the webSslCert created above, get a new instance of the certificate
            //  from the database.
            return db.WebSslCertificate;
        }
    }
}
