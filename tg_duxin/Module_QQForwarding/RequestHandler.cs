using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace tg_duxin.Module_QQForwarding {
    class RequestHandler {
        public void ConfigureServices(IServiceCollection services) {

        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
            app.Run(new Microsoft.AspNetCore.Http.RequestDelegate(HttpRequestHandler));
        }

        private async Task HttpRequestHandler(HttpContext context) {
            byte[] content = new byte[2000];
            context.Request.Body.Read(content,0,2000);
            string ret =
                InterfaceListener.OnMessageRecieved(Encoding.UTF8.GetString(content));
            await context.Response.WriteAsync(ret);
        }
    }
}
