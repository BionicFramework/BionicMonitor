using BionicMonitor.Utils;
using McMaster.Extensions.CommandLineUtils;

namespace BionicMonitor.Commands {
  [Command("uninstall", Description = "Initiate Bionic self-destruct sequence")]
  public class UninstallCommand : CommandBase {
    protected override int OnExecute(CommandLineApplication app) => UninstallBionic();

    public int Execute() => UninstallBionic();

    private static int UninstallBionic() => DotNetHelper.RunDotNet("tool uninstall -g BionicMonitor");
  }
}