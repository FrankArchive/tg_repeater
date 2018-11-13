using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using Microsoft.AspNetCore.Hosting;

namespace tg_duxin.Module_QQForwarding {
    class Reciever {
        private IWebHost host;

        public Reciever(params string[] prefixes) {
            host = new WebHostBuilder().
                UseKestrel().
                UseUrls(prefixes).
                UseStartup<RequestHandler>().
                Build();
            host.Start();
        }
        public async void Stop() {
            await host.StopAsync();
        }
    }
}
