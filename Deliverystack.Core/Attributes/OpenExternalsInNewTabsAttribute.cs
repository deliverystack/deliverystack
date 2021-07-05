namespace Deliverystack.Core.Attributes
{
    using System;
    
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OpenExternalsInNewTabsAttribute : Attribute
    {
        public OpenExternalsInNewTabsAttribute(bool value)
        {
            Value = value;
        }
        public bool Value { get; }
    }
}