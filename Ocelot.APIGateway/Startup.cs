using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.APIGateway.ExtensionsHelper;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ocelot.APIGateway
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot();
            //services.ConfigureDownstreamHostAndPortsPlaceholders(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseHttpsRedirection();

                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    //endpoints.MapGet("/", async context =>
                    //{
                    //    await context.Response.WriteAsync("Hello World!");
                    //});
                });

                await app.UseOcelot();
                //app.UseOcelot().Wait();

            }
            catch (Exception ex)
            {
                Console.WriteLine("error on middleware pipeline is --- " + ex.Message + " and stack trace "+ ex.StackTrace+ " inner exception: "+ ex.InnerException);
                throw;
            }
           

        }
    }
}
