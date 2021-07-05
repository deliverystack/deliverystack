namespace Deliverystack.DeliveryApi.Models
{
    using Microsoft.Extensions.Configuration;
    public class PathApiConfig
    {
        public PathApiConfig(IConfigurationSection config)
        {
            config.Bind(this);
        }

        // public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        //if (app.ServerFeatures.Get<IServerAddressesFeature>().Addresses.Any())
        // _baseUrl = app.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First();
        public string BaseAddress { get; set; }
    }
}
