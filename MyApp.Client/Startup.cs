using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Shift;

namespace MyApp.Client
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMemoryCache();
            services.AddMvc().AddJsonOptions(o => o.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddOptions();
            services.Configure<ShiftConfig>(Configuration.GetSection("Shift"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IMemoryCache cache)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            ConfigureShiftServer(cache);
        }

        private void ConfigureShiftServer(IMemoryCache cache)
        {
            var cacheOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove).SetAbsoluteExpiration(TimeSpan.FromDays(999999)); //never expires?

            var shiftConfig = Configuration.GetSection("Shift");
            //Shift Client
            var clientConfig = new Shift.ClientConfig();
            clientConfig.DBConnectionString = shiftConfig["ShiftDBConnection"];
            clientConfig.DBAuthKey = shiftConfig["DocumentDBAuthKey"];
            clientConfig.EncryptionKey = shiftConfig["ShiftEncryptionParametersKey"]; //optional, will encrypt parameters in DB if exists
            clientConfig.StorageMode = shiftConfig["StorageMode"];
            cache.Set("Shift.JobClient", new JobClient(clientConfig), cacheOptions); //only the DBConnectionString and CacheConfigurationString are required for Client's background job

            //Shift Server
            var serverConfig = new Shift.ServerConfig();
            serverConfig.DBConnectionString = shiftConfig["ShiftDBConnection"];
            serverConfig.DBAuthKey = shiftConfig["DocumentDBAuthKey"];
            serverConfig.EncryptionKey = shiftConfig["ShiftEncryptionParametersKey"]; //optional, will encrypt parameters in DB if exists
            serverConfig.MaxRunnableJobs = Convert.ToInt32(shiftConfig["MaxRunnableJobs"]);
            //serverConfig.ProcessID = shiftConfig["ShiftPID"];
            serverConfig.Workers = Convert.ToInt32(shiftConfig["ShiftWorkers"]);

            serverConfig.StorageMode = shiftConfig["StorageMode"];
            var progressDBInterval = shiftConfig["ProgressDBInterval"];
            if (!string.IsNullOrWhiteSpace(progressDBInterval))
                serverConfig.ProgressDBInterval = TimeSpan.Parse(progressDBInterval); //Interval when progress is updated in main DB

            var autoDeletePeriod = shiftConfig["AutoDeletePeriod"];
            serverConfig.AutoDeletePeriod = string.IsNullOrWhiteSpace(autoDeletePeriod) ? null : (int?)Convert.ToInt32(autoDeletePeriod);
            serverConfig.AutoDeleteStatus = new List<JobStatus?> { JobStatus.Completed }; //Auto delete only the jobs that had Stopped or with Error

            serverConfig.ForceStopServer = Convert.ToBoolean(shiftConfig["ForceStopServer"]); //Set to true to allow windows service to shut down after a set delay in StopServerDelay
            serverConfig.StopServerDelay = Convert.ToInt32(shiftConfig["StopServerDelay"]);

            //serverConfig.ServerTimerInterval = Convert.ToInt32(shiftConfig["ServerTimerInterval"]); //optional: default every 5 sec for server running jobs
            //serverConfig.ServerTimerInterval2 = Convert.ToInt32(shiftConfig["ServerTimerInterval2"]); //optional: default every 10 sec for server CleanUp()
            //serverConfig.AssemblyFolder = shiftConfig["AssemblyFolder"];
            //serverConfig.AssemblyListPath = shiftConfig["AssemblyListPath"]; 

            serverConfig.PollingOnce = Convert.ToBoolean(shiftConfig["PollingOnce"]);

            //For this demo, we're running the background process server in the same process as the web client
            //It's recommended to run the server in a separate process, such as windows service or Azure WebJob or another app.
            cache.Set("Shift.JobServer", new Shift.JobServer(serverConfig), cacheOptions); //only the DBConnectionString and CacheConfigurationString are required for Client's background job
        }
    }
}
