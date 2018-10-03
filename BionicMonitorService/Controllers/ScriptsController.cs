using Microsoft.AspNetCore.Mvc;

namespace BionicMonitorService.Controllers {
  public class ScriptsController : Controller {
    private const string _content = @"
var connection = new signalR.HubConnectionBuilder().withUrl(""/reloadHub"").build();

connection.on(""ReceiveMessage"", function (message) {
  if (message === 'reload') {
    console.log('Reloading Page...');
    location.reload();
  }
});

connection.start().catch(function (err) {
  return console.error(err.toString());
});
";

    [HttpGet("/bionic/reloader.js")]
    public IActionResult GetReloaderLib() => Content(_content);
  }
}