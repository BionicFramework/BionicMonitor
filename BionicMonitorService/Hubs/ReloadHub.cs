using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BionicMonitorService.Hubs {
  public class ReloadHub : Hub {
    public async Task SendMessage(string message) =>
      await Clients.All.SendAsync("ReceiveMessage", message);
  }
}