using BionicMonitorService.Utils;
using McMaster.Extensions.CommandLineUtils;

namespace BionicMonitor.Commands {
  [Command(Description = "Open Bionic Monitor documentation page in browser")]
  public class DocsCommand : CommandBase {
    protected override int OnExecute(CommandLineApplication app) => OpenBlazorDocs();

    private static int OpenBlazorDocs() {
      var browser = BrowserUtils.OpenUrl("https://bmsantos.github.io/bionic/live-reload");
      browser?.WaitForExit();
      return browser?.ExitCode ?? 1;
    }
  }
}