using Core.Common.Configuration.AppConfig;

namespace MyService.TestAPI
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            //Vivek - configure Azure app config - approach #1
            services.ReadAzureAppConfigValues(configuration, configuration["Namespace:Name"]);
            services.UpdateAppSettingsFromAzureAppConfig(configuration, configuration["app:name"]);

            //var appConfigurations =  services.BuildServiceProvider().GetRequiredService<IAppConfigurations>();
            //var connectionstring = appConfigurations.GetAppConfigValue("nolabelkey");

            //Vivek - configure Azure app config - approach #2 
            #region approach 2
            //var builder = new ConfigurationBuilder();
            //builder.AddAzureAppConfiguration(configuration["AppConfigSettings:ConnectionString"]);
            //var config = builder.Build();
            //this config will have the value from azure app config. But this will not be available in app setting configurations/IConfigurations. This needs to be updated manually
            //services.AddAzureAppConfiguration();
            #endregion 

            // Add services to the container.
            services.AddControllers();
            //services.AddEndpointsApiExplorer(); alreay called inside AddControllers()
            services.AddSwaggerGen();
            services.AddOptions();
            services.Configure<Logging>(opt => configuration.GetSection(typeof(Logging).Name).Bind(opt));
            services.Configure<AppConfigSettings>(opt => configuration.GetSection(typeof(AppConfigSettings).Name).Bind(opt));
            //Vivek - get app settings into strongly typed class
            var appSet = new AppConfigSettings();
            configuration.GetSection(nameof(AppConfigSettings)).Bind(appSet);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseRouting();
            //app.UseAuthentication(); why do we need this middleware?
            app.UseEndpoints(ep => ep.MapControllers());

            //Vivek - if there are any update to the configuration, this method needs to be there to refresh the config, and register 
            //app.UseAzureAppConfiguration();
        }
    }
}
