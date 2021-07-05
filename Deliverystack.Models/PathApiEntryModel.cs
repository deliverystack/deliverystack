// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace Deliverystack.Models
{
    using System;
    public class PathApiEntryModel
    {
        public class PageProperties
        {
            public string PageTitle { get; set; }
            public string NavTitle { get; set; }
            public string Url { get; set; }
            public string GoogleAnalyticsId { get; set; }
            
            public string FavIcon { get; set; }
            
            //TODO: match name elsewhere (deliverystack) - includeinsitemap?
            public bool IncludeInXmlSitemap { get; set; }
        }

        public PageProperties PageData { get; set; }

        public string ContentType { get; set; }

        public string Uid { get; set; }

        private string _url;

        // ReSharper disable once MemberCanBePrivate.Global
        public string Url
        {
            get
            {
                if (String.IsNullOrEmpty(_url))
                {
                    _url = PageData?.Url;
                }

                return _url;
            }

            set
            {
                _url = value;
            }
        }
        
        private string _googleAnalyticsId;

        public string GoogleAnalyticsId
        {
            get
            {
                if (String.IsNullOrEmpty(_googleAnalyticsId) && PageData != null)
                {
                    _googleAnalyticsId = PageData?.GoogleAnalyticsId;
                }

                return _googleAnalyticsId;
            }

            set { _googleAnalyticsId = value; }
        }

        public string Title { get; set; }

        public int Depth
        {
            get
            {
                return Url.Length - Url.Replace("/", "").Length;
            }
        }

        public string NavTitle
        {
            get
            {
                return PageData?.NavTitle ?? PageData?.PageTitle ?? Title;
            }
        }

        public string PageTitle
        {
            get
            {
                return PageData?.PageTitle ?? Title;   
            }
        }
        
        private string _favIcon;
        
        public string FavIcon
        {
            get
            {
                if (String.IsNullOrEmpty(_favIcon))
                {
                    _favIcon = PageData?.FavIcon ?? String.Empty;
                }

                return _favIcon;
            }

            set
            {
                _favIcon = value;
            }
        }
    }
}
