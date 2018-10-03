using System;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

namespace BionicMonitor.Commands {
  [Command(Description = "Print Bionic Monitor version number")]
  public class VersionCommand : CommandBase {
    protected override int OnExecute(CommandLineApplication app) => PrintVersion();

    public int Execute() => PrintVersion();

    private static int PrintVersion() {
      var informationalVersion = ((AssemblyInformationalVersionAttribute) Attribute.GetCustomAttribute(
          Assembly.GetExecutingAssembly(), typeof(AssemblyInformationalVersionAttribute), false))
        .InformationalVersion;
      Console.WriteLine($"🤖 Bionic Monitor v{informationalVersion}");
      return 0;
    }
  }
}