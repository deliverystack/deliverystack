namespace Deliverystack.DeliveryApi.Models
{
using Microsoft.Extensions.Configuration;
public class RedirectApiConfig
{
    public RedirectApiConfig(IConfigurationSection config)
    {
        config.Bind(this);
    }

    // public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
    //if (app.ServerFeatures.Get<IServerAddressesFeature>().Addresses.Any())
    // _baseUrl = app.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First();
    public string BaseAddress { get; set; }
}
}
