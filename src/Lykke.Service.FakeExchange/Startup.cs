using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.Server;
using Lykke.Sdk;
using Lykke.Service.FakeExchange.Domain.Services;
using Lykke.Service.FakeExchange.DomainServices;
using Lykke.Service.FakeExchange.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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
                    logs.AzureTableConnectionStringResolver = settings
                        => settings.FakeExchangeService.Db.LogsConnectionString;
                };
            });
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

                options.WithMiddleware = x =>
                {
                    x.UseAuthenticationMiddleware(token => new FakeApiClient(token, app.ApplicationServices.GetRequiredService<IFakeExchange>()));
                    x.UseHandleBusinessExceptionsMiddleware();
                    x.UseHandleDomainExceptionsMiddleware();
                };
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
    }
}
