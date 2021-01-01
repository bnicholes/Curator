using System;
using System.IO;
using System.Runtime.InteropServices;
using curator.Logic;
using CuratorService.Data;
using Serilog;
using Topshelf;
using Topshelf.Runtime.DotNetCore;

namespace CuratorService
{
    internal class Program
    {
        private static readonly string ServiceIdentifier = "CuratorService";
        private static readonly string ServiceDescription =
            "Curate a collection of photos from various sites.";

        private static void Main()
        {
            Directory.CreateDirectory(WellKnownData.ProgramDataPath);
            var logDirPath = Path.Combine(WellKnownData.ProgramDataPath, $"{ServiceIdentifier}.log");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logDirPath,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .Enrich.FromLogContext()
                .MinimumLevel.ControlledBy(LogLevelSwitcher.Instance.LogLevelSwitch)
                .CreateLogger();

            Console.WriteLine($"Curator Service logging to: {logDirPath}");

            HostFactory.Run(hostConfig =>
            {
                hostConfig.UseSerilog();
                hostConfig.Service<CuratorService>(service =>
                {
                    service.ConstructUsing(c => new CuratorService());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    hostConfig.UseEnvironmentBuilder(c => new DotNetCoreEnvironmentBuilder(c));
                }
                hostConfig.StartAutomaticallyDelayed();
                hostConfig.SetDisplayName(ServiceIdentifier);
                hostConfig.SetServiceName(ServiceIdentifier);
                hostConfig.SetDescription(ServiceDescription);
                hostConfig.EnableServiceRecovery(recoveryOption =>
                {
                    recoveryOption.RestartService(0);
                });
            });
        }
    }
}
