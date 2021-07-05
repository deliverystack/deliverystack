namespace Deliverystack.Models
{
using System.Collections.Generic;

public class PathApiResultModel
{
    public int Ancestors { get; set; }

    public int CurrentGeneration { get; set; }

    public int Descendants { get; set;  }

    public int Total
    {
        get
        {
            return Ancestors + CurrentGeneration + Descendants;
        }
    }

    public IEnumerable<PathApiEntryModel> Entries { get; set; }
}
}
