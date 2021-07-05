namespace Deliverystack.Base.Models
{
    using System;
    using System.Linq;

    public abstract class QueryStringOptions
    {
        // cannot be a property or it will go infinite loop trying to add this property to the query string
        public string GetQueryString()
        {
            return String.Join("&", GetType().GetProperties().Where(
                prop => prop.GetValue(this, null) != null).Select(
                    prop => $"{Uri.EscapeDataString(prop.Name)}={Uri.EscapeDataString(prop.GetValue(this).ToString())}"));
        }
    }
}
