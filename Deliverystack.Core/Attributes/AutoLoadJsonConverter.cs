namespace Deliverystack.Core.Attributes
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Class)]
    public class AutoLoadJsonConverter : AttributeBase
    {
        /// <summary>
        /// Enabled by default.
        /// </summary>
        public bool Enabled { get; }

        public AutoLoadJsonConverter(bool enabled = true)
        {
            Enabled = enabled;
        }

        public static bool IsEnabledForType(Type t)
        {
            Trace.Assert(t != null, "t is null");

            foreach (var attr in t.GetCustomAttributes(typeof(AutoLoadJsonConverter)))
            {
                AutoLoadJsonConverter ctdAttr = attr as AutoLoadJsonConverter;
                Trace.Assert(ctdAttr != null, "cast is null");

                if (!ctdAttr.Enabled)
                {
                    return ctdAttr.Enabled;
                }
            }

            return true;
        }
    }
}