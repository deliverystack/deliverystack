namespace Deliverystack.DeliveryApi.Models
{
using System;
using System.Collections.Generic;
using System.Threading;

using Deliverystack.Interfaces;
using Deliverystack.Models;

public class RedirectApiCache : ICanNormalizeUrlPaths
{
    public class RedirectRecordModel
    {
        public class PageDataBlock
        {
            public string Url { get; set; }
        }

        public string Url { get; set; }

        public PageDataBlock PageData { get; set; }

        public string GetUrl()
        {
            if (PageData != null && !String.IsNullOrEmpty(PageData.Url))
            {
                return PageData.Url;
            }

            return Url;
        }
    }


    Dictionary<string, string> _redirects;
    IDeliveryClient _deliveryClient = null;

    private Dictionary<string, string> Redirects
    {
        get
        {
            // wait for background threads to finish loading cache
            while (_redirects == null)
            {
                Thread.Sleep(0);
            }

            return _redirects;
        }
    }

    public RedirectApiCache(IDeliveryClient deliveryClient)
    {
        _deliveryClient = deliveryClient;
        new Thread((t) => { Reset(); }).Start();
    }

    public void Reset()
    {
        Dictionary<string, string> redirects
            = new Dictionary<string, string>();

        foreach(RedirectEntryModel redirectEntry in _deliveryClient.Entries<RedirectEntryModel>("redirects"))
        {
            foreach (RedirectData redirect in redirectEntry.Redirects)
            {
                string urlPath = (this as ICanNormalizeUrlPaths).NormalizeUrlPath(redirect.RedirectFrom);

                if (redirect.RedirectToEntry != null 
                    && redirect.RedirectToEntry.Count > 0)
                {
                    RedirectRecordModel pageEntryModel = _deliveryClient.AsA<RedirectRecordModel>(
                        new EntryIdentifier(redirect.RedirectToEntry[0].ContentType,
                        redirect.RedirectToEntry[0].Uid));

                    if (pageEntryModel != null)
                    {
                        redirects[urlPath] = pageEntryModel.GetUrl();
                        continue;
                    }
                }

                redirects[urlPath] = redirect.RedirectToUrl;
            }
        }

        _redirects = redirects;
    }

    public bool ContainsPath(string urlPath)
    {
        urlPath = (this as ICanNormalizeUrlPaths).NormalizeUrlPath(urlPath);
        bool result = Redirects.ContainsKey(urlPath);
        return result;
    }

    public RedirectModel Get(string urlPath)
    {
        urlPath = (this as ICanNormalizeUrlPaths).NormalizeUrlPath(urlPath);

        if (Redirects.ContainsKey(urlPath))
        {
            return new RedirectModel(Redirects[urlPath]);
        }

        return null;
    }
}
}
