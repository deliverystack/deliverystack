// ReSharper disable RedundantNameQualifier
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Deliverystack.Models
{
    using Deliverystack.Interfaces;

    public class RedirectModel : ICanBeQueryStringParams
    {
        public RedirectModel()
        {
        }

        public RedirectModel(string urlPath)
        {
            Url = urlPath;
        }

        public string Url { get; set; }
    }
}
