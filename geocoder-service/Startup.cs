﻿using Hangfire;
using Hangfire.Console;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Geocoder
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
            // Other ConfigureServices() code...
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "JAG LCRB GeoCoder Service", Version = "v1" });
            });

            services.AddHangfire(config =>
            {
                // Change this line if you wish to have Hangfire use persistent storage.
                config.UseMemoryStorage();
                // enable console logs for jobs
                config.UseConsole();
            });

            // health checks. 
            services.AddHealthChecks(checks =>
            {
                checks.AddValueTaskCheck("HTTP Endpoint", () => new
                    ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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


            bool startHangfire = true;
#if DEBUG
            // do not start Hangfire if we are running tests.        
            foreach (var assem in Assembly.GetEntryAssembly().GetReferencedAssemblies())
            {
                if (assem.FullName.ToLowerInvariant().StartsWith("xunit"))
                {
                    startHangfire = false;
                    break;
                }
            }
#endif

            if (startHangfire)
            {
                // enable Hangfire, using the default authentication model (local connections only)
                app.UseHangfireServer();

                DashboardOptions dashboardOptions = new DashboardOptions
                {
                    AppPath = null
                };

                app.UseHangfireDashboard("/hangfire", dashboardOptions);
            }

            if (!string.IsNullOrEmpty(Configuration["ENABLE_HANGFIRE_JOBS"]))
            {
                SetupHangfireJobs(app, loggerFactory);
            }


            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "JAG LCRB One Stop Service");
            });
        }



        /// <summary>
        /// Setup the Hangfire jobs.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="loggerFactory"></param>
        private void SetupHangfireJobs(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            ILogger log = loggerFactory.CreateLogger(typeof(Startup));
            log.LogInformation("Starting setup of Hangfire job ...");

            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    log.LogInformation("Creating Hangfire jobs for License issuance check ...");

                    ILogger geocodeLog = loggerFactory.CreateLogger(typeof(GeocodeUtils));
                    RecurringJob.AddOrUpdate(() => new GeocodeUtils(Configuration, geocodeLog).GeocodeEstablishments(null), Cron.Hourly());


                    log.LogInformation("Hangfire geocode jobs setup.");
                }
            }
            catch (Exception e)
            {
                StringBuilder msg = new StringBuilder();
                msg.AppendLine("Failed to setup Hangfire job.");

                log.LogCritical(new EventId(-1, "Hangfire job setup failed"), e, msg.ToString());
            }
        }

    }

}
