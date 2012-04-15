namespace Metro_Nexterlizer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Windows.Security.Cryptography;
    using Windows.Security.Cryptography.Core;

    public class AmazonProductAdvertisingApi
    {
        public string GetUrlForSimilarItemSearch(string searchText)
        {
            var queryParams = this.GetQueryParams(searchText);
            queryParams = this.UrlEncodeQueryParams(queryParams);
            var urlToSign = this.GetStringToSign(queryParams);
            var hmac = this.CreateHMAC(urlToSign, "HMAC_SHA256", "SECRET ACCESS KEY");
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
            queryParams["AWSAccessKeyId"] = "ACCES KEY";
            queryParams["AssociateTag"] = "jamesthebloom-20";
            queryParams["Operation"] = "ItemSearch";
            queryParams["SearchIndex"] = "Books";
            queryParams["Keywords"] = searchText;
            queryParams["ResponseGroup"] = "Similarities";
            queryParams["Timestamp"] = DateTime.Now.ToString(); // need  [YYYY-MM-DDThh:mm:ssZ]
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
