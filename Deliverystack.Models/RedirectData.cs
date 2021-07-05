// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace Deliverystack.Models
{
    using System.Collections.Generic;
    public class RedirectData
    {
        public string RedirectFrom { get; set; }

        public List<ReferenceField> RedirectToEntry { get; set; }

        public string RedirectToUrl { get; set; }
    }
}
