namespace Metro_Nexterlizer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Windows.Security.Cryptography;
    using Windows.Security.Cryptography.Core;

    public class AmazonProductAdvertisingApi : IAmazonProductAdvertisingApi
    {
        public enum AmazonCountry
        {
            UK,
            US
        }

        public AmazonProductAdvertisingApi(string secretAccessKey, string accessKey)
        {
            this.SecretAccessKey = secretAccessKey;
            this.AccessKey = accessKey;
            this.Country = AmazonCountry.UK;
        }

        internal Func<DateTime> Now = () => DateTime.UtcNow;

        public AmazonCountry Country 
        {
            set
            {
                switch (value)
                {
                    case AmazonCountry.US:
                        {
                            this.BaseUrl = "http://ecs.amazonaws.co.uk/onca/xml?";
                            break;
                        }

                    default:
                        {
                            this.BaseUrl = "http://ecs.amazonaws.co.uk/onca/xml?";
                            break;
                        }
                }
            }
        }

        private string SecretAccessKey { get; set; }

        private string AccessKey { get; set; }

        private string BaseUrl { get; set; }

        public async Task<string> CallAmazon(string searchText)
        {
            HttpClient httpClient = new HttpClient();
            var requestUrl = this.GetUrlForSimilarItemSearch(searchText);
            var response = await httpClient.GetAsync(requestUrl);
            var ret = await response.Content.ReadAsStringAsync();
            return ret;
        }

        public string GetSuggestionTextFromResponse(string xml)
        {
            return "Something else";
        }

        public Uri GetSuggestionUriFromResponse(string xml)
        {
            return new Uri("http://www.google.co.uk");
        }

        internal string GetUrlForSimilarItemSearch(string searchText)
        {
            var queryParams = this.GetQueryParams(searchText);
            queryParams = this.UrlEncodeQueryParams(queryParams);
            var urlToSign = this.GetStringToSign(queryParams);
            var hmac = this.CreateHMAC(urlToSign, "HMAC_SHA256", this.SecretAccessKey);
            var completeUrl = this.GetCompleteUrl(this.BaseUrl, queryParams, hmac);
            return completeUrl;        
        }

        internal IDictionary<string, string> UrlEncodeQueryParams(IDictionary<string, string> queryParams)
        {
            var urlEncodedQueryParams = new SortedDictionary<string, string>();
            foreach (KeyValuePair<string, string> kvp in queryParams)
            {
                urlEncodedQueryParams[kvp.Key] = WebUtility.UrlEncode(kvp.Value);
            }

            return urlEncodedQueryParams;
        }

        internal IDictionary<string, string> GetQueryParams(string searchText)
        {
            var queryParams = new SortedDictionary<string, string>();
            queryParams["Service"] = "AWSECommerceService";
            queryParams["AWSAccessKeyId"] = this.AccessKey;
            queryParams["AssociateTag"] = "jamesthebloom-20";
            queryParams["Operation"] = "ItemSearch";
            queryParams["SearchIndex"] = "Books";
            queryParams["Keywords"] = searchText;
            queryParams["ResponseGroup"] = "Similarities";
            queryParams["Timestamp"] = this.Now().ToString("yyyy-MM-ddTHH:mm:ssZ");
            queryParams["Version"] = "2011-08-01";

            ////TESTING
            ////queryParams["Operation"] = "ItemLookup";
            ////queryParams["ItemId"] = "0679722769";
            ////queryParams["ResponseGroup"]="ItemAttributes,Offers,Images,Reviews";
            ////queryParams["Version"]="2009-01-06"; 
            return queryParams;
        }

        internal string GetCompleteUrl(string baseUrl, IDictionary<string, string> queryParams, string hmac)
        {
            var htmlEncodedHmac = WebUtility.UrlEncode(hmac);
            var flattenedQueryParams = this.GetFlattenedParams(queryParams);
            return string.Format("{0}{1}&Signature={2}", baseUrl, flattenedQueryParams, htmlEncodedHmac);
        }

        internal string GetStringToSign(IDictionary<string, string> queryParams)
        {
            var baseOfStringToSign = this.BaseUrl.Replace(@"http://", "GET\n").Replace(@"/onca/xml?", "\n/onca/xml\n");
            return baseOfStringToSign + this.GetFlattenedParams(queryParams);
        }

        internal string CreateHMAC(
            string message,
            string algorithmName,
            string key)
        {
            MacAlgorithmProvider macAlgorithmProvider = MacAlgorithmProvider.OpenAlgorithm(algorithmName);
            var binaryMessage = CryptographicBuffer.ConvertStringToBinary(message, BinaryStringEncoding.Utf8);
            var binaryKeyMaterial = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            var hmacKey = macAlgorithmProvider.CreateKey(binaryKeyMaterial);
            var binarySignedMessage = CryptographicEngine.Sign(hmacKey, binaryMessage);

            var verified = CryptographicEngine.VerifySignature(hmacKey, CryptographicBuffer.ConvertStringToBinary(message, BinaryStringEncoding.Utf8), binarySignedMessage);

            if (binarySignedMessage.Length != macAlgorithmProvider.MacLength || !verified)
            {
                throw new Exception("Error computing digest");
            }

            var signedMessage = CryptographicBuffer.EncodeToBase64String(binarySignedMessage);
            return signedMessage;
        }

        internal string GetFlattenedParams(IDictionary<string, string> queryParams)
        {
            return queryParams.Aggregate(
                "", 
                (combined, next) => combined = 
                    string.Format(
                    "{0}{1}{2}={3}", 
                    combined, 
                    combined == string.Empty ? string.Empty : "&", 
                    next.Key, next.Value));
        }
    }
}
