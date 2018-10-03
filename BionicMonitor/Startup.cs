using System.Linq;
using System.Net.Mime;
using BionicMonitorService;
using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

namespace BionicMonitor {
  public class Startup {
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
      services.AddResponseCompression(options => {
        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {
          MediaTypeNames.Application.Octet,
          WasmMediaTypeNames.Application.Wasm
        });
      });
      services.AddSignalR();
      services.AddBionicMonitorService();
    }

    public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, IHostingEnvironment env) {
      app.UseResponseCompression()
        .UseDefaultFiles()
        .UseBionicMonitor(applicationLifetime);
    }
  }
}