using System;
using Azure.Identity;
using AzureFunctions;
using AzureFunctions.Services;
using AzureFunctions.Validation;
using FluentValidation;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(Startup))]  
namespace AzureFunctions  
{  
    public class Startup : FunctionsStartup  
    {  
        public override void Configure(IFunctionsHostBuilder builder)  
        {  
            builder.Services.AddSingleton<ICosmosDbService, CosmosDbService>();
            
            builder.Services.AddScoped<IFlightBookingService, FlightBookingService>();
            builder.Services.AddScoped<IHotelBookingService, HotelBookingService>();
            builder.Services.AddScoped<ICarRentService, CarRentService>();
            
            builder.Services.AddValidatorsFromAssemblyContaining<HotelValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CarValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<FlightValidator>();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var builtConfig = builder.ConfigurationBuilder.Build();
            var keyVaultName = builtConfig["KeyVaultName"];
            
            builder.ConfigurationBuilder
                .AddAzureKeyVault(new Uri($"https://{keyVaultName}.vault.azure.net/"), new DefaultAzureCredential());
            
            base.ConfigureAppConfiguration(builder);
        }
    }  
}