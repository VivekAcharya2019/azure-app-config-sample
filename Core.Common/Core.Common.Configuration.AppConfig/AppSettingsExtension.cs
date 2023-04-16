using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Core.Common.Configuration.AppConfig
{
    public static class AppSettingsExtension
    {
        private static string keyPrefix;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configurations"></param>
        /// <param name="labelFilter">Used to filter all app config, it can env like dev, qa, test</param>
        /// 
        public static void ReadAzureAppConfigValues(this IServiceCollection services, IConfiguration configurations, string labelFilter = "")
        {
            services.AddScoped<IAppConfigurations, AppConfigurations>(serviceProvider =>
            {
                return new AppConfigurations(configurations.GetSection("AppConfigSettings:ConnectionString").Value, labelFilter);
            });
        }

        public static void UpdateAppSettingsFromAzureAppConfig(this IServiceCollection services, IConfiguration configurations, string prefix)
        {
            keyPrefix = prefix;
            var appConfigService = services.BuildServiceProvider().GetRequiredService<IAppConfigurations>();
            var children = configurations.GetChildren();
            if (children.Any())
            {
                var keys = new List<string>();
                ReadFromAppConfig(appConfigService, configurations, children, keys);
            }
        }

        private static void ReadFromAppConfig(IAppConfigurations appConfigService, IConfiguration configurations, IEnumerable<IConfigurationSection> children, List<string> keys)
        {
            foreach (var section in children)
            {
                if (section.Key == "AppConfigSettings")
                    continue;
                keys.Add(section.Key);

                //Vivek - if the child is a nested object
                if (section.GetChildren().Any())
                    ReadFromAppConfig(appConfigService, configurations, section.GetChildren(), keys);
                else
                    GetConfigurationValue(appConfigService, configurations, keys);
                if (keys.Any())
                {
                    keys.RemoveAt(keys.Count - 1);
                }
            }
        }

        private static void GetConfigurationValue(IAppConfigurations appConfigService, IConfiguration configurations, List<string> keys)
        {
            if (keys.Any())
            {
                string valueFromAppConfig = string.Empty;
                //Vivek - in azure app config, nested json object are seperated by . so join by . and then find the value from app config. It can be : as well no issues with :
                var appConfigKey = string.Join(".", keys);

                if (!string.IsNullOrEmpty(keyPrefix))
                {
                    valueFromAppConfig = appConfigService.GetAppConfigValue(keyPrefix + "." + appConfigKey);
                }

                //Vivek - app config does not has the value when searched with prefix, so it might be a common setting (which will not have service name as prefix)
                if (string.IsNullOrEmpty(valueFromAppConfig) && !string.IsNullOrEmpty(keyPrefix))
                {
                    valueFromAppConfig = appConfigService.GetAppConfigValue(appConfigKey);
                }

                if (valueFromAppConfig != null)
                {
                    //Vivek - in appsetting nested object are read with : seperator
                    var appSettingPath = string.Join(":", keys);
                    configurations[appSettingPath] = valueFromAppConfig;
                }
            }
        }
    }
}
