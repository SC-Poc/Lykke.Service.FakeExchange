using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.Server;
using Lykke.Logs.Loggers.LykkeSlack;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.Sdk.Middleware;
using Lykke.Service.FakeExchange.Core.Services;
using Lykke.Service.FakeExchange.Services;
using Lykke.Service.FakeExchange.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lykke.Service.FakeExchange
{
    [UsedImplicitly]
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "FakeExchange API",
            ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {                                   
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.Swagger = swagger => swagger.ConfigureSwagger();
                options.SwaggerOptions = _swaggerOptions;

                options.Logs = logs =>
                {
                    logs.AzureTableName = "FakeExchangeLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.FakeExchangeService.Db.LogsConnString;

                    // TODO: You could add extended logging configuration here:
                    /* 
                    logs.Extended = extendedLogs =>
                    {
                        // For example, you could add additional slack channel like this:
                        extendedLogs.AddAdditionalSlackChannel("FakeExchange", channelOptions =>
                        {
                            channelOptions.MinLogLevel = LogLevel.Information;
                        });
                    };
                    */
                };

                // TODO: Extend the service configuration
                /*
                options.Extend = (sc, settings) =>
                {
                    sc
                        .AddOptions()
                        .AddAuthentication(MyAuthOptions.AuthenticationScheme)
                        .AddScheme<MyAuthOptions, KeyAuthHandler>(MyAuthOptions.AuthenticationScheme, null);
                };
                */

                // TODO: You could add extended Swagger configuration here:
                /*
                options.Swagger = swagger =>
                {
                    swagger.IgnoreObsoleteActions();
                };
                */
            });
        }

        private class AlwaysTrueComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return true;
            }

            public int GetHashCode(string obj)
            {
                return nameof(AlwaysTrueComparer).GetHashCode();
            }
        }
        
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            XApiKeyAuthAttribute.Credentials = new Dictionary<string, object>(new AlwaysTrueComparer())
            {
                { "all-api-keys", new object() }
            };
            
            app.UseLykkeConfiguration(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                
                // TODO: Configure additional middleware for eg authentication or maintenancemode checks

                options.WithMiddleware = x =>
                {
                    x.UseAuthenticationMiddleware(token => new FakeApiClient(token, app.ApplicationServices.GetRequiredService<IFakeExchange>()));
                    x.UseHandleBusinessExceptionsMiddleware();
                    x.UseHandleDomainExceptionsMiddleware();
                };

                /*
                options.WithMiddleware = x =>
                {
                    x.UseMaintenanceMode<AppSettings>(settings => new MaintenanceMode
                    {
                        Enabled = settings.MaintenanceMode?.Enabled ?? false,
                        Reason = settings.MaintenanceMode?.Reason
                    });
                    x.UseAuthentication();
                };
                */
            });

        }
    }
}
