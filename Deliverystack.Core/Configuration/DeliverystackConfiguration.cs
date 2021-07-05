namespace Deliverystack.Core.Configuration
{
    using System.Collections.Generic;

    using Microsoft.Extensions.Configuration;

    public class DeliverystackConfiguration
    {
        public bool HostWebApi { get; set; }
        
        public List<string> StaticRoutes { get; set; }
        
        public  DeliverystackConfiguration(IConfigurationSection configuration)
        {
            configuration.Bind(this);
        }
    }
}                                                                                                                       