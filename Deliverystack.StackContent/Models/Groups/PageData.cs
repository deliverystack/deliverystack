namespace Deliverystack.StackContent.Models.Groups
{
using Deliverystack.StackContent.Models.Fields;

public class PageData
{
    private string _navTitle;

    public string PageTitle { get; set; }

    public string NavTitle
    {
        set
        {
            _navTitle = value;
        }

        get
        {
            return _navTitle ?? PageTitle;
        }
    }

    public string Layout { get; set; }
    public string PartialView { get; set; }

    public string Url { get; set; }

    public string PageDescription { get; set; }

    public string SearchKeywords { get; set; }

    public bool ExcludeFromSearch { get; set; }

    public bool IncludeInSitemap { get; set; }

    public string SearchChangeFrequency { get; set; }

    public string SearchPriority { get; set; }

    public FileField OpenGraphImage { get; set; }
    
    public string GoogleAnalyticsId { get; set; }
    
    public string FavIcon { get; set; }
}        
}
