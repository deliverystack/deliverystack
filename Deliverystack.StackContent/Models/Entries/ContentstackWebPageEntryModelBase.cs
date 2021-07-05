namespace Deliverystack.StackContent.Models.Entries
{
    using Deliverystack.Interfaces;
    using Deliverystack.StackContent.Models.Groups;
    using System;

    public abstract class ContentstackWebPageEntryModelBase : ContentstackEntryModelBase, IWebPageEntryModel
    {

        public PageData PageData { get; set; }

        //TODO: on retrieval in StackContent, use DeliveryApi to set if URL is not empty or /.
        public string ParentUid { get; set; }

        public string GetNavTitle()
        {
            return PageData.NavTitle ?? PageData.PageTitle ?? Title;
        }

        public override string GetTitle()
        {
            if (PageData != null && !String.IsNullOrEmpty(PageData.PageTitle))
            {
                return PageData.PageTitle;
            }

            return Title;
        }

        public string GetUrl()
        {
            return PageData.Url;
        }

        public string GetLayout()
        {
            return PageData.Layout;
        }

        public string GetPartialView()
        {
            return PageData.PartialView;
        }
    }
}
