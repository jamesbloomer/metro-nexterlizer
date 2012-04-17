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

    public class AmazonProductAdvertisingApi
    {
        public AmazonProductAdvertisingApi(string secretAccessKey, string accessKey)
        {
            this.SecretAccessKey = secretAccessKey;
            this.AccessKey = accessKey;
        }

        internal Func<DateTime> Now = () => DateTime.Now;

        private string SecretAccessKey { get; set; }

        private string AccessKey { get; set; }

        public async Task<string> CallAmazon(string searchText)
        {
            HttpClient httpClient = new HttpClient();
            var requestUrl = this.GetUrlForSimilarItemSearch(searchText);
            var response = await httpClient.GetAsync(requestUrl);
            var ret = await response.Content.ReadAsStringAsync();
            return ret;
        }

        public string GetUrlForSimilarItemSearch(string searchText)
        {
            var queryParams = this.GetQueryParams(searchText);
            queryParams = this.UrlEncodeQueryParams(queryParams);
            var urlToSign = this.GetStringToSign(queryParams);
            var hmac = this.CreateHMAC(urlToSign, "HMAC_SHA256", this.SecretAccessKey);
            var baseUkUrl = @"http://ecs.amazonaws.co.uk/onca/xml";
            var completeUrl = this.GetCompleteUrl(baseUkUrl, queryParams, hmac);
            return null;        
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
            queryParams["Timestamp"] = this.Now().ToString("yyyy-MM-ddThh:mm:ssZz"); 
            queryParams["Version"] = "2011-08-01";
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
            return "GET\necs.amazonaws.co.uk\n/onca/xml\n" + this.GetFlattenedParams(queryParams);
        }

        internal string CreateHMAC(
            string message,
            string algorithmName,
            string keyMaterial)
        {
            MacAlgorithmProvider macAlgorithmProvider = MacAlgorithmProvider.OpenAlgorithm(algorithmName);
            var binaryMessage = CryptographicBuffer.ConvertStringToBinary(message, BinaryStringEncoding.Utf8);
            var binaryKeyMaterial = CryptographicBuffer.ConvertStringToBinary(keyMaterial, BinaryStringEncoding.Utf8);
            var hmacKey = macAlgorithmProvider.CreateKey(binaryKeyMaterial);

            var binarySignedMessage = CryptographicEngine.Sign(hmacKey, binaryMessage);

            if (binarySignedMessage.Length != macAlgorithmProvider.MacLength)
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
