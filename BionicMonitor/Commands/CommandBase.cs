using McMaster.Extensions.CommandLineUtils;

namespace BionicMonitor.Commands {
  public abstract class CommandBase {
    [HelpOption("-?|-h|--help")]
    protected bool IsHelp { get; }

    public CommandBase Subcommand { get; set; }

    protected abstract int OnExecute(CommandLineApplication app);
  }
}