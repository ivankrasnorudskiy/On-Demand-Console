using ITSS.Repository.Relay.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITSS.Repository.Relay
{
    public class RelayDependencies
    {
        public static void RegisterByWebApiStartup(IServiceCollection services, IConfiguration configuration)
        {
            var relayParam = GetRelayParamsFromAppSettingsJson(configuration);
            services.AddTransient<IRelayListener, RelayListener>(pr => new RelayListener(relayParam));
            services.AddTransient<IRelaySender, RelaySender>(pr => new RelaySender(relayParam));
        }

        private static RelayParam GetRelayParamsFromAppSettingsJson(IConfiguration configuration)
        {
            var relayParameters = new RelayParam
            {
                AzureRelayKey = configuration.GetSection("AzureRelayParameters")["AzureRelayKey"],
                AzureRelayKeyName = configuration.GetSection("AzureRelayParameters")["AzureRelayKeyName"],
                AzureRelayConnectionName = configuration.GetSection("AzureRelayParameters")["AzureRelayConnectionName"],
                AzureRelayNamespace = configuration.GetSection("AzureRelayParameters")["AzureRelayNamespace"]
            };
             return relayParameters;
        }

        public static RelayParam GetRelayParamFromAppConfig()
        {
            var relayParameters = new RelayParam
            {
                AzureRelayKey = ConfigurationUtils.GetStringValueFromAppSettings("AzureRelayKey"),
                AzureRelayKeyName = ConfigurationUtils.GetStringValueFromAppSettings("AzureRelayKeyName"),
                AzureRelayConnectionName = ConfigurationUtils.GetStringValueFromAppSettings("AzureRelayConnectionName"),
                AzureRelayNamespace = ConfigurationUtils.GetStringValueFromAppSettings("AzureRelayNamespace")
            };
            return relayParameters;
        }
    }
}
