using BionicMonitor.Utils;
using McMaster.Extensions.CommandLineUtils;

namespace BionicMonitor.Commands {
  [Command("update", Description = "Update Bionic Monitor to its latest incarnation")]
  public class UpdateCommand : CommandBase {
    protected override int OnExecute(CommandLineApplication app) => UpdateBionic();

    public int Execute() => UpdateBionic();

    private static int UpdateBionic() => DotNetHelper.RunDotNet("tool update -g BionicMonitor");
  }
}