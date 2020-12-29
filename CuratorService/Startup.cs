using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using AutofacSerilogIntegration;
using CuratorService.Data;
using CuratorService.Database;
using CuratorService.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CuratorService
{
    internal class Startup
    {
        private static readonly string ServiceName = "Curator Service";
        private static readonly string ApiName = $"{ServiceName} API";
        private static readonly string ApiVersion = "v1";
        private static readonly string VersionApiName = $"{ApiName} {ApiVersion}";
        private static readonly string ApiDescription = "Web API for curating photos from various sites.";

        private static readonly string RoutePrefix = "service/CuratorService";
        private static readonly string SwaggerRoutePrefix = $"{RoutePrefix}/swagger";
        private static readonly string SwaggerRouteTemplate = $"/{SwaggerRoutePrefix}/{{documentName}}/swagger.json";
        private static readonly string OpenApiRelativeUrl = $"/{SwaggerRoutePrefix}/{ApiVersion}/swagger.json";

        private readonly string enableCorsOrigins = "_enableCorsOrigins";
        
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(env.ContentRootPath)
                   .AddJsonFile($"{WellKnownData.AppSettings}.json", optional: true, reloadOnChange: true)
                   .AddJsonFile($"{WellKnownData.AppSettings}.{env.EnvironmentName}.json", optional: true)
                   .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: enableCorsOrigins,
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    });
            });

            services.AddMvc()
                .AddNewtonsoftJson(opts => opts.UseMemberCasing())
                .AddMvcOptions(opts => { opts.EnableEndpointRouting = false; });

            services.AddSwaggerGen(c => {
                c.SwaggerDoc(ApiVersion, new OpenApiInfo
                {
                    Title = ApiName,
                    Version = ApiVersion,
                    Description = ApiDescription
                });
                c.EnableAnnotations();
                c.AddSecurityDefinition("curator-token", new OpenApiSecurityScheme()
                {
                    Description = "Authorization header using the curator-token scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "curator-token"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "curator-token"
                            },
                            Scheme = "curator-token",
                            Name = "curator-token",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSwaggerGenNewtonsoftSupport();

            // In production, the Angular files will be served from this directory
//            services.AddSpaStaticFiles(configuration =>
//            {
//                configuration.RootPath = Path.Combine(WellKnownData.ServiceDirPath, "ClientApp/dist");
//            });
        }

        // This only gets called if your environment is Development. The
        // default ConfigureServices won't be automatically called if this
        // one is called.
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // Add things to the service collection that are only for the
            // development environment.
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterLogger();
            builder.Register(c => new CuratorDb()).As<ICuratorDb>().SingleInstance();
            builder.Register(c => new CuratorServiceLogic(c.Resolve<ICuratorDb>())).As<ICuratorServiceLogic>().SingleInstance();
//            builder.Register(c => new PluginManager(c.Resolve<IConfigurationRepository>(), c.Resolve<ISafeguardLogic>())).As<IPluginManager>().SingleInstance();
//            builder.Register(c => new PluginsLogic(c.Resolve<IConfigurationRepository>(), c.Resolve<IPluginManager>(), c.Resolve<ISafeguardLogic>())).As<IPluginsLogic>().SingleInstance();
//            builder.Register(c => new MonitoringLogic(c.Resolve<IConfigurationRepository>(), c.Resolve<IPluginManager>())).As<IMonitoringLogic>().SingleInstance();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // Enable middleware for request logging
            app.UseSerilogRequestLogging();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c => { c.RouteTemplate = SwaggerRouteTemplate; });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(OpenApiRelativeUrl, VersionApiName);
                c.RoutePrefix = SwaggerRoutePrefix;
                c.HeadContent = "https://github.com/bnicholes/CuratorService";
                c.DisplayRequestDuration();
                c.DefaultModelRendering(ModelRendering.Example);
                c.ShowExtensions();
                c.ShowCommonExtensions();
            });

            app.UseExceptionHandler("/Error");
            app.UseHttpsRedirection();
            app.UseCors(enableCorsOrigins);
            app.UseMvc();

//            app.UseStaticFiles();
//            app.UseSpaStaticFiles();
//            app.UseSpa(spa =>
//            {
//                // To learn more about options for serving an Angular SPA from ASP.NET Core,
//                // see https://go.microsoft.com/fwlink/?linkid=864501
//
//                spa.Options.SourcePath = "ClientApp";
//            });
        }
    }
}
