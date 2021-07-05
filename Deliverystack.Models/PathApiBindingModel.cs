// ReSharper disable RedundantDefaultMemberInitializer
// ReSharper disable RedundantNameQualifier
namespace Deliverystack.Models
{
    using System;
    
    using Deliverystack.Interfaces;

    public class PathApiBindingModel : ICanBeQueryStringParams
    {
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        public string Path { get; set; } = null;

        public int Ancestors { get; set; } = 0;

        public int Descendants { get; set; } = 0;

        public bool ExcludeSelf { get; set; } = false;

        public bool Siblings { get; set; } = false;

        public int PageIndex { get; set; } = 0;

        private int _pageSize = 100;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }

            set
            {
                if (value > 100)
                {
                    throw new ArgumentException("PageSize exceeded");
                }

                _pageSize = value;
            }
        }
    }
}