using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSSA_Covid.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.Azure.Services.AppAuthentication;

namespace MSSA_Covid
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var azureServiceTokenProvider = new AzureServiceTokenProvider();
                    //var keyVaultClient = new KeyVaultClient(d
                    //    new KeyVaultClient.AuthenticationCallback(
                    //        azureServiceTokenProvider.KeyVaultTokenCallback));
                    //var keyVaultEndpoint = builtConfig.GetSection("AzureKeyVault:Endpoint").Value;

                    if (context.HostingEnvironment.IsProduction())
                    {
                        var builtConfig = config.Build();
                        var secretClient = new SecretClient(new Uri(builtConfig["AzureKeyVault:Endpoint"]),
                                                                 new DefaultAzureCredential());
                        config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());

                        //var clientId = builtConfig.GetSection("AzureAd:ClientId").Value;
                        
                        /* Ensure you run these commands:
                         * dotnet user-secrets set "AzureAd:DeveloperSecret" "...SecretValue..."
                         * 
                         */
                        //var clientSecret = builtConfig.GetSection("AzureAd:DeveloperSecret").Value;
                        //config.AddAzureKeyVault(keyVaultEndpoint, clientId, clientSecret);
                    }
                    else
                    {
                        //config.AddAzureKeyVault(keyVaultClient, new DefaultKeyVaultSecretManager());
                        config.AddUserSecrets<Program>();
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                { 
                    webBuilder.UseStartup<Startup>();
                });
    }
}
