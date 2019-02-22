using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TvMazeScraper.Api.Lib;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace TvMazeScraper.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Add background services for scraping TvMaze API and updating shows.
            services.AddSingleton<IHostedService, Lib.TvMazeScraper>();
            services.AddSingleton<IHostedService, TvMazeUpdater>();

            // Add application services.
            services.AddSingleton<IShowStorage, ShowStorage>();
            services.AddSingleton<ITvMazeUpdateQueue, TvMazeUpdateQueue>();
            services.Configure<CosmosOptions>(Configuration.GetSection("Cosmos"));

            // Add typed HttpClient.
            services
                .AddHttpClient<ITvMazeApi, TvMazeApi>(client =>
                {
                    var baseUrl = Configuration["TvMazeApi:BaseUrl"];
                    client.BaseAddress = new Uri(baseUrl);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
