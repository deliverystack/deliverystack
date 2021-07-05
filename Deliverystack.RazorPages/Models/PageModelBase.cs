// ReSharper disable RedundantNameQualifier
namespace Deliverystack.RazorPages.Models
{
    using System;
    using System.Text.Json;
    
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    using Deliverystack.DeliveryApi.Models;                                                                            
    using Deliverystack.Interfaces;
    using Deliverystack.Models;

    public abstract class PageModelBase : PageModel
    {
        // content delivery client
        private readonly IDeliveryClient _deliveryClient;
        
        // URL path logic client
        readonly PathApiClient _pathApiClient;
                                                                                                                        
        // CMS URL redirect logic
        readonly RedirectApiClient _redirectApiClient;

        // entry specified in URL
        // ReSharper disable once MemberCanBePrivate.Global
        public IWebPageEntryModel EntryModel { get; internal set; }

        // dependencies configured in Startup.cs inject arguments
        protected PageModelBase(
            IDeliveryClient deliveryClient,
            PathApiClient pathApiClient,
            RedirectApiClient redirectClient)
        {
            _deliveryClient = deliveryClient;
            _pathApiClient = pathApiClient;
            _redirectApiClient = redirectClient;
        }

        // json=true renders entry as JSON rather than HTML
        [FromQuery(Name = "json")]
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool Json { get; set; }

        // determine this.EntryModel from URL path. 
        // if none, apply redirect from CMS accordingly.
        // if none, apply HTTP 404. 
        // ReSharper disable once UnusedMember.Global
        public ActionResult OnGet(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                path = "/";
            }

            PathApiEntryModel entry = _pathApiClient.GetEntry(
                new PathApiBindingModel { Path = path });

            if (entry != null)
            {
                Object entryModel = _deliveryClient.AsDefaultEntryModel(
                    new EntryIdentifier(entry.ContentType, entry.Uid));

                if (entryModel != null)
                {
                    EntryModel = entryModel as IWebPageEntryModel;

                    if (EntryModel != null)
                    {
                        if (Json)
                        {
                            return Content(JsonSerializer.Serialize(
                                EntryModel.GetJson(),
                                new JsonSerializerOptions {WriteIndented = true}));
                        }

                        return Page();
                    }
                }
            }

            RedirectModel redirect = _redirectApiClient.Get(path);

            if (redirect != null)
            {
                return Redirect(redirect.Url);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
