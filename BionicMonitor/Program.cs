using System;
using System.IO;
using BionicMonitor.Commands;
using BionicMonitorService.Options;
using BionicMonitorService.Utils;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BionicMonitor {
  [Command(Description = "🤖 Bionic Monitor - Live Reload for Blazor & Bionic projects")]
  [Subcommand(typeof(DocsCommand))]
  [Subcommand(typeof(UninstallCommand))]
  [Subcommand(typeof(UpdateCommand))]
  [Subcommand(typeof(VersionCommand))]
  public class Program {
    [Option("--bionic", Description =
      "Use Bionic root directory .bionic/wwwroot (will not mess with files in wwwroot)")]
    private static bool? bionic { get; }

    [Option("--signalRProvided", Description = "Enable if app already provides SignalR lib")]
    private static bool? signalRProvided { get; }

    [Option("--hostOrIp", Description = "Listening interface - defaults to localhost")]
    private static string hostOrIp { get; }

    [Option("--port", Description = "Serving port number - defaults to 5000")]
    private static string port { get; }

    [Option("--securePort", Description = "Serving secure port number - defaults to 5001")]
    private static string securePort { get; }

    [Option("--destinationRootDir", Description =
      "Destination root directory - defaults to wwwroot or .bionic/wwwroot with --bionic option enabled")]
    private static string destinationRootDir { get; }

    [Option("--sourceRootDir", Description = "Source root directory - defaults to wwwroot")]
    private static string sourceRootDir { get; }

    public static void Main(string[] args) {
      BionicMonitorOptions.args = args;
      CommandLineApplication.Execute<Program>(args);
    }

    private int OnExecute(CommandLineApplication app) {
      initOptions();
      if (!validateOptions()) return 1;
      CreateWebHostBuilder(BionicMonitorOptions.args).Build().Run();
      return 0;
    }

    private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
      WebHost.CreateDefaultBuilder(args)
        .UseUrls(
          $"http://{BionicMonitorOptions.HostOrIp}:{BionicMonitorOptions.Port}",
          $"https://{BionicMonitorOptions.HostOrIp}:{BionicMonitorOptions.SecurePort}")
        .UseStartup<Startup>();

    private static bool validateOptions() {
      if (BionicMonitorOptions.DestinationRootDir.StartsWith(Path.PathSeparator)) {
        Console.WriteLine(
          $"☠  --rootDir paths have to be relative to current directory and cannot start with {Path.PathSeparator}");
        return false;
      }

      if (Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj").Length == 0
          || !Directory.Exists(BionicMonitorOptions.SourceRootDir)
      ) {
        Console.WriteLine($"☠  Current directory {Directory.GetCurrentDirectory()} is not a Blazor/Bionic frontend project.");
        return false;
      }

      var destPath = Path.Combine(Directory.GetCurrentDirectory(), BionicMonitorOptions.DestinationRootDir);
      if (Directory.Exists(destPath)) return true;
      
      var srcPath = Path.Combine(Directory.GetCurrentDirectory(), BionicMonitorOptions.SourceRootDir);
      DirectoryUtils.Copy(srcPath, destPath);

      return true;
    }

    private static void initOptions() {
      BionicMonitorOptions.LoadOptions();
      BionicMonitorOptions.UseBionic = bionic ?? BionicMonitorOptions.UseBionic;
      BionicMonitorOptions.IsSignalRProvided = signalRProvided ?? BionicMonitorOptions.IsSignalRProvided;
      BionicMonitorOptions.HostOrIp = hostOrIp ?? BionicMonitorOptions.HostOrIp;
      BionicMonitorOptions.Port = port ?? BionicMonitorOptions.Port;
      BionicMonitorOptions.SecurePort = securePort ?? BionicMonitorOptions.SecurePort;
      BionicMonitorOptions.DestinationRootDir = BionicMonitorOptions.UseBionic
        ? ".bionic/wwwroot"
        : (destinationRootDir ?? BionicMonitorOptions.DestinationRootDir);
      BionicMonitorOptions.SourceRootDir = sourceRootDir ?? BionicMonitorOptions.SourceRootDir;
    }
  }
}