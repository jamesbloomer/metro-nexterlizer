namespace Metro_Nexterlizer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    interface IAmazonProductAdvertisingApi
    {
        AmazonProductAdvertisingApi.AmazonCountry Country { set; }

        Task<string> CallAmazon(string searchText);

        string GetSuggestionTextFromResponse(string xml);

        Uri GetSuggestionUriFromResponse(string xml);
    }
}
