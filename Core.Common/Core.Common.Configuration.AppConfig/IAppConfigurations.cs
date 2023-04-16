using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace Core.Common.Configuration.AppConfig
{
    public interface IAppConfigurations
    {
        string GetAppConfigValue(string key);
    }

    public class AppConfigurations : IAppConfigurations
    {
        private readonly IConfiguration configurations;
        private IConfigurationRefresher _refresher;
        public AppConfigurations(string appConfigConnectionString, string label = "")
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(options =>
            {
                //Vivek - Filter by lables - will return only app configs with labels
                if (!string.IsNullOrEmpty(label))
                {
                    options.Connect(appConfigConnectionString)
                    .Select("*", label)//Vivek - keys are not filter with starts with prefix, so we get all the keys along with common settings(not service specific)
                    //Vivek - select can be called multiple times to load lets say if there are any setting that are common to all services
                    //.Select("commonsettings" + "*", label)
                    .ConfigureKeyVault(options =>
                    {
                        //Vivek - DefaultAzureCredential is smart enough that it will check #1 if Managed identiies can be used, else #2 if the service is running local it will check if user is logged into Azure CLI/VS. #3 etc
                        options.SetCredential(new DefaultAzureCredential());
                    }).ConfigureRefresh(refreshOptions =>
                    {
                        refreshOptions.Register("Sentinel", label, true)
                        .SetCacheExpiration(TimeSpan.FromSeconds(10));
                    });
                    _refresher = options.GetRefresher();
                }
                //Vivek - This will return only app configs without label
                else
                {
                    options.Connect(appConfigConnectionString)
                    .ConfigureRefresh(refreshOptions =>
                    {
                        refreshOptions.Register("Sentinel", label, true)
                        .SetCacheExpiration(TimeSpan.FromSeconds(10));
                    });
                    _refresher = options.GetRefresher();
                }
            });
            //Vivek - Build will not add the values to appsettings.json, this configuration is private field not the one mapped to appsettings.json
            configurations = builder.Build();
        }

        public string GetAppConfigValue(string key)
        {
            //refresh the config values before fetch
            //_refresher.RefreshAsync();
            return configurations[key];
        }
    }


}
